using System;
using System.Collections.Generic;
using System.Text.Json;
using Pdr.Api.Services.Templating;
using Pdr.Api.Constants;

namespace Pdr.Api.Services.Notifications.Mappers;

public class ResourceRequestConfirmedMapper : INotificationTemplateMapper
{
    public string TemplateKey => "ResourceRequestConfirmed";

    public record ResourceRequestConfirmedData(int ResourceRequestId, int EngagementId);
    public void CollectIds(string data, BatchLoadContext context)
    {
        var payload = JsonSerializer.Deserialize<ResourceRequestConfirmedData>(data);
        if (payload is null) return;

        context.EngagementIds.Add(payload.EngagementId);
        context.ResourceRequestIds.Add(payload.ResourceRequestId);
    }


    public IDictionary<string, object?> Map(string data, BatchLoadContext context, int? actorUserId = null)
    {
        var payload = JsonSerializer.Deserialize<ResourceRequestConfirmedData>(data)
            ?? throw new InvalidOperationException(NotificationConstants.Messages.ParsePayloadFailed(TemplateKey));

        var request = context.ResourceRequests.GetValueOrDefault(payload.ResourceRequestId);
        var engagement = context.Engagements.GetValueOrDefault(payload.EngagementId);

        var roleName = request != null && ResourceRequestConstants.StandardRoleDisplayNames.TryGetValue(request.StandardRole, out var name)
            ? name
            : request?.StandardRole.ToString();

        return new Dictionary<string, object?>
        {
            ["RoleName"] = roleName,
            ["EngagementName"] = engagement?.EngagementName,
            ["EngagementId"] = payload.EngagementId
        };
    }
}
