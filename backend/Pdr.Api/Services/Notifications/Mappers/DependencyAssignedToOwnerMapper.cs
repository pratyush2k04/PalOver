using System.Text.Json;
using Pdr.Api.Constants;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Services.Notifications.Mappers;

public class DependencyAssignedToOwnerMapper(
    IEngagementRepository engagementRepository,
    IDependencyRepository dependencyRepository) : INotificationTemplateMapper
{
    public string TemplateKey => NotificationConstants.TemplateKeys.DependencyAssignedToOwner;

    public record DependencyAssignedPayload(int DependencyId, int EngagementId);

    public async Task<IDictionary<string, object?>> MapAsync(string data, CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Deserialize<DependencyAssignedPayload>(data)
            ?? throw new InvalidOperationException(NotificationConstants.Messages.ParsePayloadFailed(NotificationConstants.TemplateKeys.DependencyAssignedToOwner));

        var engagement = await engagementRepository.GetByIdAsync(payload.EngagementId);
        var dependency = await dependencyRepository.GetByIdAsync(payload.DependencyId);

        return new Dictionary<string, object?>
        {
            ["DependencyDescription"] = dependency?.Description,
            ["EngagementName"] = engagement?.EngagementName,
            ["EngagementId"] = payload.EngagementId
        };
    }
}
