using System.Text.Json;
using Pdr.Api.Constants;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Services.Notifications.Mappers;

public class TechLeadAssignedMapper(IEngagementRepository engagementRepository)
    : INotificationTemplateMapper
{
    public string TemplateKey => NotificationConstants.TemplateKeys.TechLeadAssigned;

    public record TechLeadAssignedPayload(int EngagementId);

    public async Task<IDictionary<string, object?>> MapAsync(string data, CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Deserialize<TechLeadAssignedPayload>(data)
            ?? throw new InvalidOperationException(NotificationConstants.Messages.ParsePayloadFailed(NotificationConstants.TemplateKeys.TechLeadAssigned));

        var engagement = await engagementRepository.GetByIdAsync(payload.EngagementId);

        return new Dictionary<string, object?>
        {
            ["EngagementName"] = engagement?.EngagementName,
            ["EngagementId"] = payload.EngagementId
        };
    }
}
