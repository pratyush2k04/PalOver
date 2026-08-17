using System.Text.Json;
using Pdr.Api.Models.Audit;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Services.Audit.Mappers;

public class KickoffReminderScheduledMapper(IEngagementRepository engagementRepository) : IAuditTemplateMapper
{
    public string TemplateKey => nameof(AuditActionEnum.KickoffReminderScheduled);

    public record KickoffReminderScheduledData(int EngagementId, int ThresholdDays);

    public async Task<IDictionary<string, object?>> MapAsync(string data, CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Deserialize<KickoffReminderScheduledData>(data)
            ?? throw new InvalidOperationException("KickoffReminderScheduled audit payload could not be parsed.");

        var engagement = await engagementRepository.GetByIdAsync(payload.EngagementId);

        return new Dictionary<string, object?>
        {
            ["engagementName"] = engagement?.EngagementName,
            ["thresholdLabel"] = FormatThresholdLabel(payload.ThresholdDays)
        };
    }

    internal static string FormatThresholdLabel(int thresholdDays) =>
        thresholdDays == 1 ? "1 day" : $"{thresholdDays} days";
}
