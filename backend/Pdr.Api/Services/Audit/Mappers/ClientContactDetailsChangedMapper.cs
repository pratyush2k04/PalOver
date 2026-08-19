using System.Text.Json;
using Pdr.Api.Models.Audit;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Services.Audit.Mappers;

public class ClientContactDetailsChangedMapper(IUserRepository userRepository) : IAuditTemplateMapper
{
    public string TemplateKey => nameof(AuditActionEnum.ClientContactDetailsChanged);

    private record ClientContactDetailsData(int? EngagementId, string? ClientName, int? ActorUserId);

    public async Task<IDictionary<string, object?>> MapAsync(string data, CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Deserialize<ClientContactDetailsData>(data)
            ?? throw new InvalidOperationException("ClientContactDetailsChanged audit payload could not be parsed.");

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
