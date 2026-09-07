using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Pdr.Api.Models.Audit;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Services.Audit.Mappers;

public class ResourcingGapFlagRaisedMapper(IEngagementRepository engagementRepository) : IAuditTemplateMapper
{
    public string TemplateKey => nameof(AuditActionEnum.ResourcingGapFlagRaised);

    private record ResourcingGapFlagRaisedData(int EngagementId, int UnconfirmedCount, int? DaysToBilling);

    public async Task<IDictionary<string, object?>> MapAsync(string data, CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Deserialize<ResourcingGapFlagRaisedData>(data)
            ?? throw new InvalidOperationException("ResourcingGapFlagRaised audit payload could not be parsed.");

        var engagement = await engagementRepository.GetByIdAsync(payload.EngagementId);

        return new Dictionary<string, object?>
        {
            ["engagementName"] = engagement?.EngagementName ?? "Engagement",
            ["unconfirmedCount"] = payload.UnconfirmedCount,
            ["daysToBilling"] = payload.DaysToBilling ?? 0
        };
    }
}
