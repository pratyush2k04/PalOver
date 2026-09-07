using System.Text.Json;
using Pdr.Api.Services.Templating;
using Pdr.Api.Constants;

namespace Pdr.Api.Services.Notifications.Mappers;

public class PresalesCommitmentAddedMapper : INotificationTemplateMapper
{
    public string TemplateKey => NotificationConstants.TemplateKeys.PresalesCommitmentAdded;

    public record PresalesCommitmentAddedPayload(
        int EngagementId,
        int CommitmentId,
        string CommitmentTitle);
    public void CollectIds(string data, BatchLoadContext context)
    {
        var payload = JsonSerializer.Deserialize<PresalesCommitmentAddedPayload>(data);
        if (payload is null) return;

        context.EngagementIds.Add(payload.EngagementId);
    }


    public IDictionary<string, object?> Map(string data, BatchLoadContext context, int? actorUserId = null)
    {
        var payload = JsonSerializer.Deserialize<PresalesCommitmentAddedPayload>(data)
            ?? throw new InvalidOperationException(NotificationConstants.Messages.ParsePayloadFailed(NotificationConstants.TemplateKeys.PresalesCommitmentAdded));

        var engagement = context.Engagements.GetValueOrDefault(payload.EngagementId);

        return new Dictionary<string, object?>
        {
            ["EngagementName"] = engagement?.EngagementName ?? "Engagement",
            ["EngagementId"] = payload.EngagementId,
            ["CommitmentTitle"] = payload.CommitmentTitle,
            ["CommitmentId"] = payload.CommitmentId
        };
    }
}
