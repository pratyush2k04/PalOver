using Microsoft.EntityFrameworkCore;
using Pdr.Api.Data;
using Pdr.Api.Models.Notifications;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Repositories;

public class NotificationRepository(AppDbContext context) : INotificationRepository
{
    public async Task<Notification?> GetByIdAsync(int id) =>
        await context.Notifications
            .Include(n => n.Template)
            .FirstOrDefaultAsync(n => n.Id == id);

    public async Task<(List<Notification> Notifications, int TotalCount)> GetForUserAsync(int userId, int? engagementId, int page, int pageSize)
    {
        var query = context.Notifications
            .AsNoTracking()
            .Include(n => n.Template)
            .Where(n => n.ReceiverUserId == userId);

        if (engagementId.HasValue)
        {
            query = query.Where(n => EF.Functions.JsonContains(n.Data!, $"{{\"EngagementId\":{engagementId.Value}}}"));
        }

        var totalCount = await query.CountAsync();

        var notifications = await query
            .OrderByDescending(n => n.ScheduledFor)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (notifications, totalCount);
    }

    public async Task AddAsync(Notification notification)
    {
        context.Notifications.Add(notification);
        await context.SaveChangesAsync();
    }

    public async Task SetReadAtAsync(int id, DateTime readAt)
    {
        var notification = await context.Notifications.FirstOrDefaultAsync(n => n.Id == id);
        if (notification is null) return;

        notification.ReadAt = readAt;
        await context.SaveChangesAsync();
    }

    public async Task SetAllReadAsync(int userId, DateTime readAt)
    {
        await context.Notifications
            .Where(n => n.ReceiverUserId == userId && n.ReadAt == null)
            .ExecuteUpdateAsync(setters => setters.SetProperty(n => n.ReadAt, readAt));
    }

    public async Task<int> GetUnreadCountAsync(int userId) =>
        await context.Notifications
            .AsNoTracking()
            .CountAsync(n => n.ReceiverUserId == userId && n.ReadAt == null && n.ScheduledFor <= DateTime.UtcNow);

    public async Task BulkDeleteAsync(int userId, IEnumerable<int> ids)
    {
        await context.Notifications
            .Where(n => n.ReceiverUserId == userId && ids.Contains(n.Id))
            .ExecuteDeleteAsync();
    }
}
