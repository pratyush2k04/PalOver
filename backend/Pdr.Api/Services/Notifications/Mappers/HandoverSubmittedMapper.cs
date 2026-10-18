using System.Text.Json;
using Pdr.Api.Constants;
using Pdr.Api.Services.Templating;

namespace Pdr.Api.Services.Notifications.Mappers;

public class HandoverSubmittedMapper : INotificationTemplateMapper
{
    public string TemplateKey => "HandoverSubmitted";

    public record HandoverSubmittedData(int HandoverId, int TriggerUserId, int EngagementId);
    public void CollectIds(string data, BatchLoadContext context)
    {
        var payload = JsonSerializer.Deserialize<HandoverSubmittedData>(data);
        if (payload is null) return;

        context.EngagementIds.Add(payload.EngagementId);
        context.UserIds.Add(payload.TriggerUserId);
    }


    public IDictionary<string, object?> Map(string data, BatchLoadContext context, int? actorUserId = null)
    {
        var payload = JsonSerializer.Deserialize<HandoverSubmittedData>(data)
            ?? throw new InvalidOperationException(NotificationConstants.Messages.ParsePayloadFailed(TemplateKey));

        var engagement = context.Engagements.GetValueOrDefault(payload.EngagementId);
        var triggerUser = context.Users.GetValueOrDefault(payload.TriggerUserId);

        return new Dictionary<string, object?>
        {
            ["EngagementName"] = engagement?.EngagementName,
            ["TriggerUserName"] = triggerUser?.FullName,
            ["EngagementId"] = payload.EngagementId
        };
    }
}
