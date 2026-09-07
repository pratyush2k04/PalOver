using System;
using System.Collections.Generic;
using System.Text.Json;
using Pdr.Api.Services.Templating;
using Pdr.Api.Constants;

namespace Pdr.Api.Services.Notifications.Mappers;

public class ResourcingGapFlaggedNotificationMapper : INotificationTemplateMapper
{
    public string TemplateKey => NotificationConstants.TemplateKeys.ResourcingGapFlagRaised;

    public record ResourcingGapFlaggedNotificationData(int EngagementId, int UnconfirmedCount, int DaysToBilling);
    public void CollectIds(string data, BatchLoadContext context)
    {
        var payload = JsonSerializer.Deserialize<ResourcingGapFlaggedNotificationData>(data);
        if (payload is null) return;

        context.EngagementIds.Add(payload.EngagementId);
    }


    public IDictionary<string, object?> Map(string data, BatchLoadContext context, int? actorUserId = null)
    {
        var payload = JsonSerializer.Deserialize<ResourcingGapFlaggedNotificationData>(data)
            ?? throw new InvalidOperationException(NotificationConstants.Messages.ParsePayloadFailed(TemplateKey));

        var engagement = context.Engagements.GetValueOrDefault(payload.EngagementId);

        return new Dictionary<string, object?>
        {
            ["EngagementName"] = engagement?.EngagementName,
            ["UnconfirmedCount"] = payload.UnconfirmedCount,
            ["DaysToBilling"] = payload.DaysToBilling,
            ["BillingDate"] = engagement?.BillingStartDate?.ToString("dd MMM yyyy"),
            ["EngagementId"] = payload.EngagementId
        };
    }
}
