using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Pdr.Api.Constants;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Services.Notifications.Mappers;

public class ResourceRequestAllocatedMapper(IResourceRequestRepository resourceRequestRepository)
    : INotificationTemplateMapper
{
    public string TemplateKey => "ResourceRequestAllocated";

    public record ResourceRequestAllocatedData(int ResourceRequestId, int EngagementId);

    public async Task<IDictionary<string, object?>> MapAsync(string data, CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Deserialize<ResourceRequestAllocatedData>(data)
            ?? throw new InvalidOperationException("ResourceRequestAllocated notification data could not be parsed.");

        var request = await resourceRequestRepository.GetByIdAsync(payload.ResourceRequestId);
        var engagement = await resourceRequestRepository.GetEngagementAsync(payload.EngagementId);

        var roleName = request != null && ResourceRequestConstants.StandardRoleDisplayNames.TryGetValue(request.StandardRole, out var name)
            ? name
            : request?.StandardRole.ToString();

        return new Dictionary<string, object?>
        {
            ["RoleName"] = roleName,
            ["EngagementName"] = engagement?.EngagementName,
            ["EngagementId"] = payload.EngagementId
        };
    }
}
