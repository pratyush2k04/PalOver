using System.Text.Json;
using Pdr.Api.Services.Templating;
using Pdr.Api.Models.Audit;

namespace Pdr.Api.Services.Audit.Mappers;

public class MasterChecklistPublishedMapper : IAuditTemplateMapper
{
    public string TemplateKey => nameof(AuditActionEnum.MasterChecklistPublished);

    private record MasterChecklistPublishedData(int Version, int ActorUserId);
    public void CollectIds(string data, BatchLoadContext context)
    {
        var payload = JsonSerializer.Deserialize<MasterChecklistPublishedData>(data);
        if (payload is null) return;

        context.UserIds.Add(payload.ActorUserId);
    }


    public IDictionary<string, object?> Map(string data, BatchLoadContext context, int? actorUserId = null)
    {
        var payload = JsonSerializer.Deserialize<MasterChecklistPublishedData>(data)
            ?? throw new InvalidOperationException("MasterChecklistPublished audit payload could not be parsed.");

        var actor = context.Users.GetValueOrDefault(payload.ActorUserId) ?? (actorUserId.HasValue ? context.Users.GetValueOrDefault(actorUserId.Value) : null);

        return new Dictionary<string, object?>
        {
            ["version"] = payload.Version.ToString(),
            ["actorName"] = actor?.FullName
        };
    }
}
