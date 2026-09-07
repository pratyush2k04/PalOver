using System.Text.Json;
using Pdr.Api.Models.Audit;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Services.Audit.Mappers;

public class DocumentVersionedMapper(
    IDocumentRepository documentRepository,
    IUserRepository userRepository) : IAuditTemplateMapper
{
    public string TemplateKey => nameof(AuditActionEnum.DocumentVersioned);

    private record DocumentVersionedData(int DocumentId, int Version, int ActorUserId, string? DocumentName);

    public async Task<IDictionary<string, object?>> MapAsync(string data, CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Deserialize<DocumentVersionedData>(data)
            ?? throw new InvalidOperationException("DocumentVersioned audit payload could not be parsed.");

        var doc = await documentRepository.GetByIdAsync(payload.DocumentId);
        var actor = await userRepository.GetByIdAsync(payload.ActorUserId);

        return new Dictionary<string, object?>
        {
            ["documentName"] = doc?.FileName ?? payload.DocumentName ?? "Unknown Document",
            ["version"] = payload.Version.ToString(),
            ["actorName"] = actor?.FullName ?? "Unknown User"
        };
    }
}
