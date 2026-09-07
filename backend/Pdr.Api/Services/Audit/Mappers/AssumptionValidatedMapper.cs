using System.Text.Json;
using Pdr.Api.Constants;
using Pdr.Api.Models.Audit;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Services.Audit.Mappers;

public class AssumptionValidatedMapper(
    IAssumptionRepository assumptionRepository,
    IEngagementRepository engagementRepository) : IAuditTemplateMapper
{
    public string TemplateKey => AuditConstants.AssumptionValidated.TemplateKey;

    private record AssumptionData(int AssumptionId, string Status);

    public async Task<IDictionary<string, object?>> MapAsync(string data, CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Deserialize<AssumptionData>(data)
            ?? throw new InvalidOperationException(AuditConstants.AssumptionValidated.ParseErrorMessage);

        var assumption = await assumptionRepository.GetByIdAsync(payload.AssumptionId);
        var engagement = assumption != null
            ? await engagementRepository.GetByIdAsync(assumption.EngagementId)
            : null;

        return new Dictionary<string, object?>
        {
            [AuditConstants.AssumptionValidated.AssumptionDescriptionKey] = TruncateDescription(assumption?.Description),
            [AuditConstants.AssumptionValidated.AssumptionStatusKey] = payload.Status,
            [AuditConstants.AssumptionValidated.EngagementNameKey] = engagement?.EngagementName ?? "Engagement"
        };
    }

    private static string TruncateDescription(string? description)
    {
        if (string.IsNullOrEmpty(description)) return "an assumption";
        return description.Substring(0, Math.Min(50, description.Length));
    }
}
