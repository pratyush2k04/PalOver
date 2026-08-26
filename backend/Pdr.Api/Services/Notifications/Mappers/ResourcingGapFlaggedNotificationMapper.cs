using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Pdr.Api.Constants;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Services.Notifications.Mappers;

public class ResourcingGapFlaggedNotificationMapper(IEngagementRepository engagementRepository)
    : INotificationTemplateMapper
{
    public string TemplateKey => NotificationConstants.TemplateKeys.ResourcingGapFlagRaised;

    public record ResourcingGapFlaggedNotificationData(int EngagementId, int UnconfirmedCount, int DaysToBilling);

    public async Task<IDictionary<string, object?>> MapAsync(string data, CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Deserialize<ResourcingGapFlaggedNotificationData>(data)
            ?? throw new InvalidOperationException("ResourcingGapFlagRaised notification data could not be parsed.");

        var engagement = await engagementRepository.GetByIdAsync(payload.EngagementId);

        return new Dictionary<string, object?>
        {
            ["EngagementName"] = engagement?.EngagementName ?? "Engagement",
            ["UnconfirmedCount"] = payload.UnconfirmedCount,
            ["DaysToBilling"] = payload.DaysToBilling,
            ["BillingDate"] = engagement?.BillingStartDate?.ToString("dd MMM yyyy") ?? "unspecified",
            ["EngagementId"] = payload.EngagementId
        };
    }
}
