using System;
using System.Collections.Generic;
using System.Text.Json;
using Pdr.Api.Services.Templating;
using Pdr.Api.Models.Audit;

namespace Pdr.Api.Services.Audit.Mappers;

public class ChecklistItemEvidenceUpdatedMapper : IAuditTemplateMapper
{
    public string TemplateKey => nameof(AuditActionEnum.ChecklistItemEvidenceUpdated);

    private record ChecklistItemEvidenceUpdatedData(int EngagementId, int ChecklistItemId, int ActorUserId);
    public void CollectIds(string data, BatchLoadContext context)
    {
        var payload = JsonSerializer.Deserialize<ChecklistItemEvidenceUpdatedData>(data);
        if (payload is null) return;

        context.EngagementIds.Add(payload.EngagementId);
        context.UserIds.Add(payload.ActorUserId);
        context.ChecklistItemIds.Add(payload.ChecklistItemId);
    }


    public IDictionary<string, object?> Map(string data, BatchLoadContext context, int? actorUserId = null)
    {
        var payload = JsonSerializer.Deserialize<ChecklistItemEvidenceUpdatedData>(data)
            ?? throw new InvalidOperationException("ChecklistItemEvidenceUpdated audit payload could not be parsed.");

        var engagement = context.Engagements.GetValueOrDefault(payload.EngagementId);
        var item = context.ChecklistItems.GetValueOrDefault(payload.ChecklistItemId);
        var actor = context.Users.GetValueOrDefault(payload.ActorUserId) ?? (actorUserId.HasValue ? context.Users.GetValueOrDefault(actorUserId.Value) : null);

        return new Dictionary<string, object?>
        {
            ["itemTitle"] = item?.Title,
            ["engagementName"] = engagement?.EngagementName,
            ["actorName"] = actor?.FullName
        };
    }
}
