using System.Text.Json;
using Pdr.Api.Models.Audit;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Services.Audit.Mappers;

// Registered for completeness (every seeded AuditAction has a mapper), but nothing in this
// codebase logs KickoffReminderFired today: there is no background job that runs at the moment
// a reminder's ScheduledFor threshold is crossed, so there's no distinct "fired" event to log
// beyond the KickoffReminderScheduled entry already written when the reminder was created/edited.
public class KickoffReminderFiredMapper(IEngagementRepository engagementRepository) : IAuditTemplateMapper
{
    public string TemplateKey => nameof(AuditActionEnum.KickoffReminderFired);

    public record KickoffReminderFiredData(int EngagementId, int ThresholdDays);

    public async Task<IDictionary<string, object?>> MapAsync(string data, CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Deserialize<KickoffReminderFiredData>(data)
            ?? throw new InvalidOperationException("KickoffReminderFired audit payload could not be parsed.");

        var engagement = await engagementRepository.GetByIdAsync(payload.EngagementId);

        return new Dictionary<string, object?>
        {
            ["engagementName"] = engagement?.EngagementName,
            ["thresholdLabel"] = KickoffReminderScheduledMapper.FormatThresholdLabel(payload.ThresholdDays)
        };
    }
}
