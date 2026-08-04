using Pdr.Api.DTOs.Engagements;

namespace Pdr.Api.Services.Interfaces;

public interface IEngagementService
{
    Task<MarkSowSignedResponse> MarkSowSignedAsync(MarkSowSignedRequest request);
    Task<IEnumerable<EngagementTypeDto>> GetEngagementTypesAsync();
    Task<IEnumerable<CommercialModelDto>> GetCommercialModelsAsync();
    Task<EngagementInfoDto?> GetEngagementInfoAsync(int id);
    Task<bool> EditEngagementAsync(int id, EditEngagementRequest request);
    Task<PmEngagementListResponse> GetMyEngagementsAsync(string sortBy);
    //assigning
    Task<EngagementOwnersDto> GetEngagementOwnersAsync(int engagementId);

    Task<EngagementOwnersDto> AssignProjectManagerAsync(int engagementId, int userId);
    Task<EngagementOwnersDto> AssignTechLeadAsync(int engagementId, int userId);
}
