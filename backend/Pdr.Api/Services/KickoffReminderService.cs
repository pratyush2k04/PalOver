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
    IEngagementRepository engagementRepository,
    IAuditLogService auditLogService,
    ICurrentUserService currentUserService) : IKickoffReminderService
{
    public async Task ScheduleRemindersAsync(int engagementId, DateOnly kickoffDate, int? actingUserId = null)
    {
        var template = await notificationRepository.GetTemplateByKeyAsync(KickoffReminderConstants.TemplateKey);
        if (template is null) return;

        // CreateFromTemplateAsync silently no-ops if the PM role is unfilled - check up front so we
        // don't write a KickoffReminderScheduled audit entry for a notification that was never created.
        var pmAssignment = await engagementRepository.GetAssignmentByRoleIdAsync(engagementId, (int)RoleEnum.ProjectManagement);
        if (pmAssignment is null) return;

        var existingReminders = await notificationRepository.GetKickoffRemindersAsync(engagementId, template.Id);
        var actorId = ResolveActingUserId(actingUserId);

        foreach (var (thresholdDays, priority) in KickoffReminderConstants.Thresholds)
        {
            var scheduledFor = DateTime.SpecifyKind(
                kickoffDate.AddDays(-thresholdDays).ToDateTime(TimeOnly.MinValue),
                DateTimeKind.Utc);

            var match = existingReminders.FirstOrDefault(n => TryParseData(n.Data)?.ThresholdDays == thresholdDays);

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
                suppressedAny = true;
            }
        }
        else
        {
            foreach (var reminder in reminders.Where(r => r.IsDeleted))
            {
                reminder.IsDeleted = false;
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
