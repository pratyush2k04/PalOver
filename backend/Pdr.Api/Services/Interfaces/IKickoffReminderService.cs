namespace Pdr.Api.Services.Interfaces;

public interface IKickoffReminderService
{
    Task ScheduleRemindersAsync(int engagementId, DateOnly kickoffDate);
    Task SyncSuppressionAsync(int engagementId, int readinessScore);
}
