using System.Text.Json;
using Pdr.Api.Models.Audit;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Services.Audit.Mappers;

public class ClientContactTransitionedMapper(IUserRepository userRepository) : IAuditTemplateMapper
{
    public string TemplateKey => nameof(AuditActionEnum.ClientContactTransitioned);

    private record ClientContactTransitionedData(int? EngagementId, string? OutgoingName, string? IncomingName, int? ActorUserId);

    public async Task<IDictionary<string, object?>> MapAsync(string data, CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Deserialize<ClientContactTransitionedData>(data)
            ?? throw new InvalidOperationException("ClientContactTransitioned audit payload could not be parsed.");

        var actor = payload.ActorUserId.HasValue
            ? await userRepository.GetByIdAsync(payload.ActorUserId.Value)
            : null;

        return new Dictionary<string, object?>
        {
            ["outgoingName"] = payload.OutgoingName ?? "Unknown",
            ["incomingName"] = payload.IncomingName ?? "Unknown",
            ["actorName"] = actor?.FullName ?? "Unknown User"
        };
    }
}
