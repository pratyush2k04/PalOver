using System.Text.Json;
using Pdr.Api.Services.Templating;
using Pdr.Api.Constants;
using Pdr.Api.Exceptions;

namespace Pdr.Api.Services.Notifications.Mappers;

public class BlockerEscalatedLeadershipMapper : INotificationTemplateMapper
{
    public string TemplateKey => NotificationConstants.TemplateKeys.BlockerEscalatedLeadership;

    public record BlockerEscalatedData(int EngagementId, int RiskId, string ItemTitle, int UnresolvedDays);
    public void CollectIds(string data, BatchLoadContext context)
    {
        var payload = JsonSerializer.Deserialize<BlockerEscalatedData>(data);
        if (payload is null) return;

        context.EngagementIds.Add(payload.EngagementId);
        context.RiskIds.Add(payload.RiskId);
    }


    public IDictionary<string, object?> Map(string data, BatchLoadContext context, int? actorUserId = null)
    {
        var payload = JsonSerializer.Deserialize<BlockerEscalatedData>(data)
            ?? throw new NotificationDataInvalidException(TemplateKey);

        var engagement = context.Engagements.GetValueOrDefault(payload.EngagementId);

        return new Dictionary<string, object?>
        {
            ["EngagementName"] = engagement?.EngagementName,
            ["EngagementId"] = payload.EngagementId,
            ["RiskId"] = payload.RiskId,
            ["ItemTitle"] = payload.ItemTitle,
            ["UnresolvedDays"] = payload.UnresolvedDays
        };
    }
}
