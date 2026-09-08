using System.Text.Json;
using Pdr.Api.Services.Templating;
using Pdr.Api.Constants;

namespace Pdr.Api.Services.Notifications.Mappers;

public class DependencyAssignedToOwnerMapper : INotificationTemplateMapper
{
    public string TemplateKey => NotificationConstants.TemplateKeys.DependencyAssignedToOwner;

    public record DependencyAssignedPayload(int DependencyId, int EngagementId);
    public void CollectIds(string data, BatchLoadContext context)
    {
        var payload = JsonSerializer.Deserialize<DependencyAssignedPayload>(data);
        if (payload is null) return;

        context.EngagementIds.Add(payload.EngagementId);
        context.DependencyIds.Add(payload.DependencyId);
    }


    public IDictionary<string, object?> Map(string data, BatchLoadContext context, int? actorUserId = null)
    {
        var payload = JsonSerializer.Deserialize<DependencyAssignedPayload>(data)
            ?? throw new InvalidOperationException(NotificationConstants.Messages.ParsePayloadFailed(NotificationConstants.TemplateKeys.DependencyAssignedToOwner));

        var engagement = context.Engagements.GetValueOrDefault(payload.EngagementId);
        var dependency = context.Dependencies.GetValueOrDefault(payload.DependencyId);

        return new Dictionary<string, object?>
        {
            ["DependencyDescription"] = dependency?.Description,
            ["EngagementName"] = engagement?.EngagementName,
            ["EngagementId"] = payload.EngagementId
        };
    }
}
