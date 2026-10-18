using System.Text.Json;
using Pdr.Api.Constants;
using Pdr.Api.Services.Templating;
using Pdr.Api.Models.Audit;

namespace Pdr.Api.Services.Audit.Mappers;

public class DocumentVisibilityChangedMapper : IAuditTemplateMapper
{
    public string TemplateKey => nameof(AuditActionEnum.DocumentVisibilityChanged);

    private record DocumentVisibilityChangedData(int DocumentId, int ActorUserId, string Visibility, string? DocumentName);
    public void CollectIds(string data, BatchLoadContext context)
    {
        var payload = JsonSerializer.Deserialize<DocumentVisibilityChangedData>(data);
        if (payload is null) return;

        context.UserIds.Add(payload.ActorUserId);
        context.DocumentIds.Add(payload.DocumentId);
    }


    public IDictionary<string, object?> Map(string data, BatchLoadContext context, int? actorUserId = null)
    {
        var payload = JsonSerializer.Deserialize<DocumentVisibilityChangedData>(data)
            ?? throw new InvalidOperationException(AuditConstants.Messages.ParsePayloadFailed(TemplateKey));

        var doc = context.Documents.GetValueOrDefault(payload.DocumentId);
        var actor = context.Users.GetValueOrDefault(payload.ActorUserId) ?? (actorUserId.HasValue ? context.Users.GetValueOrDefault(actorUserId.Value) : null);

        return new Dictionary<string, object?>
        {
            ["documentName"] = doc?.FileName ?? payload.DocumentName,
            ["visibility"] = payload.Visibility,
            ["actorName"] = actor?.FullName
        };
    }
}
