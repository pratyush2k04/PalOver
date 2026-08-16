using System.Text.Json;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Services.Notifications.Mappers;

public class HandoverSubmittedMapper(IEngagementRepository engagementRepository, IUserRepository userRepository)
    : INotificationTemplateMapper
{
    public string TemplateKey => "HandoverSubmitted";

    public record HandoverSubmittedData(int HandoverId, int TriggerUserId, int EngagementId);

    public async Task<IDictionary<string, object?>> MapAsync(string data, CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Deserialize<HandoverSubmittedData>(data)
            ?? throw new InvalidOperationException("HandoverSubmitted notification data could not be parsed.");

        var engagement = await engagementRepository.GetByIdAsync(payload.EngagementId);
        var triggerUser = await userRepository.GetByIdAsync(payload.TriggerUserId);

        return new Dictionary<string, object?>
        {
            ["EngagementName"] = engagement?.EngagementName,
            ["TriggerUserName"] = triggerUser?.FullName,
            ["EngagementId"] = payload.EngagementId
        };
    }
}
