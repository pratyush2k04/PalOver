using Pdr.Api.DTOs.BrownField;

namespace Pdr.Api.Services.Interfaces;

public interface IBrownfieldFindingService
{
    Task<BrownfieldReadinessResponse> GetFindingsByEngagementIdAsync(int engagementId);
    Task<BrownfieldFindingDto> CreateFindingAsync(int engagementId, CreateBrownfieldFindingRequest request);
    Task<BrownfieldFindingDto> UpdateClientDecisionAsync(int findingId, UpdateClientDecisionRequest request);
    Task DeleteFindingAsync(int findingId);
}
