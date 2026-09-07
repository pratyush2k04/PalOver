using System.Text.Json;
using Pdr.Api.Constants;
using Pdr.Api.Models.Audit;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Services.Audit.Mappers;

public class HandoverQueryRaisedMapper(
    IUserRepository userRepository,
    IHandoverQueryRepository queryRepository,
    IEngagementRepository engagementRepository) : IAuditTemplateMapper
{
    public string TemplateKey => AuditConstants.HandoverQueryRaised.TemplateKey;

    public record HandoverQueryRaisedData(int QueryId, int HandoverId, int EngagementId, int ActorUserId);

    public async Task<IDictionary<string, object?>> MapAsync(string data, CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Deserialize<HandoverQueryRaisedData>(data)
            ?? throw new InvalidOperationException(AuditConstants.HandoverQueryRaised.ParseErrorMessage);

        var actor = await userRepository.GetByIdAsync(payload.ActorUserId);
        var query = await queryRepository.GetByIdAsync(payload.QueryId, payload.HandoverId);
        var engagement = await engagementRepository.GetByIdAsync(payload.EngagementId);

        return new Dictionary<string, object?>
        {
            [AuditConstants.HandoverQueryRaised.ActorNameKey] = actor?.FullName,
            [AuditConstants.HandoverQueryRaised.FieldSectionKey] = query?.FieldSection.Name,
            [AuditConstants.HandoverQueryRaised.EngagementNameKey] = engagement?.EngagementName
        };
    }
}
