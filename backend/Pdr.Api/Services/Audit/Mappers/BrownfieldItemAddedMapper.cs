using System.Text.Json;
using Pdr.Api.Constants;
using Pdr.Api.Services.Templating;
using Pdr.Api.Models.Audit;

namespace Pdr.Api.Services.Audit.Mappers;

public class BrownfieldItemAddedMapper : IAuditTemplateMapper
{
    public string TemplateKey => nameof(AuditActionEnum.BrownfieldItemAdded);

    private record BrownfieldItemAddedData(int EngagementId, string? FindingName, int ActorUserId);
    public void CollectIds(string data, BatchLoadContext context)
    {
        var payload = JsonSerializer.Deserialize<BrownfieldItemAddedData>(data);
        if (payload is null) return;

        context.EngagementIds.Add(payload.EngagementId);
        context.UserIds.Add(payload.ActorUserId);
    }


    public IDictionary<string, object?> Map(string data, BatchLoadContext context, int? actorUserId = null)
    {
        var payload = JsonSerializer.Deserialize<BrownfieldItemAddedData>(data)
            ?? throw new InvalidOperationException(AuditConstants.Messages.ParsePayloadFailed(TemplateKey));

        var engagement = context.Engagements.GetValueOrDefault(payload.EngagementId);
        var actor = context.Users.GetValueOrDefault(payload.ActorUserId) ?? (actorUserId.HasValue ? context.Users.GetValueOrDefault(actorUserId.Value) : null);

        return new Dictionary<string, object?>
        {
            ["findingName"] = payload.FindingName,
            ["engagementName"] = engagement?.EngagementName,
            ["actorName"] = actor?.FullName
        };
    }
}
