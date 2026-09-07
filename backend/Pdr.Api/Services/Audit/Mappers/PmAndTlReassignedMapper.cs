using System.Text.Json;
using Pdr.Api.Constants;
using Pdr.Api.Models.Audit;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Services.Audit.Mappers;

public class PmAndTlReassignedMapper(
    IEngagementRepository engagementRepository,
    IUserRepository userRepository) : IAuditTemplateMapper
{
    public string TemplateKey => AuditConstants.PmAndTlReassigned.TemplateKey;

    private record PmAndTlReassignedData(int EngagementId, int ActorUserId);

    public async Task<IDictionary<string, object?>> MapAsync(string data, CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Deserialize<PmAndTlReassignedData>(data)
            ?? throw new InvalidOperationException(AuditConstants.PmAndTlReassigned.ParseErrorMessage);

        var engagement = await engagementRepository.GetByIdAsync(payload.EngagementId);
        var actor = await userRepository.GetByIdAsync(payload.ActorUserId);

        return new Dictionary<string, object?>
        {
            [AuditConstants.PmAndTlReassigned.EngagementNameKey] = engagement?.EngagementName ?? "Engagement",
            [AuditConstants.PmAndTlReassigned.ActorNameKey] = actor?.FullName ?? "Unknown User"
        };
    }
}
