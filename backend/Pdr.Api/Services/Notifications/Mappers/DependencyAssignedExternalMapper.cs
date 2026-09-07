using System.Text.Json;
using Pdr.Api.Services.Templating;
using Pdr.Api.Constants;

namespace Pdr.Api.Services.Notifications.Mappers;

public class DependencyAssignedExternalMapper : INotificationTemplateMapper
{
    public string TemplateKey => NotificationConstants.TemplateKeys.DependencyAssignedExternal;

    public record DependencyAssignedExternalPayload(int DependencyId, int EngagementId);
    public void CollectIds(string data, BatchLoadContext context)
    {
        var payload = JsonSerializer.Deserialize<DependencyAssignedExternalPayload>(data);
        if (payload is null) return;

        context.EngagementIds.Add(payload.EngagementId);
        context.DependencyIds.Add(payload.DependencyId);
    }


    public IDictionary<string, object?> Map(string data, BatchLoadContext context, int? actorUserId = null)
    {
        var payload = JsonSerializer.Deserialize<DependencyAssignedExternalPayload>(data)
            ?? throw new InvalidOperationException(NotificationConstants.Messages.ParsePayloadFailed(NotificationConstants.TemplateKeys.DependencyAssignedExternal));

        var engagement = context.Engagements.GetValueOrDefault(payload.EngagementId);
        var dependency = context.Dependencies.GetValueOrDefault(payload.DependencyId);

        return new Dictionary<string, object?>
        {
            ["DependencyDescription"] = dependency?.Description,
            ["OwnerName"] = dependency?.Owner?.Name,
            ["EngagementName"] = engagement?.EngagementName,
            ["EngagementId"] = payload.EngagementId
        };
    }
}
