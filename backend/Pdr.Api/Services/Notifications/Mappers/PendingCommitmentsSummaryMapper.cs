using System.Text.Json;
using Pdr.Api.Services.Templating;
using Pdr.Api.Constants;

namespace Pdr.Api.Services.Notifications.Mappers;

public class PendingCommitmentsSummaryMapper : INotificationTemplateMapper
{
    public string TemplateKey => NotificationConstants.TemplateKeys.PendingCommitmentsSummary;

    public record PendingCommitmentsSummaryPayload(
        int EngagementId,
        int Count);
    public void CollectIds(string data, BatchLoadContext context)
    {
        var payload = JsonSerializer.Deserialize<PendingCommitmentsSummaryPayload>(data);
        if (payload is null) return;

        context.EngagementIds.Add(payload.EngagementId);
    }


    public IDictionary<string, object?> Map(string data, BatchLoadContext context, int? actorUserId = null)
    {
        var payload = JsonSerializer.Deserialize<PendingCommitmentsSummaryPayload>(data)
            ?? throw new InvalidOperationException(NotificationConstants.Messages.ParsePayloadFailed(NotificationConstants.TemplateKeys.PendingCommitmentsSummary));

        var engagement = context.Engagements.GetValueOrDefault(payload.EngagementId);

        return new Dictionary<string, object?>
        {
            ["EngagementName"] = engagement?.EngagementName,
            ["EngagementId"] = payload.EngagementId,
            ["Count"] = payload.Count
        };
    }
}
