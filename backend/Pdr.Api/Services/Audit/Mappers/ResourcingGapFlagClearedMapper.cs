using System;
using System.Collections.Generic;
using System.Text.Json;
using Pdr.Api.Services.Templating;
using Pdr.Api.Models.Audit;

namespace Pdr.Api.Services.Audit.Mappers;

public class ResourcingGapFlagClearedMapper : IAuditTemplateMapper
{
    public string TemplateKey => nameof(AuditActionEnum.ResourcingGapFlagCleared);

    private record ResourcingGapFlagClearedData(int EngagementId);
    public void CollectIds(string data, BatchLoadContext context)
    {
        var payload = JsonSerializer.Deserialize<ResourcingGapFlagClearedData>(data);
        if (payload is null) return;

        context.EngagementIds.Add(payload.EngagementId);
    }


    public IDictionary<string, object?> Map(string data, BatchLoadContext context, int? actorUserId = null)
    {
        var payload = JsonSerializer.Deserialize<ResourcingGapFlagClearedData>(data)
            ?? throw new InvalidOperationException("ResourcingGapFlagCleared audit payload could not be parsed.");

        var engagement = context.Engagements.GetValueOrDefault(payload.EngagementId);

        return new Dictionary<string, object?>
        {
            ["engagementName"] = engagement?.EngagementName ?? "Engagement"
        };
    }
}
