using System.Text.Json;
using Pdr.Api.Services.Templating;

namespace Pdr.Api.Services.Notifications.Mappers;

public class HandoverQueryRaisedMapper : INotificationTemplateMapper
{
    public string TemplateKey => "HandoverQueryRaised";

    public record HandoverQueryRaisedData(int QueryId, int HandoverId, int TriggerUserId, int EngagementId);
    public void CollectIds(string data, BatchLoadContext context)
    {
        var payload = JsonSerializer.Deserialize<HandoverQueryRaisedData>(data);
        if (payload is null) return;

        context.EngagementIds.Add(payload.EngagementId);
        context.UserIds.Add(payload.TriggerUserId);
        context.HandoverQueryIds.Add(payload.QueryId);
    }


    public IDictionary<string, object?> Map(string data, BatchLoadContext context, int? actorUserId = null)
    {
        var payload = JsonSerializer.Deserialize<HandoverQueryRaisedData>(data)
            ?? throw new InvalidOperationException("HandoverQueryRaised notification data could not be parsed.");

        var engagement = context.Engagements.GetValueOrDefault(payload.EngagementId);
        var triggerUser = context.Users.GetValueOrDefault(payload.TriggerUserId);
        var query = context.HandoverQueries.GetValueOrDefault(payload.QueryId);

        return new Dictionary<string, object?>
        {
            ["EngagementName"] = engagement?.EngagementName,
            ["TriggerUserName"] = triggerUser?.FullName,
            ["FieldSection"] = query?.FieldSection.Name,
            ["EngagementId"] = payload.EngagementId
        };
    }
}
