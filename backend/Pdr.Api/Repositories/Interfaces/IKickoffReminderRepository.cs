using Pdr.Api.Models.Notifications;

namespace Pdr.Api.Repositories.Interfaces;

public interface IKickoffReminderRepository
{
    Task<List<Notification>> GetKickoffRemindersAsync(int engagementId, int templateId);
}
