using System.Text.Json;
using Pdr.Api.Constants;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Services.Notifications.Mappers;

public class KickoffActionItemAssignedMapper(
    IKickoffRepository kickoffRepository,
    IEngagementRepository engagementRepository) : INotificationTemplateMapper
{
    public string TemplateKey => NotificationConstants.TemplateKeys.KickoffActionItemAssigned;

    public record KickoffActionItemAssignedPayload(int ActionItemId, int EngagementId);

    public async Task<IDictionary<string, object?>> MapAsync(string data, CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Deserialize<KickoffActionItemAssignedPayload>(data)
            ?? throw new InvalidOperationException(NotificationConstants.Messages.ParsePayloadFailed(NotificationConstants.TemplateKeys.KickoffActionItemAssigned));

        var actionItem = await kickoffRepository.GetActionItemByIdAsync(payload.ActionItemId);
        var engagement = await engagementRepository.GetByIdAsync(payload.EngagementId);

        return new Dictionary<string, object?>
        {
            ["ActionTitle"] = actionItem?.Title,
            ["EngagementName"] = engagement?.EngagementName,
            ["EngagementId"] = payload.EngagementId
        };
    }
}
