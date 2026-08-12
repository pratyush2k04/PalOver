using System.Text.Json;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Services.Notifications.Mappers;

public class PMAllocatedMapper(IEngagementRepository engagementRepository, IUserRepository userRepository)
    : INotificationTemplateMapper
{
    public string TemplateKey => "PMAllocated";

    private record PMAllocatedData(int AssignedUserId, int EngagementId);

    public async Task<IDictionary<string, object?>> MapAsync(string data, CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Deserialize<PMAllocatedData>(data)
            ?? throw new InvalidOperationException("PMAllocated notification data could not be parsed.");

        var engagement = await engagementRepository.GetByIdAsync(payload.EngagementId);
        var pm = await userRepository.GetByIdAsync(payload.AssignedUserId);

        return new Dictionary<string, object?>
        {
            ["PMName"] = pm?.FullName,
            ["EngagementName"] = engagement?.EngagementName,
            ["EngagementId"] = payload.EngagementId
        };
    }
}
