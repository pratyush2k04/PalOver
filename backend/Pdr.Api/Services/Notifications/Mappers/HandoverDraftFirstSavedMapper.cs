using System.Text.Json;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Services.Notifications.Mappers;

public class HandoverDraftFirstSavedMapper(IEngagementRepository engagementRepository, IUserRepository userRepository)
    : INotificationTemplateMapper
{
    public string TemplateKey => "HandoverDraftFirstSaved";

    public record HandoverDraftFirstSavedPayload(int HandoverId, int TriggerUserId, int EngagementId);

    public async Task<IDictionary<string, object?>> MapAsync(string data, CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Deserialize<HandoverDraftFirstSavedPayload>(data)
            ?? throw new InvalidOperationException("HandoverDraftFirstSaved notification data could not be parsed.");

        var engagement = await engagementRepository.GetByIdAsync(payload.EngagementId);
        var actor = await userRepository.GetByIdAsync(payload.TriggerUserId);

        return new Dictionary<string, object?>
        {
            ["EngagementName"] = engagement?.EngagementName,
            ["ActorName"] = actor?.FullName,
            ["EngagementId"] = payload.EngagementId
        };
    }
}
