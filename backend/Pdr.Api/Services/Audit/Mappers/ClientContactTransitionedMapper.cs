using System.Text.Json;
using Pdr.Api.Constants;
using Pdr.Api.Models.Audit;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Services.Audit.Mappers;

public class ClientContactTransitionedMapper(IUserRepository userRepository) : IAuditTemplateMapper
{
    public string TemplateKey => AuditConstants.ClientContactTransitioned.TemplateKey;

    private record ClientContactTransitionedData(int? EngagementId, string? OutgoingName, string? IncomingName, int? ActorUserId);

    public async Task<IDictionary<string, object?>> MapAsync(string data, CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Deserialize<ClientContactTransitionedData>(data)
            ?? throw new InvalidOperationException(AuditConstants.ClientContactTransitioned.ParseErrorMessage);

        var actor = payload.ActorUserId.HasValue
            ? await userRepository.GetByIdAsync(payload.ActorUserId.Value)
            : null;

        return new Dictionary<string, object?>
        {
            [AuditConstants.ClientContactTransitioned.OutgoingNameKey] = payload.OutgoingName ?? "Unknown",
            [AuditConstants.ClientContactTransitioned.IncomingNameKey] = payload.IncomingName ?? "Unknown",
            [AuditConstants.ClientContactTransitioned.ActorNameKey] = actor?.FullName ?? "Unknown User"
        };
    }
}
