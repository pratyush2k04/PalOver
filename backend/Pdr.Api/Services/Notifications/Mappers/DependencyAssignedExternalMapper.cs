using System.Text.Json;
using Pdr.Api.Constants;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Services.Notifications.Mappers;

public class DependencyAssignedExternalMapper(
    IEngagementRepository engagementRepository,
    IDependencyRepository dependencyRepository) : INotificationTemplateMapper
{
    public string TemplateKey => NotificationConstants.TemplateKeys.DependencyAssignedExternal;

    public record DependencyAssignedExternalPayload(int DependencyId, int EngagementId);

    public async Task<IDictionary<string, object?>> MapAsync(string data, CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Deserialize<DependencyAssignedExternalPayload>(data)
            ?? throw new InvalidOperationException(NotificationConstants.Messages.ParsePayloadFailed(NotificationConstants.TemplateKeys.DependencyAssignedExternal));

        var engagement = await engagementRepository.GetByIdAsync(payload.EngagementId);
        var dependency = await dependencyRepository.GetByIdAsync(payload.DependencyId);

        return new Dictionary<string, object?>
        {
            ["DependencyDescription"] = dependency?.Description,
            ["OwnerName"] = dependency?.Owner?.Name,
            ["EngagementName"] = engagement?.EngagementName,
            ["EngagementId"] = payload.EngagementId
        };
    }
}
