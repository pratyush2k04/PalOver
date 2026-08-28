using System.Text.Json;
using Pdr.Api.Constants;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Services.Notifications.Mappers;

public class PendingCommitmentsSummaryMapper(IEngagementRepository engagementRepository)
    : INotificationTemplateMapper
{
    public string TemplateKey => NotificationConstants.TemplateKeys.PendingCommitmentsSummary;

    public record PendingCommitmentsSummaryPayload(
        int EngagementId,
        int Count);

    public async Task<IDictionary<string, object?>> MapAsync(string data, CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Deserialize<PendingCommitmentsSummaryPayload>(data)
            ?? throw new InvalidOperationException(NotificationConstants.Messages.ParsePayloadFailed(NotificationConstants.TemplateKeys.PendingCommitmentsSummary));

        var engagement = await engagementRepository.GetByIdAsync(payload.EngagementId);

        return new Dictionary<string, object?>
        {
            ["EngagementName"] = engagement?.EngagementName ?? "Engagement",
            ["EngagementId"] = payload.EngagementId,
            ["Count"] = payload.Count
        };
    }
}
