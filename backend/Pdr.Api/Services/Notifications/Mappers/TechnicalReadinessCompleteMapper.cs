using System.Text.Json;
using Pdr.Api.Services.Templating;
using Pdr.Api.Constants;

namespace Pdr.Api.Services.Notifications.Mappers;

public class TechnicalReadinessCompleteMapper : INotificationTemplateMapper
{
    public string TemplateKey => NotificationConstants.TemplateKeys.TechnicalReadinessComplete;

    public record TechnicalReadinessCompletePayload(int EngagementId);
    public void CollectIds(string data, BatchLoadContext context)
    {
        var payload = JsonSerializer.Deserialize<TechnicalReadinessCompletePayload>(data);
        if (payload is null) return;

        context.EngagementIds.Add(payload.EngagementId);
    }


    public IDictionary<string, object?> Map(string data, BatchLoadContext context, int? actorUserId = null)
    {
        var payload = JsonSerializer.Deserialize<TechnicalReadinessCompletePayload>(data)
            ?? throw new InvalidOperationException(NotificationConstants.Messages.ParsePayloadFailed(NotificationConstants.TemplateKeys.TechnicalReadinessComplete));

        var engagement = context.Engagements.GetValueOrDefault(payload.EngagementId);

        return new Dictionary<string, object?>
        {
            ["EngagementName"] = engagement?.EngagementName,
            ["EngagementId"] = payload.EngagementId
        };
    }
}
