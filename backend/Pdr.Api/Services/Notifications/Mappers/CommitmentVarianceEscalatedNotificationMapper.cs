using System;
using System.Collections.Generic;
using System.Text.Json;
using Pdr.Api.Services.Templating;
using Pdr.Api.Constants;

namespace Pdr.Api.Services.Notifications.Mappers;

public class CommitmentVarianceEscalatedNotificationMapper : INotificationTemplateMapper
{
    public string TemplateKey => NotificationConstants.TemplateKeys.CommitmentVarianceEscalated;

    public record CommitmentVarianceNotificationData(
        int EngagementId,
        int CommitmentId,
        string Title,
        string VarianceDescription,
        int PercentGap,
        int WeekGap,
        int TriggerUserId);
    public void CollectIds(string data, BatchLoadContext context)
    {
        var payload = JsonSerializer.Deserialize<CommitmentVarianceNotificationData>(data);
        if (payload is null) return;

        context.EngagementIds.Add(payload.EngagementId);
        context.UserIds.Add(payload.TriggerUserId);
    }


    public IDictionary<string, object?> Map(string data, BatchLoadContext context, int? actorUserId = null)
    {
        var payload = JsonSerializer.Deserialize<CommitmentVarianceNotificationData>(data)
            ?? throw new InvalidOperationException(NotificationConstants.Messages.ParsePayloadFailed(TemplateKey));

        var engagement = context.Engagements.GetValueOrDefault(payload.EngagementId);

        return new Dictionary<string, object?>
        {
            [NotificationConstants.CommitmentVarianceEscalated.EngagementNameKey] = engagement?.EngagementName ?? NotificationConstants.CommitmentVarianceEscalated.DefaultEngagementName,
            [NotificationConstants.CommitmentVarianceEscalated.CommitmentTitleKey] = payload.Title,
            [NotificationConstants.CommitmentVarianceEscalated.VarianceDescriptionKey] = payload.VarianceDescription,
            [NotificationConstants.CommitmentVarianceEscalated.PercentGapKey] = payload.PercentGap,
            [NotificationConstants.CommitmentVarianceEscalated.WeekGapKey] = payload.WeekGap,
            [NotificationConstants.CommitmentVarianceEscalated.EngagementIdKey] = payload.EngagementId
        };
    }
}
