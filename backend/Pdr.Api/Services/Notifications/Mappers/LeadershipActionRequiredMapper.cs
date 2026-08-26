using System.Text.Json;
using Pdr.Api.Constants;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Services.Notifications.Mappers;

public class LeadershipActionRequiredMapper(IEngagementRepository engagementRepository)
    : INotificationTemplateMapper
{
    public string TemplateKey => NotificationConstants.TemplateKeys.LeadershipActionRequired;

    public record LeadershipActionRequiredPayload(int EngagementId);

    public async Task<IDictionary<string, object?>> MapAsync(string data, CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Deserialize<LeadershipActionRequiredPayload>(data)
            ?? throw new InvalidOperationException(NotificationConstants.Messages.ParsePayloadFailed(NotificationConstants.TemplateKeys.LeadershipActionRequired));

        var engagement = await engagementRepository.GetByIdAsync(payload.EngagementId);

        return new Dictionary<string, object?>
        {
            ["EngagementName"] = engagement?.EngagementName,
            ["EngagementId"] = payload.EngagementId
        };
    }
}
