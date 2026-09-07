using System.Text.Json;
using Pdr.Api.Constants;
using Pdr.Api.Models.Audit;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Services.Audit.Mappers;

public class ReadinessOnePagerGeneratedMapper(
    IEngagementRepository engagementRepository,
    IUserRepository userRepository) : IAuditTemplateMapper
{
    public string TemplateKey => AuditConstants.ReadinessOnePagerGenerated.TemplateKey;

    private record ReadinessOnePagerGeneratedData(int EngagementId, int ActorUserId);

    public async Task<IDictionary<string, object?>> MapAsync(string data, CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Deserialize<ReadinessOnePagerGeneratedData>(data)
            ?? throw new InvalidOperationException(AuditConstants.ReadinessOnePagerGenerated.ParseErrorMessage);

        var engagement = await engagementRepository.GetByIdAsync(payload.EngagementId);
        var actor = await userRepository.GetByIdAsync(payload.ActorUserId);

        return new Dictionary<string, object?>
        {
            [AuditConstants.ReadinessOnePagerGenerated.EngagementNameKey] = engagement?.EngagementName ?? "Engagement",
            [AuditConstants.ReadinessOnePagerGenerated.ActorNameKey] = actor?.FullName ?? "Unknown User"
        };
    }
}
