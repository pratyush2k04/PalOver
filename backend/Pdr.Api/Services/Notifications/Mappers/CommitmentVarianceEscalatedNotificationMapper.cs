using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Pdr.Api.Constants;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Services.Notifications.Mappers;

public class CommitmentVarianceEscalatedNotificationMapper(IEngagementRepository engagementRepository)
    : INotificationTemplateMapper
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

    public async Task<IDictionary<string, object?>> MapAsync(string data, CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Deserialize<CommitmentVarianceNotificationData>(data)
            ?? throw new InvalidOperationException(NotificationConstants.CommitmentVarianceEscalated.ParseErrorMessage);

        var engagement = await engagementRepository.GetByIdAsync(payload.EngagementId);

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
