using Pdr.Api.DTOs.Engagements;

namespace Pdr.Api.Services.Interfaces;

public interface IEngagementService
{
    Task<MarkSowSignedResponse> MarkSowSignedAsync(MarkSowSignedRequest request);
    Task<IEnumerable<EngagementTypeDto>> GetEngagementTypesAsync();
    Task<EngagementInfoDto?> GetEngagementInfoAsync(long id);
    Task<bool> EditEngagementAsync(long id, EditEngagementRequest request);
}
