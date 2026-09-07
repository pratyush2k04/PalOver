using System;
using System.Collections.Generic;
using System.Text.Json;
using Pdr.Api.Services.Templating;
using Pdr.Api.Models.Audit;

namespace Pdr.Api.Services.Audit.Mappers;

public class ResourcingGapFlagRaisedMapper : IAuditTemplateMapper
{
    public string TemplateKey => nameof(AuditActionEnum.ResourcingGapFlagRaised);

    private record ResourcingGapFlagRaisedData(int EngagementId, int UnconfirmedCount, int? DaysToBilling);
    public void CollectIds(string data, BatchLoadContext context)
    {
        var payload = JsonSerializer.Deserialize<ResourcingGapFlagRaisedData>(data);
        if (payload is null) return;

        context.EngagementIds.Add(payload.EngagementId);
    }


    public IDictionary<string, object?> Map(string data, BatchLoadContext context, int? actorUserId = null)
    {
        var payload = JsonSerializer.Deserialize<ResourcingGapFlagRaisedData>(data)
            ?? throw new InvalidOperationException("ResourcingGapFlagRaised audit payload could not be parsed.");

        var engagement = context.Engagements.GetValueOrDefault(payload.EngagementId);

        return new Dictionary<string, object?>
        {
            ["engagementName"] = engagement?.EngagementName ?? "Engagement",
            ["unconfirmedCount"] = payload.UnconfirmedCount,
            ["daysToBilling"] = payload.DaysToBilling ?? 0
        };
    }
}
