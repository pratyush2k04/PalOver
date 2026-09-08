using System.Text.Json;
using Pdr.Api.Constants;
using Pdr.Api.Services.Templating;

namespace Pdr.Api.Services.Notifications.Mappers;

public class AccessItemBlockedMapper : INotificationTemplateMapper
{
    public string TemplateKey => "AccessItemBlocked";

    public record AccessItemBlockedData(int AccessItemId, int TriggerUserId);
    public void CollectIds(string data, BatchLoadContext context)
    {
        var payload = JsonSerializer.Deserialize<AccessItemBlockedData>(data);
        if (payload is null) return;

        context.UserIds.Add(payload.TriggerUserId);
        context.AccessItemIds.Add(payload.AccessItemId);
    }


    public IDictionary<string, object?> Map(string data, BatchLoadContext context, int? actorUserId = null)
    {
        var payload = JsonSerializer.Deserialize<AccessItemBlockedData>(data)
            ?? throw new InvalidOperationException(NotificationConstants.Messages.ParsePayloadFailed(TemplateKey));

        var accessItem = context.AccessItems.GetValueOrDefault(payload.AccessItemId);
        var triggerUser = context.Users.GetValueOrDefault(payload.TriggerUserId);

        return new Dictionary<string, object?>
        {
            ["ItemName"] = accessItem?.Name,
            ["TriggerUserName"] = triggerUser?.FullName,
            ["BlockReason"] = accessItem?.BlockReason,
            ["EngagementId"] = accessItem?.EngagementId
        };
    }
}
