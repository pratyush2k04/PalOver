using System.Text.Json;
using Pdr.Api.Models.Audit;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Services.Audit.Mappers;

public class LeadershipAllocatedMapper(
    IEngagementRepository engagementRepository,
    IUserRepository userRepository) : IAuditTemplateMapper
{
    public string TemplateKey => nameof(AuditActionEnum.LeadershipAllocated);

    private record LeadershipAllocatedData(int EngagementId, int ActorUserId);

    public async Task<IDictionary<string, object?>> MapAsync(string data, CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Deserialize<LeadershipAllocatedData>(data)
            ?? throw new InvalidOperationException("LeadershipAllocated audit payload could not be parsed.");

        var engagement = await engagementRepository.GetByIdAsync(payload.EngagementId);
        var actor = await userRepository.GetByIdAsync(payload.ActorUserId);

        return new Dictionary<string, object?>
        {
            ["engagementName"] = engagement?.EngagementName ?? "Engagement",
            ["actorName"] = actor?.FullName ?? "Unknown User"
        };
    }
}
