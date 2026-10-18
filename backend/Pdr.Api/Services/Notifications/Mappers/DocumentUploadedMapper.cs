using System.Text.Json;
using Pdr.Api.Services.Templating;
using Pdr.Api.Constants;

namespace Pdr.Api.Services.Notifications.Mappers;

public class DocumentUploadedMapper : INotificationTemplateMapper
{
    public string TemplateKey => NotificationConstants.TemplateKeys.DocumentUploaded;

    public record DocumentUploadedPayload(int DocumentId, int EngagementId, int UploaderUserId);
    public void CollectIds(string data, BatchLoadContext context)
    {
        var payload = JsonSerializer.Deserialize<DocumentUploadedPayload>(data);
        if (payload is null) return;

        context.EngagementIds.Add(payload.EngagementId);
        context.UserIds.Add(payload.UploaderUserId);
        context.DocumentIds.Add(payload.DocumentId);
    }


    public IDictionary<string, object?> Map(string data, BatchLoadContext context, int? actorUserId = null)
    {
        var payload = JsonSerializer.Deserialize<DocumentUploadedPayload>(data)
            ?? throw new InvalidOperationException(NotificationConstants.Messages.ParsePayloadFailed(NotificationConstants.TemplateKeys.DocumentUploaded));

        var document = context.Documents.GetValueOrDefault(payload.DocumentId);
        var engagement = context.Engagements.GetValueOrDefault(payload.EngagementId);
        var uploader = context.Users.GetValueOrDefault(payload.UploaderUserId);

        return new Dictionary<string, object?>
        {
            ["DocumentName"] = document?.FileName,
            ["EngagementName"] = engagement?.EngagementName,
            ["UploaderName"] = uploader?.FullName,
            ["EngagementId"] = payload.EngagementId
        };
    }
}
