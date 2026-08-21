using System.Text.Json;
using Pdr.Api.Constants;
using Pdr.Api.Exceptions;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Services.Notifications.Mappers;

public class BlockerEscalatedPMMapper(IEngagementRepository engagementRepository)
    : INotificationTemplateMapper
{
    public string TemplateKey => NotificationConstants.TemplateKeys.BlockerEscalatedPM;

    public record BlockerEscalatedData(int EngagementId, int RiskId, string ItemTitle, int UnresolvedDays);

    public async Task<IDictionary<string, object?>> MapAsync(string data, CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Deserialize<BlockerEscalatedData>(data)
            ?? throw new NotificationDataInvalidException(TemplateKey);

        var engagement = await engagementRepository.GetByIdAsync(payload.EngagementId);

        return new Dictionary<string, object?>
        {
            ["EngagementName"] = engagement?.EngagementName ?? "Engagement",
            ["EngagementId"] = payload.EngagementId,
            ["RiskId"] = payload.RiskId,
            ["ItemTitle"] = payload.ItemTitle,
            ["UnresolvedDays"] = payload.UnresolvedDays
        };
    }
}
