using Pdr.Api.DTOs.Common;
using Pdr.Api.DTOs.Notifications;
using Pdr.Api.Models.LoginEntity;

namespace Pdr.Api.Services.Interfaces;

public interface INotificationService
{
    Task CreateFromTemplateAsync(
        string templateKey,
        int engagementId,
        RoleEnum receiverRole,
        string data,
        DateTime scheduledFor,
        CancellationToken cancellationToken = default);

    Task CreateFromTemplateAsync(
        string templateKey,
        int receiverUserId,
        string data,
        DateTime scheduledFor,
        CancellationToken cancellationToken = default);

    Task CreateFromTemplateAsync(
        string templateKey,
        IEnumerable<int> receiverUserIds,
        string data,
        DateTime scheduledFor,
        CancellationToken cancellationToken = default);

    Task<PagedResult<NotificationDto>> GetForUserAsync(int userId, int? engagementId, int page, int pageSize);
    Task MarkAsReadAsync(int id, int userId);
    Task MarkAllAsReadAsync(int userId);
    Task<int> GetUnreadCountAsync(int userId);
    Task DeleteAsync(int userId, IEnumerable<int> ids);
}
