using Pdr.Api.DTOs.EngagementChecklist;

namespace Pdr.Api.Services.Interfaces;

public interface IEngagementChecklistService
{
    Task<EngagementChecklistResponseDto?> GetByEngagementIdAsync(int engagementId);
    Task<EngagementChecklistResponseDto> ApplyMasterChecklistToEngagementAsync(int engagementId);
    Task<EngagementChecklistResponseDto> UpdateItemStatusAsync(int engagementId, int itemId, UpdateItemStatusRequestDto dto);
    Task<EngagementChecklistResponseDto> UpdateItemApplicabilityAsync(int engagementId, int itemId, UpdateItemApplicabilityRequestDto dto);
    Task<EngagementChecklistResponseDto> ReviewExemptionAsync(int engagementId, int itemId, ReviewExemptionRequestDto dto);
}
