using System.Text.Json;
using Pdr.Api.Services.Templating;
using Pdr.Api.Models.Audit;

namespace Pdr.Api.Services.Audit.Mappers;

public class DocumentUploadedMapper : IAuditTemplateMapper
{
    public string TemplateKey => nameof(AuditActionEnum.DocumentUploaded);

    private record DocumentUploadedData(int DocumentId, int CategoryId, int ActorUserId, string? DocumentName);
    public void CollectIds(string data, BatchLoadContext context)
    {
        var payload = JsonSerializer.Deserialize<DocumentUploadedData>(data);
        if (payload is null) return;

        context.UserIds.Add(payload.ActorUserId);
        context.DocumentIds.Add(payload.DocumentId);
        context.DocumentCategoryIds.Add(payload.CategoryId);
    }


    public IDictionary<string, object?> Map(string data, BatchLoadContext context, int? actorUserId = null)
    {
        var payload = JsonSerializer.Deserialize<DocumentUploadedData>(data)
            ?? throw new InvalidOperationException("DocumentUploaded audit payload could not be parsed.");

        var doc = context.Documents.GetValueOrDefault(payload.DocumentId);
                var category = doc?.Category ?? context.DocumentCategories.GetValueOrDefault(payload.CategoryId);
        var actor = context.Users.GetValueOrDefault(payload.ActorUserId) ?? (actorUserId.HasValue ? context.Users.GetValueOrDefault(actorUserId.Value) : null);

        return new Dictionary<string, object?>
        {
            ["documentName"] = doc?.FileName ?? payload.DocumentName,
            ["categoryName"] = category?.CategoryName,
            ["actorName"] = actor?.FullName
        };
    }
}
