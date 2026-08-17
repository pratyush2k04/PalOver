using System.Text.Json;
using Pdr.Api.Models.Audit;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Services.Audit.Mappers;

public class KickoffReminderSuppressedMapper(IEngagementRepository engagementRepository) : IAuditTemplateMapper
{
    public string TemplateKey => nameof(AuditActionEnum.KickoffReminderSuppressed);

    public record KickoffReminderSuppressedData(int EngagementId, List<int> ThresholdDays, string? KickoffDate);

    public async Task<IDictionary<string, object?>> MapAsync(string data, CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Deserialize<KickoffReminderSuppressedData>(data)
            ?? throw new InvalidOperationException("KickoffReminderSuppressed audit payload could not be parsed.");

        var engagement = await engagementRepository.GetByIdAsync(payload.EngagementId);

        return new Dictionary<string, object?>
        {
            ["engagementName"] = engagement?.EngagementName,
            ["thresholdsLabel"] = FormatThresholdsLabel(payload.ThresholdDays),
            ["kickoffDate"] = payload.KickoffDate
        };
    }

    internal static string FormatThresholdsLabel(List<int> thresholdDays) =>
        string.Join(", ", thresholdDays.Select(KickoffReminderScheduledMapper.FormatThresholdLabel));
}
