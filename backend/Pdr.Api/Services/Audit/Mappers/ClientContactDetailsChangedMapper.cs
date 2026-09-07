using System.Text.Json;
using Pdr.Api.Services.Templating;
using Pdr.Api.Models.Audit;

namespace Pdr.Api.Services.Audit.Mappers;

public class ClientContactDetailsChangedMapper : IAuditTemplateMapper
{
    public string TemplateKey => nameof(AuditActionEnum.ClientContactDetailsChanged);

    private record ClientContactDetailsData(int? EngagementId, string? ClientName, int? ActorUserId);
    public void CollectIds(string data, BatchLoadContext context)
    {
        var payload = JsonSerializer.Deserialize<ClientContactDetailsData>(data);
        if (payload is null) return;

        if (payload.EngagementId.HasValue) context.EngagementIds.Add(payload.EngagementId.Value);
        if (payload.ActorUserId.HasValue) context.UserIds.Add(payload.ActorUserId.Value);
    }


    public IDictionary<string, object?> Map(string data, BatchLoadContext context, int? actorUserId = null)
    {
        var payload = JsonSerializer.Deserialize<ClientContactDetailsData>(data)
            ?? throw new InvalidOperationException("ClientContactDetailsChanged audit payload could not be parsed.");

        var actor = payload.ActorUserId.HasValue
            ? context.Users.GetValueOrDefault(payload.ActorUserId.Value) ?? (actorUserId.HasValue ? context.Users.GetValueOrDefault(actorUserId.Value) : null)
            : null;

        return new Dictionary<string, object?>
        {
            ["clientName"] = payload.ClientName,
            ["actorName"] = actor?.FullName
        };
    }
}
