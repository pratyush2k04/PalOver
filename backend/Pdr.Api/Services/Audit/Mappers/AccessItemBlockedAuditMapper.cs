using System.Text.Json;
using Pdr.Api.Models.Audit;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Services.Audit.Mappers;

public class AccessItemBlockedAuditMapper(
    IAccessTrackerRepository accessTrackerRepository,
    IEngagementRepository engagementRepository,
    IUserRepository userRepository) : IAuditTemplateMapper
{
    public string TemplateKey => nameof(AuditActionEnum.AccessItemBlocked);

    private record AccessItemBlockedData(int AccessItemId, int ActorUserId);

    public async Task<IDictionary<string, object?>> MapAsync(string data, CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Deserialize<AccessItemBlockedData>(data)
            ?? throw new InvalidOperationException("AccessItemBlocked audit payload could not be parsed.");

        var item = await accessTrackerRepository.GetAccessItemByIdAsync(payload.AccessItemId);
        var engagement = item is null ? null : await engagementRepository.GetByIdAsync(item.EngagementId);
        var actor = await userRepository.GetByIdAsync(payload.ActorUserId);

        return new Dictionary<string, object?>
        {
            ["itemName"] = item?.Name ?? "Access item",
            ["engagementName"] = engagement?.EngagementName ?? "Engagement",
            ["actorName"] = actor?.FullName ?? "Unknown User"
        };
    }
}
