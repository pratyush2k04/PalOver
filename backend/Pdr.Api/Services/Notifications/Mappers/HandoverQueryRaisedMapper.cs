using System.Text.Json;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Services.Notifications.Mappers;

public class HandoverQueryRaisedMapper(
    IEngagementRepository engagementRepository,
    IUserRepository userRepository,
    IHandoverQueryRepository queryRepository)
    : INotificationTemplateMapper
{
    public string TemplateKey => "HandoverQueryRaised";

    public record HandoverQueryRaisedData(int QueryId, int HandoverId, int TriggerUserId, int EngagementId);

    public async Task<IDictionary<string, object?>> MapAsync(string data, CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Deserialize<HandoverQueryRaisedData>(data)
            ?? throw new InvalidOperationException("HandoverQueryRaised notification data could not be parsed.");

        var engagement = await engagementRepository.GetByIdAsync(payload.EngagementId);
        var triggerUser = await userRepository.GetByIdAsync(payload.TriggerUserId);
        var query = await queryRepository.GetByIdAsync(payload.QueryId, payload.HandoverId);

        return new Dictionary<string, object?>
        {
            ["EngagementName"] = engagement?.EngagementName,
            ["TriggerUserName"] = triggerUser?.FullName,
            ["FieldSection"] = query?.FieldSection.Name,
            ["EngagementId"] = payload.EngagementId
        };
    }
}
