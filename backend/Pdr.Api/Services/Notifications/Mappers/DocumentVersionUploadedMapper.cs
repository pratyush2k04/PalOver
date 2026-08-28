using System.Text.Json;
using Pdr.Api.Constants;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Services.Notifications.Mappers;

public class DocumentVersionUploadedMapper(
    IDocumentRepository documentRepository,
    IEngagementRepository engagementRepository,
    IUserRepository userRepository)
    : INotificationTemplateMapper
{
    public string TemplateKey => NotificationConstants.TemplateKeys.DocumentVersionUploaded;

    public record DocumentVersionUploadedPayload(int DocumentId, int EngagementId, int UploaderUserId, int Version);

    public async Task<IDictionary<string, object?>> MapAsync(string data, CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Deserialize<DocumentVersionUploadedPayload>(data)
            ?? throw new InvalidOperationException(NotificationConstants.Messages.ParsePayloadFailed(NotificationConstants.TemplateKeys.DocumentVersionUploaded));

        var document = await documentRepository.GetByIdAsync(payload.DocumentId);
        var engagement = await engagementRepository.GetByIdAsync(payload.EngagementId);
        var uploader = await userRepository.GetByIdAsync(payload.UploaderUserId);

        return new Dictionary<string, object?>
        {
            ["DocumentName"] = document?.FileName,
            ["EngagementName"] = engagement?.EngagementName,
            ["UploaderName"] = uploader?.FullName,
            ["Version"] = payload.Version,
            ["EngagementId"] = payload.EngagementId
        };
    }
}
