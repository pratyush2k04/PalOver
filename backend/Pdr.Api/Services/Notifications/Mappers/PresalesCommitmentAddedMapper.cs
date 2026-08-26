using System.Text.Json;
using Pdr.Api.Constants;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Services.Notifications.Mappers;

public class PresalesCommitmentAddedMapper(IEngagementRepository engagementRepository)
    : INotificationTemplateMapper
{
    public string TemplateKey => NotificationConstants.TemplateKeys.PresalesCommitmentAdded;

    public record PresalesCommitmentAddedPayload(
        int EngagementId,
        int CommitmentId,
        string CommitmentTitle);

    public async Task<IDictionary<string, object?>> MapAsync(string data, CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Deserialize<PresalesCommitmentAddedPayload>(data)
            ?? throw new InvalidOperationException(NotificationConstants.Messages.ParsePayloadFailed(NotificationConstants.TemplateKeys.PresalesCommitmentAdded));

        var engagement = await engagementRepository.GetByIdAsync(payload.EngagementId);

        return new Dictionary<string, object?>
        {
            ["EngagementName"] = engagement?.EngagementName ?? "Engagement",
            ["EngagementId"] = payload.EngagementId,
            ["CommitmentTitle"] = payload.CommitmentTitle,
            ["CommitmentId"] = payload.CommitmentId
        };
    }
}
