using System.Text.Json;
using Pdr.Api.Services.Templating;
using Pdr.Api.Models.Audit;

namespace Pdr.Api.Services.Audit.Mappers;

public class AccessItemBlockedAuditMapper : IAuditTemplateMapper
{
    public string TemplateKey => nameof(AuditActionEnum.AccessItemBlocked);

    private record AccessItemBlockedData(int AccessItemId, int ActorUserId);
    public void CollectIds(string data, BatchLoadContext context)
    {
        var payload = JsonSerializer.Deserialize<AccessItemBlockedData>(data);
        if (payload is null) return;

        context.UserIds.Add(payload.ActorUserId);
        context.AccessItemIds.Add(payload.AccessItemId);
    }


    public IDictionary<string, object?> Map(string data, BatchLoadContext context, int? actorUserId = null)
    {
        var payload = JsonSerializer.Deserialize<AccessItemBlockedData>(data)
            ?? throw new InvalidOperationException("AccessItemBlocked audit payload could not be parsed.");

        var item = context.AccessItems.GetValueOrDefault(payload.AccessItemId);
        var engagement = item is null ? null : context.Engagements.GetValueOrDefault(item.EngagementId);
        var actor = context.Users.GetValueOrDefault(payload.ActorUserId) ?? (actorUserId.HasValue ? context.Users.GetValueOrDefault(actorUserId.Value) : null);

        return new Dictionary<string, object?>
        {
            ["itemName"] = item?.Name,
            ["engagementName"] = engagement?.EngagementName,
            ["actorName"] = actor?.FullName
        };
    }
}
