using System.Text.Json;
using Pdr.Api.DTOs.Audit;
using Pdr.Api.DTOs.Common;
using Pdr.Api.Models.Audit;
using Pdr.Api.Models.LoginEntity;
using Pdr.Api.Repositories.Interfaces;
using Pdr.Api.Services.Audit;
using Pdr.Api.Services.Interfaces;
using Pdr.Api.Services.Templating;

namespace Pdr.Api.Services;

public class AuditLogService(
    IAuditLogRepository repository,
    ICurrentUserService currentUserService,
    TemplateMapperResolver<IAuditTemplateMapper> mapperResolver,
    TemplateRenderer templateRenderer) : IAuditLogService
{
    public async Task LogAsync(AuditActionEnum action, IDictionary<string, object?> payload, int? actingUserId = null)
    {
        var userId = actingUserId ?? currentUserService.GetUserId();

        var auditAction = await repository.GetAuditActionByIdAsync((int)action)
            ?? throw new InvalidOperationException($"No AuditAction row found for {action}.");

        await repository.AddAsync(new AuditLog
        {
            UserId = userId,
            AuditActionId = auditAction.Id,
            PayloadJson = JsonSerializer.Serialize(payload)
        });

        await repository.SaveChangesAsync();
    }

    private const int MaxScanRows = 500;

    public async Task<CursorPagedResult<AuditLogDto>> GetAuditLogsAsync(int? cursor, int limit)
    {
        var isElevated = currentUserService.IsInRole(RoleEnum.Leadership) || currentUserService.IsAdmin();
        var scopedUserId = isElevated ? (int?)null : currentUserService.GetUserId();

        var logs = await repository.GetPageAsync(cursor, limit, scopedUserId);

        var hasMore = logs.Count > limit;
        var pageLogs = hasMore ? logs.Take(limit).ToList() : logs;

        var items = new List<AuditLogDto>();
        foreach (var log in pageLogs)
        {
            items.Add(await MapToDtoAsync(log));
        }

        return new CursorPagedResult<AuditLogDto>
        {
            Items = items,
            NextCursor = hasMore ? pageLogs[^1].Id : null,
            HasMore = hasMore
        };
    }

    /// <summary>
    /// EngagementId isn't an indexed column (it's read out of PayloadJson, see MapToDtoAsync), so
    /// this can't be pushed down as a SQL WHERE clause. Instead it scans batches of rows newest-first,
    /// filtering after deserialization, advancing the cursor past every row examined (not just
    /// matches) so a follow-up request resumes scanning rather than re-checking old rows. Bounded by
    /// MaxScanRows so a request can't scan the entire table in one call.
    /// </summary>
    public async Task<CursorPagedResult<EngagementAuditLogDto>> GetEngagementAuditLogsAsync(int engagementId, int? cursor, int limit)
    {
        int? scopedUserId = null; // irrespective of who the actor was
        var items = new List<EngagementAuditLogDto>();
        var currentCursor = cursor;
        var rowsScanned = 0;
        var hasMore = false;

        while (items.Count < limit && rowsScanned < MaxScanRows)
        {
            var batchSize = Math.Min(limit, MaxScanRows - rowsScanned);
            var batch = await repository.GetPageAsync(currentCursor, batchSize, scopedUserId);

            if (batch.Count == 0)
            {
                break;
            }

            var batchHasMore = batch.Count > batchSize;
            var examined = batchHasMore ? batch.Take(batchSize).ToList() : batch;

            foreach (var log in examined)
            {
                rowsScanned++;
                var actionString = log.AuditAction?.Action ?? string.Empty;
                if (!Enum.TryParse<AuditActionEnum>(actionString, out var actionEnum))
                {
                    continue;
                }

                if (!EngagementAuditActionVisibilityRegistry.Entries.TryGetValue(actionEnum, out var visibility))
                {
                    continue;
                }

                var roles = visibility.VisibleToRoles;
                if (roles == null || roles.Length == 0)
                {
                    continue;
                }

                bool hasAccess = currentUserService.IsAdmin() || roles.Any(r => currentUserService.IsInRole(r));
                if (!hasAccess)
                {
                    continue;
                }

                var logEngagementId = ExtractEngagementId(log.PayloadJson);
                if (logEngagementId != engagementId)
                {
                    continue;
                }

                var dto = await MapToDtoAsync(log);
                {
                    items.Add(new EngagementAuditLogDto
                    {
                        Id = dto.Id,
                        UserId = dto.UserId,
                        UserName = dto.UserName,
                        Action = dto.Action,
                        Details = dto.Details,
                        CreatedAt = dto.CreatedAt
                    });
                    if (items.Count == limit)
                    {
                        hasMore = true;
                        break;
                    }
                }
            }

            currentCursor = examined[^1].Id;

            if (items.Count == limit)
            {
                hasMore = true;
                break;
            }

            if (!batchHasMore)
            {
                hasMore = false;
                break;
            }

            hasMore = rowsScanned >= MaxScanRows;
        }

        return new CursorPagedResult<EngagementAuditLogDto>
        {
            Items = items,
            NextCursor = hasMore ? currentCursor : null,
            HasMore = hasMore
        };
    }

    private async Task<AuditLogDto> MapToDtoAsync(AuditLog auditLog)
    {
        var actionKey = auditLog.AuditAction?.Action ?? string.Empty;

        var mapper = mapperResolver.Resolve(actionKey)
            ?? throw new InvalidOperationException($"No IAuditTemplateMapper registered for action {actionKey}.");

        var values = await mapper.MapAsync(auditLog.PayloadJson);

        return new AuditLogDto
        {
            Id = auditLog.Id,
            UserId = auditLog.UserId,
            UserName = auditLog.User?.FullName ?? string.Empty,
            Action = actionKey,
            Details = templateRenderer.Render(auditLog.AuditAction?.Details ?? string.Empty, values),
            CreatedAt = auditLog.CreatedAt,
            EngagementId = ExtractEngagementId(auditLog.PayloadJson)
        };
    }

    private static int? ExtractEngagementId(string payloadJson)
    {
        using var document = JsonDocument.Parse(payloadJson);

        foreach (var property in document.RootElement.EnumerateObject())
        {
            if (!string.Equals(property.Name, "EngagementId", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (property.Value.ValueKind == JsonValueKind.Number && property.Value.TryGetInt32(out var numeric))
            {
                return numeric;
            }

            if (property.Value.ValueKind == JsonValueKind.String && int.TryParse(property.Value.GetString(), out var parsed))
            {
                return parsed;
            }
        }

        return null;
    }
}
