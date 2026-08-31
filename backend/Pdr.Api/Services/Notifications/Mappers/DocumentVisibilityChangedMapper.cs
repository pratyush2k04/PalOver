using System.Text.Json;
using Pdr.Api.Constants;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Services.Notifications.Mappers;

public class DocumentVisibilityChangedMapper(
    IDocumentRepository documentRepository,
    IEngagementRepository engagementRepository,
    IUserRepository userRepository)
    : INotificationTemplateMapper
{
    public string TemplateKey => NotificationConstants.TemplateKeys.DocumentVisibilityChanged;

    public record DocumentVisibilityChangedPayload(int DocumentId, int EngagementId, int ActorUserId, string Visibility);

    public async Task<IDictionary<string, object?>> MapAsync(string data, CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Deserialize<DocumentVisibilityChangedPayload>(data)
            ?? throw new InvalidOperationException(NotificationConstants.Messages.ParsePayloadFailed(NotificationConstants.TemplateKeys.DocumentVisibilityChanged));

        var document = await documentRepository.GetByIdAsync(payload.DocumentId);
        var engagement = await engagementRepository.GetByIdAsync(payload.EngagementId);
        var actor = await userRepository.GetByIdAsync(payload.ActorUserId);

        return new Dictionary<string, object?>
        {
            ["DocumentName"] = document?.FileName,
            ["EngagementName"] = engagement?.EngagementName,
            ["ActorName"] = actor?.FullName,
            ["Visibility"] = payload.Visibility,
            ["EngagementId"] = payload.EngagementId
        };
    }
}
