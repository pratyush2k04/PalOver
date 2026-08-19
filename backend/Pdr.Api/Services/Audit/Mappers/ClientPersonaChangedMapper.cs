using System.Text.Json;
using Pdr.Api.Models.Audit;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Services.Audit.Mappers;

public class ClientPersonaChangedMapper(IUserRepository userRepository) : IAuditTemplateMapper
{
    public string TemplateKey => nameof(AuditActionEnum.ClientPersonaChanged);

    private record ClientPersonaData(int? EngagementId, string? ClientName, string? Persona, int? ActorUserId);

    public async Task<IDictionary<string, object?>> MapAsync(string data, CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Deserialize<ClientPersonaData>(data)
            ?? throw new InvalidOperationException("ClientPersonaChanged audit payload could not be parsed.");

        var actor = payload.ActorUserId.HasValue
            ? await userRepository.GetByIdAsync(payload.ActorUserId.Value)
            : null;

        return new Dictionary<string, object?>
        {
            ["clientName"] = payload.ClientName ?? "Client",
            ["actorName"] = actor?.FullName ?? "Unknown User"
        };
    }
}
