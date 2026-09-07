using System;
using System.Collections.Generic;
using System.Text.Json;
using Pdr.Api.Constants;
using System.Threading;
using System.Threading.Tasks;
using Pdr.Api.Models.Audit;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Services.Audit.Mappers;

public class ResourcingGapFlagClearedMapper(IEngagementRepository engagementRepository) : IAuditTemplateMapper
{
    public string TemplateKey => AuditConstants.ResourcingGapFlagCleared.TemplateKey;

    private record ResourcingGapFlagClearedData(int EngagementId);

    public async Task<IDictionary<string, object?>> MapAsync(string data, CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Deserialize<ResourcingGapFlagClearedData>(data)
            ?? throw new InvalidOperationException(AuditConstants.ResourcingGapFlagCleared.ParseErrorMessage);

        var engagement = await engagementRepository.GetByIdAsync(payload.EngagementId);

        return new Dictionary<string, object?>
        {
            [AuditConstants.ResourcingGapFlagCleared.EngagementNameKey] = engagement?.EngagementName ?? "Engagement"
        };
    }
}
