using Pdr.Api.DTOs.EngagementChecklist;

namespace Pdr.Api.Services.Interfaces;

public interface IEngagementChecklistService
{
    Task<EngagementChecklistResponseDto?> GetByEngagementIdAsync(int engagementId);
    Task<EngagementChecklistResponseDto> ApplyMasterChecklistToEngagementAsync(int engagementId);
}
