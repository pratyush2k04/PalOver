using Pdr.Api.Models.Notifications;
using Pdr.Api.DTOs.Notifications;

namespace Pdr.Api.Repositories.Interfaces;

public interface INotificationRepository
{
    Task<Notification?> GetByIdAsync(int id);
    Task<(List<Notification> Notifications, int TotalCount)> GetForUserAsync(int userId, int? engagementId, int page, int pageSize);
    Task AddAsync(Notification notification);
    Task AddRangeAsync(IEnumerable<Notification> notifications);
    Task SetReadAtAsync(int id, DateTime readAt);
    Task SetAllReadAsync(int userId, DateTime readAt);
    Task<int> GetUnreadCountAsync(int userId);
    Task BulkDeleteAsync(IEnumerable<int> ids);
    Task<NotificationTemplate?> GetTemplateByKeyAsync(string templateKey);
    Task SaveChangesAsync();

    // Raw int priority, not NotificationPriority — priority 4 has no enum member yet.
    // TODO: accept NotificationPriority once the priority-4 escalation-notifications feature lands.
    Task<List<NotificationLiteDto>> GetByPriorityAsync(int priority);
}
