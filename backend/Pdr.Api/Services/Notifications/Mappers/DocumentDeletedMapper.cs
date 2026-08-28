using System.Text.Json;
using Pdr.Api.Constants;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Services.Notifications.Mappers;

public class DocumentDeletedMapper(
    IEngagementRepository engagementRepository,
    IUserRepository userRepository)
    : INotificationTemplateMapper
{
    public string TemplateKey => NotificationConstants.TemplateKeys.DocumentDeleted;

    public record DocumentDeletedPayload(int DocumentId, int EngagementId, int ActorUserId, string DocumentName);

    public async Task<IDictionary<string, object?>> MapAsync(string data, CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Deserialize<DocumentDeletedPayload>(data)
            ?? throw new InvalidOperationException(NotificationConstants.Messages.ParsePayloadFailed(NotificationConstants.TemplateKeys.DocumentDeleted));

        var engagement = await engagementRepository.GetByIdAsync(payload.EngagementId);
        var actor = await userRepository.GetByIdAsync(payload.ActorUserId);

        return new Dictionary<string, object?>
        {
            ["DocumentName"] = payload.DocumentName,
            ["EngagementName"] = engagement?.EngagementName,
            ["ActorName"] = actor?.FullName,
            ["EngagementId"] = payload.EngagementId
        };
    }
}
