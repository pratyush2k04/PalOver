using System.Text.Json;
using Pdr.Api.Constants;
using Pdr.Api.Models.Audit;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Services.Audit.Mappers;

public class DocumentDeletedMapper(IUserRepository userRepository) : IAuditTemplateMapper
{
    public string TemplateKey => AuditConstants.DocumentDeleted.TemplateKey;

    private record DocumentDeletedData(int DocumentId, int ActorUserId, string Reason, string? DocumentName);

    public async Task<IDictionary<string, object?>> MapAsync(string data, CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Deserialize<DocumentDeletedData>(data)
            ?? throw new InvalidOperationException(AuditConstants.DocumentDeleted.ParseErrorMessage);

        var actor = await userRepository.GetByIdAsync(payload.ActorUserId);

        return new Dictionary<string, object?>
        {
            [AuditConstants.DocumentDeleted.DocumentNameKey] = payload.DocumentName ?? "Unknown Document",
            [AuditConstants.DocumentDeleted.ReasonKey] = payload.Reason,
            [AuditConstants.DocumentDeleted.ActorNameKey] = actor?.FullName ?? "Unknown User"
        };
    }
}
