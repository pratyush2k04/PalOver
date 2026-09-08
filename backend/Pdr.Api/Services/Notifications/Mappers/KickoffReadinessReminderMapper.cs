using System.Text.Json;
using Pdr.Api.Services.Templating;
using Pdr.Api.Constants;

namespace Pdr.Api.Services.Notifications.Mappers;

public class KickoffReadinessReminderMapper : INotificationTemplateMapper
{
    public string TemplateKey => KickoffReminderConstants.TemplateKey;

    public record KickoffReadinessReminderData(int EngagementId, int ThresholdDays);
    public void CollectIds(string data, BatchLoadContext context)
    {
        var payload = JsonSerializer.Deserialize<KickoffReadinessReminderData>(data);
        if (payload is null) return;

        context.EngagementIds.Add(payload.EngagementId);
        context.KickoffReadinessEngagementIds.Add(payload.EngagementId);
    }


    public IDictionary<string, object?> Map(string data, BatchLoadContext context, int? actorUserId = null)
    {
        var payload = JsonSerializer.Deserialize<KickoffReadinessReminderData>(data)
            ?? throw new InvalidOperationException(NotificationConstants.Messages.ParsePayloadFailed(TemplateKey));

        var engagement = context.Engagements.GetValueOrDefault(payload.EngagementId);
        var nextTab = context.KickoffReadinessNextTabs.GetValueOrDefault(payload.EngagementId) ?? "handover";
        
        return new Dictionary<string, object?>
        {
            ["EngagementId"] = payload.EngagementId,
            ["EngagementName"] = engagement?.EngagementName,
            ["DaysBeforeKickoff"] = payload.ThresholdDays,
            ["TabKey"] = nextTab
        };
    }
}
