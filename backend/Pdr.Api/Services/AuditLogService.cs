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

        var payloadJson = JsonSerializer.Serialize(payload);

        await repository.AddAsync(new AuditLog
        {
            UserId = userId,
            AuditActionId = auditAction.Id,
            PayloadJson = payloadJson,
            EngagementId = ExtractEngagementId(payloadJson)
        });

        await repository.SaveChangesAsync();
    }


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
    /// Retrieves a cursor-paginated page of audit log entries for a specific engagement.
    /// The database efficiently filters by EngagementId, so we can fetch exactly one batch.
    /// </summary>
    public async Task<CursorPagedResult<EngagementAuditLogDto>> GetEngagementAuditLogsAsync(int engagementId, int? cursor, int limit)
    {
        var items = new List<EngagementAuditLogDto>();
        
        // Make exactly 1 database call
        var batch = await repository.GetPageAsync(cursor, limit, userId: null, engagementId: engagementId);

        var hasMore = batch.Count > limit;
        var examined = hasMore ? batch.Take(limit).ToList() : batch;

        if (examined.Count == 0)
        {
            return new CursorPagedResult<EngagementAuditLogDto>
            {
                Items = items,
                NextCursor = null,
                HasMore = false
            };
        }

        foreach (var log in examined)
        {
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

            var dto = await MapToDtoAsync(log);
            items.Add(new EngagementAuditLogDto
            {
                Id = dto.Id,
                UserId = dto.UserId,
                UserName = dto.UserName,
                Action = dto.Action,
                Details = dto.Details,
                CreatedAt = dto.CreatedAt
            });
        }

        return new CursorPagedResult<EngagementAuditLogDto>
        {
            Items = items,
            NextCursor = hasMore ? examined[^1].Id : null,
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
            EngagementId = auditLog.EngagementId
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
