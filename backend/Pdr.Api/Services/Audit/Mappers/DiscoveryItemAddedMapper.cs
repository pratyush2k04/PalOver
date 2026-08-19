using System.Text.Json;
using Pdr.Api.Models.Audit;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Services.Audit.Mappers;

public class DiscoveryItemAddedMapper(
    IEngagementRepository engagementRepository,
    IUserRepository userRepository) : IAuditTemplateMapper
{
    public string TemplateKey => nameof(AuditActionEnum.DiscoveryItemAdded);

    private record DiscoveryItemAddedData(int EngagementId, string? ItemName, int ActorUserId);

    public async Task<IDictionary<string, object?>> MapAsync(string data, CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Deserialize<DiscoveryItemAddedData>(data)
            ?? throw new InvalidOperationException("DiscoveryItemAdded audit payload could not be parsed.");

        var engagement = await engagementRepository.GetByIdAsync(payload.EngagementId);
        var actor = await userRepository.GetByIdAsync(payload.ActorUserId);

        return new Dictionary<string, object?>
        {
            ["itemName"] = payload.ItemName ?? "Item",
            ["engagementName"] = engagement?.EngagementName ?? "Engagement",
            ["actorName"] = actor?.FullName ?? "Unknown User"
        };
    }
}
