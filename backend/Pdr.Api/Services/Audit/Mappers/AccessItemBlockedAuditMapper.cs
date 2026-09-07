using System.Text.Json;
using Pdr.Api.Constants;
using Pdr.Api.Models.Audit;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Services.Audit.Mappers;

public class AccessItemBlockedAuditMapper(
    IAccessTrackerRepository accessTrackerRepository,
    IEngagementRepository engagementRepository,
    IUserRepository userRepository) : IAuditTemplateMapper
{
    public string TemplateKey => AuditConstants.AccessItemBlocked.TemplateKey;

    private record AccessItemBlockedData(int AccessItemId, int ActorUserId);

    public async Task<IDictionary<string, object?>> MapAsync(string data, CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Deserialize<AccessItemBlockedData>(data)
            ?? throw new InvalidOperationException(AuditConstants.AccessItemBlocked.ParseErrorMessage);

        var item = await accessTrackerRepository.GetAccessItemByIdAsync(payload.AccessItemId);
        var engagement = item is null ? null : await engagementRepository.GetByIdAsync(item.EngagementId);
        var actor = await userRepository.GetByIdAsync(payload.ActorUserId);

        return new Dictionary<string, object?>
        {
            [AuditConstants.AccessItemBlocked.ItemNameKey] = item?.Name ?? "Access item",
            [AuditConstants.AccessItemBlocked.EngagementNameKey] = engagement?.EngagementName ?? "Engagement",
            [AuditConstants.AccessItemBlocked.ActorNameKey] = actor?.FullName ?? "Unknown User"
        };
    }
}
