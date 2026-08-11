using Pdr.Api.Models.Notifications;

namespace Pdr.Api.Repositories.Interfaces;

public interface INotificationRepository
{
    Task<Notification?> GetByIdAsync(int id);
    Task<(List<Notification> Notifications, int TotalCount)> GetForUserAsync(int userId, int? engagementId, int page, int pageSize);
    Task AddAsync(Notification notification);
    Task SetReadAtAsync(int id, DateTime readAt);
    Task SetAllReadAsync(int userId, DateTime readAt);
    Task<int> GetUnreadCountAsync(int userId);
    Task BulkDeleteAsync(IEnumerable<int> ids);
}
