using System.Text.Json;
using Pdr.Api.Services.Templating;
using Pdr.Api.Constants;

namespace Pdr.Api.Services.Notifications.Mappers;

public class DocumentDeletedMapper : INotificationTemplateMapper
{
    public string TemplateKey => NotificationConstants.TemplateKeys.DocumentDeleted;

    public record DocumentDeletedPayload(int DocumentId, int EngagementId, int ActorUserId, string DocumentName);
    public void CollectIds(string data, BatchLoadContext context)
    {
        var payload = JsonSerializer.Deserialize<DocumentDeletedPayload>(data);
        if (payload is null) return;

        context.EngagementIds.Add(payload.EngagementId);
        context.UserIds.Add(payload.ActorUserId);
    }


    public IDictionary<string, object?> Map(string data, BatchLoadContext context, int? actorUserId = null)
    {
        var payload = JsonSerializer.Deserialize<DocumentDeletedPayload>(data)
            ?? throw new InvalidOperationException(NotificationConstants.Messages.ParsePayloadFailed(NotificationConstants.TemplateKeys.DocumentDeleted));

        var engagement = context.Engagements.GetValueOrDefault(payload.EngagementId);
        var actor = context.Users.GetValueOrDefault(payload.ActorUserId);

        return new Dictionary<string, object?>
        {
            ["DocumentName"] = payload.DocumentName,
            ["EngagementName"] = engagement?.EngagementName,
            ["ActorName"] = actor?.FullName,
            ["EngagementId"] = payload.EngagementId
        };
    }
}
