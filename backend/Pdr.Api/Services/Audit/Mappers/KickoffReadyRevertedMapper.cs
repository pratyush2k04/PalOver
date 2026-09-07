using System.Text.Json;
using Pdr.Api.Constants;
using Pdr.Api.Models.Audit;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Services.Audit.Mappers;

public class KickoffReadyRevertedMapper(
    IEngagementRepository engagementRepository,
    IUserRepository userRepository) : IAuditTemplateMapper
{
    public string TemplateKey => AuditConstants.KickoffReadyReverted.TemplateKey;

    private record KickoffReadyRevertedData(int EngagementId, int ActorUserId);

    public async Task<IDictionary<string, object?>> MapAsync(string data, CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Deserialize<KickoffReadyRevertedData>(data)
            ?? throw new InvalidOperationException(AuditConstants.KickoffReadyReverted.ParseErrorMessage);

        var engagement = await engagementRepository.GetByIdAsync(payload.EngagementId);
        var actor = await userRepository.GetByIdAsync(payload.ActorUserId);

        return new Dictionary<string, object?>
        {
            [AuditConstants.KickoffReadyReverted.EngagementNameKey] = engagement?.EngagementName ?? "Engagement",
            [AuditConstants.KickoffReadyReverted.ActorNameKey] = actor?.FullName ?? "Unknown User"
        };
    }
}
