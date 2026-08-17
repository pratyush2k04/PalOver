namespace Pdr.Api.Services.Interfaces;

public interface IKickoffReminderService
{
    Task ScheduleRemindersAsync(int engagementId, DateOnly kickoffDate, int? actingUserId = null);
    Task SyncSuppressionAsync(int engagementId, int readinessScore, int? actingUserId = null);
    Task RetireForBillingAsync(int engagementId, int? actingUserId = null);
}
