using System.Text.Json;
using Pdr.Api.Models.Audit;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Services.Audit.Mappers;

public class KickoffReminderReactivatedMapper(IEngagementRepository engagementRepository) : IAuditTemplateMapper
{
    public string TemplateKey => nameof(AuditActionEnum.KickoffReminderReactivated);

    public record KickoffReminderReactivatedData(int EngagementId, List<int> ThresholdDays, string? KickoffDate, int ReadinessScore);

    public async Task<IDictionary<string, object?>> MapAsync(string data, CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Deserialize<KickoffReminderReactivatedData>(data)
            ?? throw new InvalidOperationException("KickoffReminderReactivated audit payload could not be parsed.");

        var engagement = await engagementRepository.GetByIdAsync(payload.EngagementId);

        return new Dictionary<string, object?>
        {
            ["engagementName"] = engagement?.EngagementName,
            ["thresholdsLabel"] = KickoffReminderSuppressedMapper.FormatThresholdsLabel(payload.ThresholdDays),
            ["kickoffDate"] = payload.KickoffDate,
            ["readinessScore"] = payload.ReadinessScore
        };
    }
}
