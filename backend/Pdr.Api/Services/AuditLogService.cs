using System.Text.Json;
using Pdr.Api.DTOs.Audit;
using Pdr.Api.DTOs.Common;
using Pdr.Api.Models.Audit;
using Pdr.Api.Repositories.Interfaces;
using Pdr.Api.Services.Interfaces;
using Pdr.Api.Services.Notifications;

namespace Pdr.Api.Services;

public class AuditLogService(
    IAuditLogRepository repository,
    ICurrentUserService currentUserService,
    TemplateRenderer templateRenderer) : IAuditLogService
{
    public async Task LogAsync(AuditActionEnum action, IDictionary<string, object?> payload)
    {
        var userId = currentUserService.GetUserId();

        var auditAction = await repository.GetAuditActionByIdAsync((int)action)
            ?? throw new InvalidOperationException($"No AuditAction row found for {action}.");

        await repository.AddAsync(new AuditLog
        {
            UserId = userId,
            AuditActionId = auditAction.AuditActionId,
            PayloadJson = JsonSerializer.Serialize(payload)
        });

        await repository.SaveChangesAsync();
    }

    public async Task<CursorPagedResult<AuditLogDto>> GetAuditLogsAsync(int? cursor, int limit)
    {
        var logs = await repository.GetPageAsync(cursor, limit);

        var hasMore = logs.Count > limit;
        var pageLogs = hasMore ? logs.Take(limit).ToList() : logs;

        var items = pageLogs.Select(MapToDto).ToList();

        return new CursorPagedResult<AuditLogDto>
        {
            Items = items,
            NextCursor = hasMore ? pageLogs[^1].AuditLogId : null,
            HasMore = hasMore
        };
    }

    private AuditLogDto MapToDto(AuditLog auditLog)
    {
        var payload = JsonSerializer.Deserialize<Dictionary<string, object?>>(auditLog.PayloadJson) ?? new();

        return new AuditLogDto
        {
            AuditLogId = auditLog.AuditLogId,
            UserId = auditLog.UserId,
            UserName = auditLog.User?.FullName ?? string.Empty,
            Action = auditLog.AuditAction?.Action ?? string.Empty,
            Details = templateRenderer.Render(auditLog.AuditAction?.Details ?? string.Empty, payload),
            CreatedAt = auditLog.CreatedAt
        };
    }
}
