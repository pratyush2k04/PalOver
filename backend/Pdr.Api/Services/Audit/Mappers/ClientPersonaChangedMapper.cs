using System.Text.Json;
using Pdr.Api.Constants;
using Pdr.Api.Services.Templating;
using Pdr.Api.Models.Audit;

namespace Pdr.Api.Services.Audit.Mappers;

public class ClientPersonaChangedMapper : IAuditTemplateMapper
{
    public string TemplateKey => nameof(AuditActionEnum.ClientPersonaChanged);

    private record ClientPersonaData(int? EngagementId, string? ClientName, string? Persona, int? ActorUserId);
    public void CollectIds(string data, BatchLoadContext context)
    {
        var payload = JsonSerializer.Deserialize<ClientPersonaData>(data);
        if (payload is null) return;

        if (payload.EngagementId.HasValue) context.EngagementIds.Add(payload.EngagementId.Value);
        if (payload.ActorUserId.HasValue) context.UserIds.Add(payload.ActorUserId.Value);
    }


    public IDictionary<string, object?> Map(string data, BatchLoadContext context, int? actorUserId = null)
    {
        var payload = JsonSerializer.Deserialize<ClientPersonaData>(data)
            ?? throw new InvalidOperationException(AuditConstants.Messages.ParsePayloadFailed(TemplateKey));

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
