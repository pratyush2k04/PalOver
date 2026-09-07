using System.Text.Json;
using Pdr.Api.Services.Templating;
using Pdr.Api.Models.Audit;

namespace Pdr.Api.Services.Audit.Mappers;

public class DocumentDeletedMapper : IAuditTemplateMapper
{
    public string TemplateKey => nameof(AuditActionEnum.DocumentDeleted);

    private record DocumentDeletedData(int DocumentId, int ActorUserId, string Reason, string? DocumentName);
    public void CollectIds(string data, BatchLoadContext context)
    {
        var payload = JsonSerializer.Deserialize<DocumentDeletedData>(data);
        if (payload is null) return;

        context.UserIds.Add(payload.ActorUserId);
    }


    public IDictionary<string, object?> Map(string data, BatchLoadContext context, int? actorUserId = null)
    {
        var payload = JsonSerializer.Deserialize<DocumentDeletedData>(data)
            ?? throw new InvalidOperationException("DocumentDeleted audit payload could not be parsed.");

        var actor = context.Users.GetValueOrDefault(payload.ActorUserId) ?? (actorUserId.HasValue ? context.Users.GetValueOrDefault(actorUserId.Value) : null);

        return new Dictionary<string, object?>
        {
            ["documentName"] = payload.DocumentName ?? "Unknown Document",
            ["reason"] = payload.Reason,
            ["actorName"] = actor?.FullName ?? "Unknown User"
        };
    }
}
