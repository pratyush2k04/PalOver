using System.Text.Json;
using Pdr.Api.Constants;
using Pdr.Api.Services.Templating;
using Pdr.Api.Models.Audit;

namespace Pdr.Api.Services.Audit.Mappers;

public class HandoverQueryRaisedMapper : IAuditTemplateMapper
{
    public string TemplateKey => nameof(AuditActionEnum.HandoverQueryRaised);

    public record HandoverQueryRaisedData(int QueryId, int HandoverId, int EngagementId, int ActorUserId);
    public void CollectIds(string data, BatchLoadContext context)
    {
        var payload = JsonSerializer.Deserialize<HandoverQueryRaisedData>(data);
        if (payload is null) return;

        context.EngagementIds.Add(payload.EngagementId);
        context.UserIds.Add(payload.ActorUserId);
        context.HandoverQueryIds.Add(payload.QueryId);
    }


    public IDictionary<string, object?> Map(string data, BatchLoadContext context, int? actorUserId = null)
    {
        var payload = JsonSerializer.Deserialize<HandoverQueryRaisedData>(data)
            ?? throw new InvalidOperationException(AuditConstants.Messages.ParsePayloadFailed(TemplateKey));

        var actor = context.Users.GetValueOrDefault(payload.ActorUserId) ?? (actorUserId.HasValue ? context.Users.GetValueOrDefault(actorUserId.Value) : null);
        var query = context.HandoverQueries.GetValueOrDefault(payload.QueryId);
        var engagement = context.Engagements.GetValueOrDefault(payload.EngagementId);

        return new Dictionary<string, object?>
        {
            ["actorName"] = actor?.FullName,
            ["fieldSection"] = query?.FieldSection.Name,
            ["engagementName"] = engagement?.EngagementName
        };
    }
}
