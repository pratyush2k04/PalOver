using System.Text.Json;
using Pdr.Api.Constants;
using Pdr.Api.Models.Audit;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Services.Audit.Mappers;

public class HandoverQueryResolvedMapper(
    IUserRepository userRepository,
    IHandoverQueryRepository queryRepository,
    IEngagementRepository engagementRepository) : IAuditTemplateMapper
{
    public string TemplateKey => AuditConstants.HandoverQueryResolved.TemplateKey;

    public record HandoverQueryResolvedData(int QueryId, int HandoverId, int EngagementId, int ActorUserId);

    public async Task<IDictionary<string, object?>> MapAsync(string data, CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Deserialize<HandoverQueryResolvedData>(data)
            ?? throw new InvalidOperationException(AuditConstants.HandoverQueryResolved.ParseErrorMessage);

        var actor = await userRepository.GetByIdAsync(payload.ActorUserId);
        var query = await queryRepository.GetByIdAsync(payload.QueryId, payload.HandoverId);
        var engagement = await engagementRepository.GetByIdAsync(payload.EngagementId);

        return new Dictionary<string, object?>
        {
            [AuditConstants.HandoverQueryResolved.ActorNameKey] = actor?.FullName,
            [AuditConstants.HandoverQueryResolved.FieldSectionKey] = query?.FieldSection.Name,
            [AuditConstants.HandoverQueryResolved.EngagementNameKey] = engagement?.EngagementName
        };
    }
}
