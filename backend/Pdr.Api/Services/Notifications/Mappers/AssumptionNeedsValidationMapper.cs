using System.Text.Json;
using Pdr.Api.Constants;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Services.Notifications.Mappers;

public class AssumptionNeedsValidationMapper(
    IEngagementRepository engagementRepository,
    IAssumptionRepository assumptionRepository) : INotificationTemplateMapper
{
    public string TemplateKey => NotificationConstants.TemplateKeys.AssumptionNeedsValidation;

    public record AssumptionNeedsValidationPayload(int AssumptionId, int EngagementId);

    public async Task<IDictionary<string, object?>> MapAsync(string data, CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Deserialize<AssumptionNeedsValidationPayload>(data)
            ?? throw new InvalidOperationException(NotificationConstants.Messages.ParsePayloadFailed(NotificationConstants.TemplateKeys.AssumptionNeedsValidation));

        var engagement = await engagementRepository.GetByIdAsync(payload.EngagementId);
        var assumption = await assumptionRepository.GetByIdAsync(payload.AssumptionId);

        return new Dictionary<string, object?>
        {
            ["AssumptionDescription"] = assumption?.Description,
            ["EngagementName"] = engagement?.EngagementName,
            ["EngagementId"] = payload.EngagementId
        };
    }
}
