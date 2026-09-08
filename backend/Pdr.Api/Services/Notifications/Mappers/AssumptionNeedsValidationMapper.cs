using System.Text.Json;
using Pdr.Api.Services.Templating;
using Pdr.Api.Constants;

namespace Pdr.Api.Services.Notifications.Mappers;

public class AssumptionNeedsValidationMapper : INotificationTemplateMapper
{
    public string TemplateKey => NotificationConstants.TemplateKeys.AssumptionNeedsValidation;

    public record AssumptionNeedsValidationPayload(int AssumptionId, int EngagementId);
    public void CollectIds(string data, BatchLoadContext context)
    {
        var payload = JsonSerializer.Deserialize<AssumptionNeedsValidationPayload>(data);
        if (payload is null) return;

        context.EngagementIds.Add(payload.EngagementId);
        context.AssumptionIds.Add(payload.AssumptionId);
    }


    public IDictionary<string, object?> Map(string data, BatchLoadContext context, int? actorUserId = null)
    {
        var payload = JsonSerializer.Deserialize<AssumptionNeedsValidationPayload>(data)
            ?? throw new InvalidOperationException(NotificationConstants.Messages.ParsePayloadFailed(NotificationConstants.TemplateKeys.AssumptionNeedsValidation));

        var engagement = context.Engagements.GetValueOrDefault(payload.EngagementId);
        var assumption = context.Assumptions.GetValueOrDefault(payload.AssumptionId);

        return new Dictionary<string, object?>
        {
            ["AssumptionDescription"] = assumption?.Description,
            ["EngagementName"] = engagement?.EngagementName,
            ["EngagementId"] = payload.EngagementId
        };
    }
}
