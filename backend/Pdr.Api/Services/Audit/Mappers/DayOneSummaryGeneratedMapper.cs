using System.Text.Json;
using Pdr.Api.Constants;
using Pdr.Api.Models.Audit;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Services.Audit.Mappers;

public class DayOneSummaryGeneratedMapper(
    IEngagementRepository engagementRepository,
    IUserRepository userRepository) : IAuditTemplateMapper
{
    public string TemplateKey => AuditConstants.DayOneSummaryGenerated.TemplateKey;

    private record DayOneSummaryGeneratedData(int EngagementId, int ActorUserId);

    public async Task<IDictionary<string, object?>> MapAsync(string data, CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Deserialize<DayOneSummaryGeneratedData>(data)
            ?? throw new InvalidOperationException(AuditConstants.DayOneSummaryGenerated.ParseErrorMessage);

        var engagement = await engagementRepository.GetByIdAsync(payload.EngagementId);
        var actor = await userRepository.GetByIdAsync(payload.ActorUserId);

        return new Dictionary<string, object?>
        {
            [AuditConstants.DayOneSummaryGenerated.EngagementNameKey] = engagement?.EngagementName ?? "Engagement",
            [AuditConstants.DayOneSummaryGenerated.ActorNameKey] = actor?.FullName ?? "Unknown User"
        };
    }
}
