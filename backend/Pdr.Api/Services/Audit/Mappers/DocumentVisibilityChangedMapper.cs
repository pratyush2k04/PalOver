using System.Text.Json;
using Pdr.Api.Models.Audit;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Services.Audit.Mappers;

public class DocumentVisibilityChangedMapper(
    IDocumentRepository documentRepository,
    IUserRepository userRepository) : IAuditTemplateMapper
{
    public string TemplateKey => nameof(AuditActionEnum.DocumentVisibilityChanged);

    private record DocumentVisibilityChangedData(int DocumentId, int ActorUserId, string Visibility, string? DocumentName);

    public async Task<IDictionary<string, object?>> MapAsync(string data, CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Deserialize<DocumentVisibilityChangedData>(data)
            ?? throw new InvalidOperationException("DocumentVisibilityChanged audit payload could not be parsed.");

        var doc = await documentRepository.GetByIdAsync(payload.DocumentId);
        var actor = await userRepository.GetByIdAsync(payload.ActorUserId);

        return new Dictionary<string, object?>
        {
            ["documentName"] = doc?.FileName ?? payload.DocumentName ?? "Unknown Document",
            ["visibility"] = payload.Visibility,
            ["actorName"] = actor?.FullName ?? "Unknown User"
        };
    }
}
