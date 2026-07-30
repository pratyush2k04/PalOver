using Pdr.Api.DTOs.Workspace;

namespace Pdr.Api.Services.Interfaces;

public interface IWorkspaceService
{
    Task<IEnumerable<EngagementTypeDto>> GetEngagementTypesAsync();
    Task<EngagementInfoDto?> GetEngagementInfoAsync(long id);
    Task<bool> EditEngagementAsync(long id, EditEngagementRequest request);
}
