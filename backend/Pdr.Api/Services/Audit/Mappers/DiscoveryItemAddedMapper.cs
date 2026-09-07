using System.Text.Json;
using Pdr.Api.Services.Templating;
using Pdr.Api.Models.Audit;

namespace Pdr.Api.Services.Audit.Mappers;

public class DiscoveryItemAddedMapper : IAuditTemplateMapper
{
    public string TemplateKey => nameof(AuditActionEnum.DiscoveryItemAdded);

    private record DiscoveryItemAddedData(int EngagementId, string? ItemName, int ActorUserId);
    public void CollectIds(string data, BatchLoadContext context)
    {
        var payload = JsonSerializer.Deserialize<DiscoveryItemAddedData>(data);
        if (payload is null) return;

        context.EngagementIds.Add(payload.EngagementId);
        context.UserIds.Add(payload.ActorUserId);
    }


    public IDictionary<string, object?> Map(string data, BatchLoadContext context, int? actorUserId = null)
    {
        var payload = JsonSerializer.Deserialize<DiscoveryItemAddedData>(data)
            ?? throw new InvalidOperationException("DiscoveryItemAdded audit payload could not be parsed.");

        var engagement = context.Engagements.GetValueOrDefault(payload.EngagementId);
        var actor = context.Users.GetValueOrDefault(payload.ActorUserId) ?? (actorUserId.HasValue ? context.Users.GetValueOrDefault(actorUserId.Value) : null);

        return new Dictionary<string, object?>
        {
            ["itemName"] = payload.ItemName,
            ["engagementName"] = engagement?.EngagementName,
            ["actorName"] = actor?.FullName
        };
    }
}
