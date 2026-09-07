using System.Text.Json;
using Pdr.Api.Services.Templating;
using Pdr.Api.Constants;

namespace Pdr.Api.Services.Notifications.Mappers;

public class DocumentVersionUploadedMapper : INotificationTemplateMapper
{
    public string TemplateKey => NotificationConstants.TemplateKeys.DocumentVersionUploaded;

    public record DocumentVersionUploadedPayload(int DocumentId, int EngagementId, int UploaderUserId, int Version);
    public void CollectIds(string data, BatchLoadContext context)
    {
        var payload = JsonSerializer.Deserialize<DocumentVersionUploadedPayload>(data);
        if (payload is null) return;

        context.EngagementIds.Add(payload.EngagementId);
        context.UserIds.Add(payload.UploaderUserId);
        context.DocumentIds.Add(payload.DocumentId);
    }


    public IDictionary<string, object?> Map(string data, BatchLoadContext context, int? actorUserId = null)
    {
        var payload = JsonSerializer.Deserialize<DocumentVersionUploadedPayload>(data)
            ?? throw new InvalidOperationException(NotificationConstants.Messages.ParsePayloadFailed(NotificationConstants.TemplateKeys.DocumentVersionUploaded));

        var document = context.Documents.GetValueOrDefault(payload.DocumentId);
        var engagement = context.Engagements.GetValueOrDefault(payload.EngagementId);
        var uploader = context.Users.GetValueOrDefault(payload.UploaderUserId);

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
