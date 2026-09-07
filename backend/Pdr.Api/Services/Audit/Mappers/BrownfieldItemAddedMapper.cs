using System.Text.Json;
using Pdr.Api.Constants;
using Pdr.Api.Models.Audit;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Services.Audit.Mappers;

public class BrownfieldItemAddedMapper(
    IEngagementRepository engagementRepository,
    IUserRepository userRepository) : IAuditTemplateMapper
{
    public string TemplateKey => AuditConstants.BrownfieldItemAdded.TemplateKey;

    private record BrownfieldItemAddedData(int EngagementId, string? FindingName, int ActorUserId);

    public async Task<IDictionary<string, object?>> MapAsync(string data, CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Deserialize<BrownfieldItemAddedData>(data)
            ?? throw new InvalidOperationException(AuditConstants.BrownfieldItemAdded.ParseErrorMessage);

        var engagement = await engagementRepository.GetByIdAsync(payload.EngagementId);
        var actor = await userRepository.GetByIdAsync(payload.ActorUserId);

        return new Dictionary<string, object?>
        {
            [AuditConstants.BrownfieldItemAdded.FindingNameKey] = payload.FindingName ?? "Finding",
            [AuditConstants.BrownfieldItemAdded.EngagementNameKey] = engagement?.EngagementName ?? "Engagement",
            [AuditConstants.BrownfieldItemAdded.ActorNameKey] = actor?.FullName ?? "Unknown User"
        };
    }
}
