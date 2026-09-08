using System.Text.Json;
using Pdr.Api.Constants;
using Pdr.Api.Services.Templating;

namespace Pdr.Api.Services.Notifications.Mappers;

public class PMAllocatedMapper : INotificationTemplateMapper
{
    public string TemplateKey => "PMAllocated";

    public record PMAllocatedData(int AssignedUserId, int EngagementId);
    public void CollectIds(string data, BatchLoadContext context)
    {
        var payload = JsonSerializer.Deserialize<PMAllocatedData>(data);
        if (payload is null) return;

        context.EngagementIds.Add(payload.EngagementId);
        context.UserIds.Add(payload.AssignedUserId);
    }


    public IDictionary<string, object?> Map(string data, BatchLoadContext context, int? actorUserId = null)
    {
        var payload = JsonSerializer.Deserialize<PMAllocatedData>(data)
            ?? throw new InvalidOperationException(NotificationConstants.Messages.ParsePayloadFailed(TemplateKey));

        var engagement = context.Engagements.GetValueOrDefault(payload.EngagementId);
        var pm = context.Users.GetValueOrDefault(payload.AssignedUserId);

        return new Dictionary<string, object?>
        {
            ["PMName"] = pm?.FullName,
            ["EngagementName"] = engagement?.EngagementName,
            ["EngagementId"] = payload.EngagementId
        };
    }
}
