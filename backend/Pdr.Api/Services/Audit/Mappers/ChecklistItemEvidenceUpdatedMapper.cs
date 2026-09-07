using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Pdr.Api.Models.Audit;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Services.Audit.Mappers;

public class ChecklistItemEvidenceUpdatedMapper(
    IEngagementRepository engagementRepository,
    IEngagementChecklistRepository checklistRepository,
    IUserRepository userRepository) : IAuditTemplateMapper
{
    public string TemplateKey => nameof(AuditActionEnum.ChecklistItemEvidenceUpdated);

    private record ChecklistItemEvidenceUpdatedData(int EngagementId, int ChecklistItemId, int ActorUserId);

    public async Task<IDictionary<string, object?>> MapAsync(string data, CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Deserialize<ChecklistItemEvidenceUpdatedData>(data)
            ?? throw new InvalidOperationException("ChecklistItemEvidenceUpdated audit payload could not be parsed.");

        var engagement = await engagementRepository.GetByIdAsync(payload.EngagementId);
        var item = await checklistRepository.GetItemByIdAsync(payload.EngagementId, payload.ChecklistItemId);
        var actor = await userRepository.GetByIdAsync(payload.ActorUserId);

        return new Dictionary<string, object?>
        {
            ["itemTitle"] = item?.Title ?? "Checklist Item",
            ["engagementName"] = engagement?.EngagementName ?? "Engagement",
            ["actorName"] = actor?.FullName ?? "Unknown User"
        };
    }
}
