using System.Text.Json;
using Pdr.Api.Constants;
using Pdr.Api.Models.Audit;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Services.Audit.Mappers;

public class DocumentVisibilityChangedMapper(
    IDocumentRepository documentRepository,
    IUserRepository userRepository) : IAuditTemplateMapper
{
    public string TemplateKey => AuditConstants.DocumentVisibilityChanged.TemplateKey;

    private record DocumentVisibilityChangedData(int DocumentId, int ActorUserId, string Visibility, string? DocumentName);

    public async Task<IDictionary<string, object?>> MapAsync(string data, CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Deserialize<DocumentVisibilityChangedData>(data)
            ?? throw new InvalidOperationException(AuditConstants.DocumentVisibilityChanged.ParseErrorMessage);

        var doc = await documentRepository.GetByIdAsync(payload.DocumentId);
        var actor = await userRepository.GetByIdAsync(payload.ActorUserId);

        return new Dictionary<string, object?>
        {
            [AuditConstants.DocumentVisibilityChanged.DocumentNameKey] = doc?.FileName ?? payload.DocumentName ?? "Unknown Document",
            [AuditConstants.DocumentVisibilityChanged.VisibilityKey] = payload.Visibility,
            [AuditConstants.DocumentVisibilityChanged.ActorNameKey] = actor?.FullName ?? "Unknown User"
        };
    }
}
