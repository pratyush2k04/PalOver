using System.Text.Json;
using Pdr.Api.Constants;
using Pdr.Api.Models.Audit;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Services.Audit.Mappers;

public class DocumentVersionedMapper(
    IDocumentRepository documentRepository,
    IUserRepository userRepository) : IAuditTemplateMapper
{
    public string TemplateKey => AuditConstants.DocumentVersioned.TemplateKey;

    private record DocumentVersionedData(int DocumentId, int Version, int ActorUserId, string? DocumentName);

    public async Task<IDictionary<string, object?>> MapAsync(string data, CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Deserialize<DocumentVersionedData>(data)
            ?? throw new InvalidOperationException(AuditConstants.DocumentVersioned.ParseErrorMessage);

        var doc = await documentRepository.GetByIdAsync(payload.DocumentId);
        var actor = await userRepository.GetByIdAsync(payload.ActorUserId);

        return new Dictionary<string, object?>
        {
            [AuditConstants.DocumentVersioned.DocumentNameKey] = doc?.FileName ?? payload.DocumentName ?? "Unknown Document",
            [AuditConstants.DocumentVersioned.VersionKey] = payload.Version.ToString(),
            [AuditConstants.DocumentVersioned.ActorNameKey] = actor?.FullName ?? "Unknown User"
        };
    }
}
