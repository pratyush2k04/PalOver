using System.Text.Json;
using Pdr.Api.Constants;
using Pdr.Api.Models.Audit;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Services.Audit.Mappers;

public class PmAndTlAssignedMapper(
    IEngagementRepository engagementRepository,
    IUserRepository userRepository) : IAuditTemplateMapper
{
    public string TemplateKey => AuditConstants.PmAndTlAssigned.TemplateKey;

    private record PmAndTlAssignedData(int EngagementId, int ActorUserId);

    public async Task<IDictionary<string, object?>> MapAsync(string data, CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Deserialize<PmAndTlAssignedData>(data)
            ?? throw new InvalidOperationException(AuditConstants.PmAndTlAssigned.ParseErrorMessage);

        var engagement = await engagementRepository.GetByIdAsync(payload.EngagementId);
        var actor = await userRepository.GetByIdAsync(payload.ActorUserId);

        return new Dictionary<string, object?>
        {
            [AuditConstants.PmAndTlAssigned.EngagementNameKey] = engagement?.EngagementName ?? "Engagement",
            [AuditConstants.PmAndTlAssigned.ActorNameKey] = actor?.FullName ?? "Unknown User"
        };
    }
}
