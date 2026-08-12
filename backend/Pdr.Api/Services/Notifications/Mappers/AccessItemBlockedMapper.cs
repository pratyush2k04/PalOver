using System.Text.Json;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Services.Notifications.Mappers;

public class AccessItemBlockedMapper(IAccessTrackerRepository accessTrackerRepository, IUserRepository userRepository)
    : INotificationTemplateMapper
{
    public string TemplateKey => "AccessItemBlocked";

    public record AccessItemBlockedData(int AccessItemId, int TriggerUserId);

    public async Task<IDictionary<string, object?>> MapAsync(string data, CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Deserialize<AccessItemBlockedData>(data)
            ?? throw new InvalidOperationException("AccessItemBlocked notification data could not be parsed.");

        var accessItem = await accessTrackerRepository.GetAccessItemByIdAsync(payload.AccessItemId);
        var triggerUser = await userRepository.GetByIdAsync(payload.TriggerUserId);

        return new Dictionary<string, object?>
        {
            ["ItemName"] = accessItem?.Name,
            ["TriggerUserName"] = triggerUser?.FullName,
            ["BlockReason"] = accessItem?.BlockReason,
            ["EngagementId"] = accessItem?.EngagementId
        };
    }
}
