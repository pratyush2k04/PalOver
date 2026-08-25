using System.Text.Json;
using Pdr.Api.Constants;
using Pdr.Api.Repositories.Interfaces;
using Pdr.Api.Services.Interfaces;
using Pdr.Api.Services.Notifications.Mappers;

namespace Pdr.Api.Services;

public class KickoffReminderService(
    INotificationService notificationService,
    INotificationRepository notificationRepository,
    IKickoffReminderRepository kickoffReminderRepository,
    IEngagementRepository engagementRepository) : IKickoffReminderService
{
    public async Task ScheduleRemindersAsync(int engagementId, DateOnly kickoffDate)
    {
        var template = await notificationRepository.GetTemplateByKeyAsync(KickoffReminderConstants.TemplateKey);
        if (template is null) return;

        var existingReminders = await kickoffReminderRepository.GetKickoffRemindersAsync(engagementId, template.Id);
        var assignments = await engagementRepository.GetAssignmentsAsync(engagementId);
        var hasChanges = false;

        foreach (var role in KickoffReminderConstants.ReceiverRoles)
        {
            // CreateFromTemplateAsync silently no-ops if this role is unfilled - check up front so we
            // don't attempt to schedule a notification for a role with no assignment.
            var assignment = assignments.FirstOrDefault(a => a.User.RoleId == (int)role);
            if (assignment is null) continue;

            foreach (var (thresholdDays, priority) in KickoffReminderConstants.Thresholds)
            {
                var scheduledFor = DateTime.SpecifyKind(
                    kickoffDate.AddDays(-thresholdDays).ToDateTime(TimeOnly.MinValue),
                    DateTimeKind.Utc);

                var match = existingReminders.FirstOrDefault(n =>
                    n.ReceiverUserId == assignment.UserId && TryParseData(n.Data)?.ThresholdDays == thresholdDays);

                if (match is null)
                {
                    await notificationService.CreateFromTemplateAsync(
                        KickoffReminderConstants.TemplateKey,
                        engagementId,
                        role,
                        JsonSerializer.Serialize(new KickoffReadinessReminderMapper.KickoffReadinessReminderData(engagementId, thresholdDays)),
                        scheduledFor,
                        priority);
                }
                else if (match.ScheduledFor != scheduledFor)
                {
                    match.ScheduledFor = scheduledFor;
                    hasChanges = true;
                }
            }
        }

        if (hasChanges)
        {
            await notificationRepository.SaveChangesAsync();
        }
    }

    public async Task SyncSuppressionAsync(int engagementId, int readinessScore)
    {
        var template = await notificationRepository.GetTemplateByKeyAsync(KickoffReminderConstants.TemplateKey);
        if (template is null) return;

        var reminders = await kickoffReminderRepository.GetKickoffRemindersAsync(engagementId, template.Id);
        if (reminders.Count == 0) return;

        var changed = false;

        if (readinessScore == 100)
        {
            foreach (var reminder in reminders.Where(r => !r.IsDeleted))
            {
                reminder.IsDeleted = true;
                changed = true;
            }
        }
        else
        {
            foreach (var reminder in reminders.Where(r => r.IsDeleted))
            {
                reminder.IsDeleted = false;
                changed = true;
            }
        }

        if (changed)
        {
            await notificationRepository.SaveChangesAsync();
        }
    }

    private static KickoffReadinessReminderMapper.KickoffReadinessReminderData? TryParseData(string? data)
    {
        if (data is null) return null;

        try
        {
            return JsonSerializer.Deserialize<KickoffReadinessReminderMapper.KickoffReadinessReminderData>(data);
        }
        catch (JsonException)
        {
            return null;
        }
    }
}
