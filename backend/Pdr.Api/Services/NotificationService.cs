using Microsoft.Extensions.Logging;
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
    TemplateRenderer renderer,
    ILogger<NotificationService> logger) : INotificationService
{
    public async Task CreateFromTemplateAsync(
        string templateKey,
        int engagementId,
        RoleEnum receiverRole,
        string data,
        DateTime scheduledFor,
        string? link = null,
        CancellationToken cancellationToken = default)
    {
        var assignment = await engagementRepository.GetAssignmentByRoleIdAsync(engagementId, (int)receiverRole);
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
            Priority = template.DefaultPriority,
            ScheduledFor = scheduledFor,
            Data = data,
            Link = link
        });
    }

    public async Task CreateFromTemplateAsync(
        string templateKey,
        int receiverUserId,
        string data,
        DateTime scheduledFor,
        CancellationToken cancellationToken = default)
    {
        var template = await repository.GetTemplateByKeyAsync(templateKey)
            ?? throw new NotificationTemplateNotFoundException(templateKey);

        await repository.AddAsync(new Notification
        {
            TemplateId = template.Id,
            ReceiverUserId = receiverUserId,
            Priority = template.DefaultPriority,
            ScheduledFor = scheduledFor,
            Data = data
        });
    }

    public async Task<PagedResult<NotificationDto>> GetForUserAsync(int userId, int? engagementId, int page, int pageSize)
    {
        var (notifications, totalCount) = await repository.GetForUserAsync(userId, engagementId, page, pageSize);

        var items = new List<NotificationDto>();
        foreach (var notification in notifications)
        {
            var (title, body) = await ResolveContentAsync(notification);
            items.Add(new NotificationDto(
                notification.Id,
                title,
                body,
                notification.Priority,
                ComputeStatus(notification),
                notification.ScheduledFor,
                notification.ReadAt,
                notification.Link));
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

    private async Task<(string Title, string Body)> ResolveContentAsync(Notification notification)
    {
        if (notification.TemplateId is null || notification.Template is null)
        {
            return (notification.Title ?? string.Empty, notification.Body ?? string.Empty);
        }

        var mapper = mapperResolver.Resolve(notification.Template.Key);
        if (mapper is null || notification.Data is null)
        {
            return (notification.Template.Title, notification.Template.Body);
        }

        var values = await mapper.MapAsync(notification.Data);
        return (
            renderer.Render(notification.Template.Title, values),
            renderer.Render(notification.Template.Body, values));
    }

    private static string ComputeStatus(Notification notification)
    {
        if (notification.ReadAt is not null) return "Read";
        return notification.ScheduledFor <= DateTime.UtcNow ? "Sent" : "Pending";
    }

}
