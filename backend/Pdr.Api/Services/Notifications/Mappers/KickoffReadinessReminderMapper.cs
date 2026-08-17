using System.Text.Json;
using Pdr.Api.Constants;
using Pdr.Api.Repositories.Interfaces;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Services.Notifications.Mappers;

public class KickoffReadinessReminderMapper(
    IEngagementRepository engagementRepository,
    IKickoffReadinessService kickoffReadinessService) : INotificationTemplateMapper
{
    public string TemplateKey => KickoffReminderConstants.TemplateKey;

    // RetiredForBilling defaults to false so existing/newly-scheduled payloads deserialize cleanly;
    // KickoffReminderService flips it to true (by rewriting Data, not IsDeleted alone) when billing is
    // signed off, so a later readiness drop can never resurrect a permanently retired reminder - the
    // Notification.UpdatedBy column can't be used for this because AppDbContext.SaveChangesAsync always
    // overwrites UpdatedBy with the current user on every modified row.
    public record KickoffReadinessReminderData(int EngagementId, int ThresholdDays, bool RetiredForBilling = false);

    public async Task<IDictionary<string, object?>> MapAsync(string data, CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Deserialize<KickoffReadinessReminderData>(data)
            ?? throw new InvalidOperationException("KickoffReadinessReminder notification data could not be parsed.");

        var engagement = await engagementRepository.GetByIdAsync(payload.EngagementId);
        var readiness = await kickoffReadinessService.GetReadinessAsync(payload.EngagementId);

        return new Dictionary<string, object?>
        {
            ["EngagementId"] = payload.EngagementId,
            ["EngagementName"] = engagement?.EngagementName,
            ["DaysBeforeKickoff"] = payload.ThresholdDays,
            ["TabKey"] = readiness.NextIncompleteTabKey ?? "handover"
        };
    }
}
