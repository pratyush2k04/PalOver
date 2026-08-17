using System.Text.Json;
using Pdr.Api.Constants;
using Pdr.Api.Models.Audit;
using Pdr.Api.Models.LoginEntity;
using Pdr.Api.Repositories.Interfaces;
using Pdr.Api.Services.Interfaces;
using Pdr.Api.Services.Notifications.Mappers;

namespace Pdr.Api.Services;

public class KickoffReminderService(
    INotificationService notificationService,
    INotificationRepository notificationRepository,
    IAuditLogService auditLogService,
    ICurrentUserService currentUserService) : IKickoffReminderService
{
    public async Task ScheduleRemindersAsync(int engagementId, DateOnly kickoffDate, int? actingUserId = null)
    {
        var template = await notificationRepository.GetTemplateByKeyAsync(KickoffReminderConstants.TemplateKey);
        if (template is null) return;

        var existingReminders = await notificationRepository.GetKickoffRemindersAsync(engagementId, template.Id);
        var actorId = ResolveActingUserId(actingUserId);

        foreach (var (thresholdDays, priority) in KickoffReminderConstants.Thresholds)
        {
            var scheduledFor = DateTime.SpecifyKind(
                kickoffDate.AddDays(-thresholdDays).ToDateTime(TimeOnly.MinValue),
                DateTimeKind.Utc);

            var match = existingReminders.FirstOrDefault(n => TryGetThresholdDays(n.Data) == thresholdDays);

            if (match is null)
            {
                await notificationService.CreateFromTemplateAsync(
                    KickoffReminderConstants.TemplateKey,
                    engagementId,
                    RoleEnum.ProjectManagement,
                    JsonSerializer.Serialize(new KickoffReadinessReminderMapper.KickoffReadinessReminderData(engagementId, thresholdDays)),
                    scheduledFor,
                    priority);

                await LogScheduledAsync(engagementId, thresholdDays, actorId);
            }
            else if (match.ScheduledFor != scheduledFor)
            {
                match.ScheduledFor = scheduledFor;
                await notificationRepository.SaveChangesAsync();

                await LogScheduledAsync(engagementId, thresholdDays, actorId);
            }
        }
    }

    public async Task SyncSuppressionAsync(int engagementId, int readinessScore, int? actingUserId = null)
    {
        var template = await notificationRepository.GetTemplateByKeyAsync(KickoffReminderConstants.TemplateKey);
        if (template is null) return;

        var reminders = await notificationRepository.GetKickoffRemindersAsync(engagementId, template.Id);
        if (reminders.Count == 0) return;

        var suppressedAny = false;
        var reactivatedAny = false;

        if (readinessScore == 100)
        {
            foreach (var reminder in reminders.Where(r => !r.IsDeleted))
            {
                reminder.IsDeleted = true;
                reminder.UpdatedBy = KickoffReminderConstants.ReadinessSyncActor;
                suppressedAny = true;
            }
        }
        else
        {
            foreach (var reminder in reminders.Where(r => r.IsDeleted && r.UpdatedBy != KickoffReminderConstants.BillingSignOffActor))
            {
                reminder.IsDeleted = false;
                reminder.UpdatedBy = KickoffReminderConstants.ReadinessSyncActor;
                reactivatedAny = true;
            }
        }

        if (!suppressedAny && !reactivatedAny) return;

        await notificationRepository.SaveChangesAsync();

        var actorId = ResolveActingUserId(actingUserId);

        if (suppressedAny)
        {
            await LogAsync(
                AuditActionEnum.KickoffReminderSuppressed,
                new Dictionary<string, object?> { ["EngagementId"] = engagementId },
                actorId);
        }

        if (reactivatedAny)
        {
            await LogAsync(
                AuditActionEnum.KickoffReminderReactivated,
                new Dictionary<string, object?> { ["EngagementId"] = engagementId, ["ReadinessScore"] = readinessScore },
                actorId);
        }
    }

    public async Task RetireForBillingAsync(int engagementId, int? actingUserId = null)
    {
        var template = await notificationRepository.GetTemplateByKeyAsync(KickoffReminderConstants.TemplateKey);
        if (template is null) return;

        var reminders = await notificationRepository.GetKickoffRemindersAsync(engagementId, template.Id);
        var active = reminders.Where(r => !r.IsDeleted).ToList();
        if (active.Count == 0) return;

        foreach (var reminder in active)
        {
            reminder.IsDeleted = true;
            reminder.UpdatedBy = KickoffReminderConstants.BillingSignOffActor;
        }

        await notificationRepository.SaveChangesAsync();

        await LogAsync(
            AuditActionEnum.KickoffReminderRetiredForBilling,
            new Dictionary<string, object?> { ["EngagementId"] = engagementId },
            ResolveActingUserId(actingUserId));
    }

    private async Task LogScheduledAsync(int engagementId, int thresholdDays, int? actorId) =>
        await LogAsync(
            AuditActionEnum.KickoffReminderScheduled,
            new Dictionary<string, object?> { ["EngagementId"] = engagementId, ["ThresholdDays"] = thresholdDays },
            actorId);

    private async Task LogAsync(AuditActionEnum action, IDictionary<string, object?> payload, int? actorId)
    {
        if (actorId is null) return;

        await auditLogService.LogAsync(action, payload, actorId);
    }

    private int? ResolveActingUserId(int? actingUserId)
    {
        if (actingUserId is not null) return actingUserId;

        try
        {
            return currentUserService.GetUserId();
        }
        catch (InvalidOperationException)
        {
            return null;
        }
    }

    private static int? TryGetThresholdDays(string? data)
    {
        if (data is null) return null;

        try
        {
            var payload = JsonSerializer.Deserialize<KickoffReadinessReminderMapper.KickoffReadinessReminderData>(data);
            return payload?.ThresholdDays;
        }
        catch (JsonException)
        {
            return null;
        }
    }
}
