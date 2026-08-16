using System.Text.Json;
using Pdr.Api.DTOs.Audit;
using Pdr.Api.DTOs.Common;
using Pdr.Api.Models.Audit;
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

        var items = new List<AuditLogDto>();
        foreach (var log in pageLogs)
        {
            items.Add(await MapToDtoAsync(log));
        }

        return new CursorPagedResult<AuditLogDto>
        {
            Items = items,
            NextCursor = hasMore ? pageLogs[^1].AuditLogId : null,
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
            AuditLogId = auditLog.AuditLogId,
            UserId = auditLog.UserId,
            UserName = auditLog.User?.FullName ?? string.Empty,
            Action = actionKey,
            Details = templateRenderer.Render(auditLog.AuditAction?.Details ?? string.Empty, values),
            CreatedAt = auditLog.CreatedAt
        };
    }
}
