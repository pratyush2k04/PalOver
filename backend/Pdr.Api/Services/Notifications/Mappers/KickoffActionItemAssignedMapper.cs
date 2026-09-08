using System.Text.Json;
using Pdr.Api.Services.Templating;
using Pdr.Api.Constants;

namespace Pdr.Api.Services.Notifications.Mappers;

public class KickoffActionItemAssignedMapper : INotificationTemplateMapper
{
    public string TemplateKey => NotificationConstants.TemplateKeys.KickoffActionItemAssigned;

    public record KickoffActionItemAssignedPayload(int ActionItemId, int EngagementId);
    public void CollectIds(string data, BatchLoadContext context)
    {
        var payload = JsonSerializer.Deserialize<KickoffActionItemAssignedPayload>(data);
        if (payload is null) return;

        context.EngagementIds.Add(payload.EngagementId);
        context.KickoffActionItemIds.Add(payload.ActionItemId);
    }


    public IDictionary<string, object?> Map(string data, BatchLoadContext context, int? actorUserId = null)
    {
        var payload = JsonSerializer.Deserialize<KickoffActionItemAssignedPayload>(data)
            ?? throw new InvalidOperationException(NotificationConstants.Messages.ParsePayloadFailed(NotificationConstants.TemplateKeys.KickoffActionItemAssigned));

        var actionItem = context.KickoffActionItems.GetValueOrDefault(payload.ActionItemId);
        var engagement = context.Engagements.GetValueOrDefault(payload.EngagementId);

        return new Dictionary<string, object?>
        {
            ["ActionTitle"] = actionItem?.Title,
            ["EngagementName"] = engagement?.EngagementName,
            ["EngagementId"] = payload.EngagementId
        };
    }
}
