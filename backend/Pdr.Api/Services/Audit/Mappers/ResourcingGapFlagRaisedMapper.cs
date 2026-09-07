using System;
using System.Collections.Generic;
using System.Text.Json;
using Pdr.Api.Constants;
using System.Threading;
using System.Threading.Tasks;
using Pdr.Api.Models.Audit;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Services.Audit.Mappers;

public class ResourcingGapFlagRaisedMapper(IEngagementRepository engagementRepository) : IAuditTemplateMapper
{
    public string TemplateKey => AuditConstants.ResourcingGapFlagRaised.TemplateKey;

    private record ResourcingGapFlagRaisedData(int EngagementId, int UnconfirmedCount, int? DaysToBilling);

    public async Task<IDictionary<string, object?>> MapAsync(string data, CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Deserialize<ResourcingGapFlagRaisedData>(data)
            ?? throw new InvalidOperationException(AuditConstants.ResourcingGapFlagRaised.ParseErrorMessage);

        var engagement = await engagementRepository.GetByIdAsync(payload.EngagementId);

        return new Dictionary<string, object?>
        {
            [AuditConstants.ResourcingGapFlagRaised.EngagementNameKey] = engagement?.EngagementName ?? "Engagement",
            [AuditConstants.ResourcingGapFlagRaised.UnconfirmedCountKey] = payload.UnconfirmedCount,
            [AuditConstants.ResourcingGapFlagRaised.DaysToBillingKey] = payload.DaysToBilling ?? 0
        };
    }
}
