using System.Text.Json;
using Pdr.Api.Models.Audit;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Services.Audit.Mappers;

public class KickoffReminderRetiredForBillingMapper(IEngagementRepository engagementRepository) : IAuditTemplateMapper
{
    public string TemplateKey => nameof(AuditActionEnum.KickoffReminderRetiredForBilling);

    public record KickoffReminderRetiredForBillingData(int EngagementId);

    public async Task<IDictionary<string, object?>> MapAsync(string data, CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Deserialize<KickoffReminderRetiredForBillingData>(data)
            ?? throw new InvalidOperationException("KickoffReminderRetiredForBilling audit payload could not be parsed.");

        var engagement = await engagementRepository.GetByIdAsync(payload.EngagementId);

        return new Dictionary<string, object?>
        {
            ["engagementName"] = engagement?.EngagementName
        };
    }
}
