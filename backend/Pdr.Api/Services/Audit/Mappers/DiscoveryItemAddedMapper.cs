using System.Text.Json;
using Pdr.Api.Constants;
using Pdr.Api.Models.Audit;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Services.Audit.Mappers;

public class DiscoveryItemAddedMapper(
    IEngagementRepository engagementRepository,
    IUserRepository userRepository) : IAuditTemplateMapper
{
    public string TemplateKey => AuditConstants.DiscoveryItemAdded.TemplateKey;

    private record DiscoveryItemAddedData(int EngagementId, string? ItemName, int ActorUserId);

    public async Task<IDictionary<string, object?>> MapAsync(string data, CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Deserialize<DiscoveryItemAddedData>(data)
            ?? throw new InvalidOperationException(AuditConstants.DiscoveryItemAdded.ParseErrorMessage);

        var engagement = await engagementRepository.GetByIdAsync(payload.EngagementId);
        var actor = await userRepository.GetByIdAsync(payload.ActorUserId);

        return new Dictionary<string, object?>
        {
            [AuditConstants.DiscoveryItemAdded.ItemNameKey] = payload.ItemName ?? "Item",
            [AuditConstants.DiscoveryItemAdded.EngagementNameKey] = engagement?.EngagementName ?? "Engagement",
            [AuditConstants.DiscoveryItemAdded.ActorNameKey] = actor?.FullName ?? "Unknown User"
        };
    }
}
