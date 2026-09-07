using System.Text.Json;
using Pdr.Api.Constants;
using Pdr.Api.Models.Audit;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Services.Audit.Mappers;

public class KickoffReadyMarkedMapper(
    IEngagementRepository engagementRepository,
    IUserRepository userRepository) : IAuditTemplateMapper
{
    public string TemplateKey => AuditConstants.KickoffReadyMarked.TemplateKey;

    private record KickoffReadyMarkedData(int EngagementId, int ActorUserId);

    public async Task<IDictionary<string, object?>> MapAsync(string data, CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Deserialize<KickoffReadyMarkedData>(data)
            ?? throw new InvalidOperationException(AuditConstants.KickoffReadyMarked.ParseErrorMessage);

        var engagement = await engagementRepository.GetByIdAsync(payload.EngagementId);
        var actor = await userRepository.GetByIdAsync(payload.ActorUserId);

        return new Dictionary<string, object?>
        {
            [AuditConstants.KickoffReadyMarked.EngagementNameKey] = engagement?.EngagementName ?? "Engagement",
            [AuditConstants.KickoffReadyMarked.ActorNameKey] = actor?.FullName ?? "Unknown User"
        };
    }
}
