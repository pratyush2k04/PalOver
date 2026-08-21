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
    public async Task LogAsync(AuditActionEnum action, IDictionary<string, object?> payload, int? engagementId = null, int? actingUserId = null)
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
            EngagementId = engagementId
        });

        await repository.SaveChangesAsync();
    }


    public async Task<PagedResult<AuditLogDto>> GetAuditLogsAsync(int page, int pageSize, List<AuditCategoryEnum>? categories = null)
    {
        var isElevated = currentUserService.IsInRole(RoleEnum.Leadership) || currentUserService.IsAdmin();
        var scopedUserId = isElevated ? (int?)null : currentUserService.GetUserId();

        List<int>? validActionIds = null;
        if (categories != null && categories.Any())
        {
            validActionIds = AuditActionCategoryRegistry.Entries
                .Where(actionToCategory => categories.Contains(actionToCategory.Value))
                .Select(actionToCategory => (int)actionToCategory.Key)
                .ToList();
        }

        var (items, count) = await repository.GetPageAsync(page, pageSize, scopedUserId, validActionIds: validActionIds);

        var dtos = new List<AuditLogDto>();
        foreach (var log in items)
        {
            dtos.Add(await MapToDtoAsync(log));
        }

        return new PagedResult<AuditLogDto>
        {
            Items = dtos,
            TotalCount = count,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<PagedResult<EngagementAuditLogDto>> GetEngagementAuditLogsAsync(int engagementId, int page, int pageSize, int? userId = null, List<AuditCategoryEnum>? categories = null)
    {
        var validActionIds = EngagementAuditActionVisibilityRegistry.Entries
            .Where(actionToVisibility => currentUserService.IsAdmin() || (actionToVisibility.Value.VisibleToRoles?.Any(r => currentUserService.IsInRole(r)) ?? false))
            .Select(actionToVisibility => (int)actionToVisibility.Key)
            .ToList();

        if (categories != null && categories.Any())
        {
            var categoryActionIds = AuditActionCategoryRegistry.Entries
                .Where(actionToCategory => categories.Contains(actionToCategory.Value))
                .Select(actionToCategory => (int)actionToCategory.Key)
                .ToList();

            validActionIds = validActionIds.Intersect(categoryActionIds).ToList();
        }

        var (items, count) = await repository.GetPageAsync(page, pageSize, userId, engagementId, validActionIds);

        var dtos = new List<EngagementAuditLogDto>();

        foreach (var log in items)
        {
            var dto = await MapToDtoAsync(log);
            dtos.Add(new EngagementAuditLogDto
            {
                Id = dto.Id,
                UserId = dto.UserId,
                UserName = dto.UserName,
                Action = dto.Action,
                Details = dto.Details,
                CreatedAt = dto.CreatedAt
            });
        }

        return new PagedResult<EngagementAuditLogDto>
        {
            Items = dtos,
            TotalCount = count,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<List<SimpleUserDto>> GetEngagementAuditUsersAsync(int engagementId)
    {
        return await repository.GetEngagementAuditUsersAsync(engagementId);
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

}
