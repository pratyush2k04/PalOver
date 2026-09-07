using System.Text.Json;
using Pdr.Api.Constants;
using Pdr.Api.Models.Audit;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Services.Audit.Mappers;

public class ClientPersonaChangedMapper(IUserRepository userRepository) : IAuditTemplateMapper
{
    public string TemplateKey => AuditConstants.ClientPersonaChanged.TemplateKey;

    private record ClientPersonaData(int? EngagementId, string? ClientName, string? Persona, int? ActorUserId);

    public async Task<IDictionary<string, object?>> MapAsync(string data, CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Deserialize<ClientPersonaData>(data)
            ?? throw new InvalidOperationException(AuditConstants.ClientPersonaChanged.ParseErrorMessage);

        var actor = payload.ActorUserId.HasValue
            ? await userRepository.GetByIdAsync(payload.ActorUserId.Value)
            : null;

        return new Dictionary<string, object?>
        {
            [AuditConstants.ClientPersonaChanged.ClientNameKey] = payload.ClientName ?? "Client",
            [AuditConstants.ClientPersonaChanged.ActorNameKey] = actor?.FullName ?? "Unknown User"
        };
    }
}
