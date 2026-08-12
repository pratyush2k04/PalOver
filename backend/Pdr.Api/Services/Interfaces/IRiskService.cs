using Pdr.Api.DTOs.Risks;
using Pdr.Api.Models.Risks;

namespace Pdr.Api.Services.Interfaces;

public interface IRiskService
{
    Task<List<RiskDto>> GetRisksByEngagementAsync(int engagementId, RiskStatus? status = null, int? ownerId = null);
    Task<RiskDto> CreateRiskAsync(int engagementId, CreateRiskDto dto);
    Task<RiskDto> UpdateRiskAsync(int engagementId, int riskId, UpdateRiskDto dto);
    Task<RiskDto> CloseRiskAsync(int engagementId, int riskId, CloseRiskDto dto);
    Task<RiskDto> ReopenRiskAsync(int engagementId, int riskId);
    Task AutoCreateRiskFromBlockerAsync(int engagementId, string title, string blockReason, int? ownerId, string sourceType, int sourceItemId, string sourceTitle);
}
