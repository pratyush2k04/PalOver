using System.Text.Json;
using Pdr.Api.Models.Audit;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Services.Audit.Mappers;

public class HandoverQueryRaisedMapper(
    IUserRepository userRepository,
    IHandoverQueryRepository queryRepository,
    IEngagementRepository engagementRepository) : IAuditTemplateMapper
{
    public string TemplateKey => nameof(AuditActionEnum.HandoverQueryRaised);

    public record HandoverQueryRaisedData(int QueryId, int HandoverId, int EngagementId, int ActorUserId);

    public async Task<IDictionary<string, object?>> MapAsync(string data, CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Deserialize<HandoverQueryRaisedData>(data)
            ?? throw new InvalidOperationException("HandoverQueryRaised audit payload could not be parsed.");

        var actor = await userRepository.GetByIdAsync(payload.ActorUserId);
        var query = await queryRepository.GetByIdAsync(payload.QueryId, payload.HandoverId);
        var engagement = await engagementRepository.GetByIdAsync(payload.EngagementId);

        return new Dictionary<string, object?>
        {
            ["actorName"] = actor?.FullName,
            ["fieldSection"] = query?.FieldSection.Name,
            ["engagementName"] = engagement?.EngagementName
        };
    }
}
