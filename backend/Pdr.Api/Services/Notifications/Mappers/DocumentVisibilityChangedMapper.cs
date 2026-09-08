using System.Text.Json;
using Pdr.Api.Services.Templating;
using Pdr.Api.Constants;

namespace Pdr.Api.Services.Notifications.Mappers;

public class DocumentVisibilityChangedMapper : INotificationTemplateMapper
{
    public string TemplateKey => NotificationConstants.TemplateKeys.DocumentVisibilityChanged;

    public record DocumentVisibilityChangedPayload(int DocumentId, int EngagementId, int ActorUserId, string Visibility);
    public void CollectIds(string data, BatchLoadContext context)
    {
        var payload = JsonSerializer.Deserialize<DocumentVisibilityChangedPayload>(data);
        if (payload is null) return;

        context.EngagementIds.Add(payload.EngagementId);
        context.UserIds.Add(payload.ActorUserId);
        context.DocumentIds.Add(payload.DocumentId);
    }


    public IDictionary<string, object?> Map(string data, BatchLoadContext context, int? actorUserId = null)
    {
        var payload = JsonSerializer.Deserialize<DocumentVisibilityChangedPayload>(data)
            ?? throw new InvalidOperationException(NotificationConstants.Messages.ParsePayloadFailed(NotificationConstants.TemplateKeys.DocumentVisibilityChanged));

        var document = context.Documents.GetValueOrDefault(payload.DocumentId);
        var engagement = context.Engagements.GetValueOrDefault(payload.EngagementId);
        var actor = context.Users.GetValueOrDefault(payload.ActorUserId);

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
