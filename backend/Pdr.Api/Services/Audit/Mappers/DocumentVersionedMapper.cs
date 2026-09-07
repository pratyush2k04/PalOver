using System.Text.Json;
using Pdr.Api.Services.Templating;
using Pdr.Api.Models.Audit;

namespace Pdr.Api.Services.Audit.Mappers;

public class DocumentVersionedMapper : IAuditTemplateMapper
{
    public string TemplateKey => nameof(AuditActionEnum.DocumentVersioned);

    private record DocumentVersionedData(int DocumentId, int Version, int ActorUserId, string? DocumentName);
    public void CollectIds(string data, BatchLoadContext context)
    {
        var payload = JsonSerializer.Deserialize<DocumentVersionedData>(data);
        if (payload is null) return;

        context.UserIds.Add(payload.ActorUserId);
        context.DocumentIds.Add(payload.DocumentId);
    }


    public IDictionary<string, object?> Map(string data, BatchLoadContext context, int? actorUserId = null)
    {
        var payload = JsonSerializer.Deserialize<DocumentVersionedData>(data)
            ?? throw new InvalidOperationException("DocumentVersioned audit payload could not be parsed.");

        var doc = context.Documents.GetValueOrDefault(payload.DocumentId);
        var actor = context.Users.GetValueOrDefault(payload.ActorUserId) ?? (actorUserId.HasValue ? context.Users.GetValueOrDefault(actorUserId.Value) : null);

        return new Dictionary<string, object?>
        {
            ["documentName"] = doc?.FileName ?? payload.DocumentName ?? "Unknown Document",
            ["version"] = payload.Version.ToString(),
            ["actorName"] = actor?.FullName ?? "Unknown User"
        };
    }
}
