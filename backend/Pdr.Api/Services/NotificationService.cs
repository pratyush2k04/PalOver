using Microsoft.Extensions.Logging;
using System.Linq;
using Pdr.Api.DTOs.Common;
using Pdr.Api.DTOs.Notifications;
using Pdr.Api.Exceptions;
using Pdr.Api.Models.LoginEntity;
using Pdr.Api.Models.Notifications;
using Pdr.Api.Repositories.Interfaces;
using Pdr.Api.Services.Interfaces;
using Pdr.Api.Services.Notifications;
using Pdr.Api.Services.Templating;

namespace Pdr.Api.Services;

public class NotificationService(
    INotificationRepository repository,
    IEngagementRepository engagementRepository,
    TemplateMapperResolver<INotificationTemplateMapper> mapperResolver,
    IBatchDataPreloader batchPreloader,
    TemplateRenderer renderer,
    ILogger<NotificationService> logger) : INotificationService
{
    public async Task CreateFromTemplateAsync(
        string templateKey,
        int engagementId,
        EngagementRoleEnum receiverRole,
        string data,
        DateTime scheduledFor,
        NotificationPriority? priorityOverride = null,
        string? staticFallback = null,
        CancellationToken cancellationToken = default)
    {
        var targetEngagementRoleId = (int)receiverRole;

        var assignment = await engagementRepository.GetAssignmentByEngagementRoleIdAsync(engagementId, targetEngagementRoleId);
        if (assignment is null)
        {
            logger.LogInformation(
                "Skipped creating notification {TemplateKey} for engagement {EngagementId}: role {ReceiverRole} is unfilled.",
                templateKey, engagementId, receiverRole);
            return;
        }

        var template = await repository.GetTemplateByKeyAsync(templateKey);
        if (template is null)
        {
            logger.LogInformation(
                "Skipped creating notification: no NotificationTemplate registered for key {TemplateKey}.",
                templateKey);
            return;
        }

        await repository.AddAsync(new Notification
        {
            TemplateId = template.Id,
            ReceiverUserId = assignment.UserId,
            Priority = priorityOverride ?? template.DefaultPriority,
            ScheduledFor = scheduledFor,
            Data = data,
            StaticFallback = staticFallback
        });
    }

    public async Task CreateFromTemplateAsync(
        string templateKey,
        int receiverUserId,
        string data,
        DateTime scheduledFor,
        NotificationPriority? priorityOverride = null,
        string? staticFallback = null,
        CancellationToken cancellationToken = default)
    {
        var template = await repository.GetTemplateByKeyAsync(templateKey)
            ?? throw new NotificationTemplateNotFoundException(templateKey);

        await repository.AddAsync(new Notification
        {
            TemplateId = template.Id,
            ReceiverUserId = receiverUserId,
            Priority = priorityOverride ?? template.DefaultPriority,
            ScheduledFor = scheduledFor,
            Data = data,
            StaticFallback = staticFallback
        });
    }

    public async Task CreateFromTemplateAsync(
        string templateKey,
        IEnumerable<int> receiverUserIds,
        string data,
        DateTime scheduledFor,
        string? staticFallback = null,
        CancellationToken cancellationToken = default)
    {
        var template = await repository.GetTemplateByKeyAsync(templateKey)
            ?? throw new NotificationTemplateNotFoundException(templateKey);

        var notifications = receiverUserIds.Select(receiverUserId => new Notification
        {
            TemplateId = template.Id,
            ReceiverUserId = receiverUserId,
            Priority = template.DefaultPriority,
            ScheduledFor = scheduledFor,
            Data = data,
            StaticFallback = staticFallback
        });

        await repository.AddRangeAsync(notifications);
    }

    public async Task<PagedResult<NotificationDto>> GetForUserAsync(
        int userId, int? engagementId, int page, int pageSize,
        bool? isRead = null, NotificationPriority? priority = null)
    {
        var (notifications, totalCount) = await repository.GetForUserAsync(userId, engagementId, page, pageSize, isRead, priority);

        // Phase 1: Collect IDs across all notifications in page
        var context = new BatchLoadContext();
        foreach (var notification in notifications)
        {
            if (notification.Template?.Key is not null && notification.Data is not null)
            {
                var mapper = mapperResolver.Resolve(notification.Template.Key);
                mapper?.CollectIds(notification.Data, context);
            }
        }

        // Phase 2: Bulk preload referenced entities in batch queries
        await batchPreloader.PreloadAsync(context);

        // Phase 3: Synchronous in-memory mapping and template rendering
        var items = new List<NotificationDto>(notifications.Count);
        foreach (var notification in notifications)
        {
            var (title, body, link) = ResolveContent(notification, context);
            items.Add(new NotificationDto(
                notification.Id,
                title,
                body,
                notification.Priority,
                ComputeStatus(notification),
                notification.ScheduledFor,
                notification.ReadAt,
                link));
        }

        return new PagedResult<NotificationDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task MarkAsReadAsync(int id, int userId)
    {
        var notification = await repository.GetByIdAsync(id)
            ?? throw new NotificationNotFoundException(id);

        if (notification.ReceiverUserId != userId)
            throw new NotificationAccessDeniedException(id);

        if (notification.ScheduledFor > DateTime.UtcNow)
            throw new NotificationNotYetScheduledException(id);

        await repository.SetReadAtAsync(id, DateTime.UtcNow);
    }

    public async Task MarkAllAsReadAsync(int userId) =>
        await repository.SetAllReadAsync(userId, DateTime.UtcNow);

    public async Task<int> GetUnreadCountAsync(int userId) =>
        await repository.GetUnreadCountAsync(userId);

    public async Task DeleteAsync(int adminUserId, IEnumerable<int> ids)
    {
        var idList = ids.ToList();
        await repository.BulkDeleteAsync(idList);

        logger.LogInformation(
            "Admin {AdminUserId} deleted notifications {NotificationIds}",
            adminUserId, idList);
    }

    private (string Title, string Body, string? Link) ResolveContent(Notification notification, BatchLoadContext context)
    {
        if (notification.TemplateId is null || notification.Template is null)
        {
            return (notification.Title ?? string.Empty, notification.Body ?? string.Empty, notification.Link);
        }

        var mapper = mapperResolver.Resolve(notification.Template.Key);
        if (mapper is null || notification.Data is null)
        {
            return (notification.Template.Title, notification.Template.Body, notification.Link);
        }

        var values = mapper.Map(notification.Data, context);
        var initialValues = TemplateRenderer.ExtractInitialValues(notification.Data);
        var staticFallback = notification.StaticFallback;

        var renderedLink = notification.Template.LinkPattern is null
            ? null
            : renderer.Render(notification.Template.LinkPattern, values, initialValues, staticFallback: null);

        if (renderedLink is not null && renderedLink.Contains(TemplateRenderer.DeletedPlaceholder))
        {
            renderedLink = null;
        }

        var link = notification.Link ?? renderedLink;

        return (
            renderer.Render(notification.Template.Title, values, initialValues, staticFallback),
            renderer.Render(notification.Template.Body, values, initialValues, staticFallback),
            link);
    }

    private static string ComputeStatus(Notification notification)
    {
        if (notification.ReadAt is not null) return "Read";
        return notification.ScheduledFor <= DateTime.UtcNow ? "Sent" : "Pending";
    }

}
