using System.Text.Json;
using Pdr.Api.Models.Audit;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Services.Audit.Mappers;

public class DocumentDeletedMapper(IUserRepository userRepository) : IAuditTemplateMapper
{
    public string TemplateKey => nameof(AuditActionEnum.DocumentDeleted);

    private record DocumentDeletedData(int DocumentId, int ActorUserId, string Reason, string? DocumentName);

    public async Task<IDictionary<string, object?>> MapAsync(string data, CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Deserialize<DocumentDeletedData>(data)
            ?? throw new InvalidOperationException("DocumentDeleted audit payload could not be parsed.");

        var actor = await userRepository.GetByIdAsync(payload.ActorUserId);

        return new Dictionary<string, object?>
        {
            ["documentName"] = payload.DocumentName ?? "Unknown Document",
            ["reason"] = payload.Reason,
            ["actorName"] = actor?.FullName ?? "Unknown User"
        };
    }
}
