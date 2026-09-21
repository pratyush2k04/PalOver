using System.Text.Json;
using Pdr.Api.Constants;
using Pdr.Api.Services.Templating;

namespace Pdr.Api.Services.Notifications.Mappers;

public class TechnicalReadinessRegressedMapper : INotificationTemplateMapper
{
    public string TemplateKey => NotificationConstants.TemplateKeys.TechnicalReadinessRegressed;

    public record TechnicalReadinessRegressedPayload(int EngagementId, string? Reason = null);

    public void CollectIds(string data, BatchLoadContext context)
    {
        var payload = JsonSerializer.Deserialize<TechnicalReadinessRegressedPayload>(data);
        if (payload is null) return;

        context.EngagementIds.Add(payload.EngagementId);
    }

    public IDictionary<string, object?> Map(string data, BatchLoadContext context, int? actorUserId = null)
    {
        var payload = JsonSerializer.Deserialize<TechnicalReadinessRegressedPayload>(data)
            ?? throw new InvalidOperationException(NotificationConstants.Messages.ParsePayloadFailed(NotificationConstants.TemplateKeys.TechnicalReadinessRegressed));

        var engagement = context.Engagements.GetValueOrDefault(payload.EngagementId);

        return new Dictionary<string, object?>
        {
            ["EngagementName"] = engagement?.EngagementName,
            ["EngagementId"] = payload.EngagementId,
            ["Reason"] = string.IsNullOrWhiteSpace(payload.Reason) ? "open technical blockers" : payload.Reason
        };
    }
}
