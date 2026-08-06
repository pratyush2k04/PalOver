using Pdr.Api.DTOs.Risk;

namespace Pdr.Api.Services.Interfaces;

public interface IRiskService
{
    // Automated Integration Hooks
    Task CreateRiskAsync(int engagementId, string title, string description, int? accessItemId = null, int? environmentToolingItemId = null);
    Task ResolveRiskAsync(int engagementId, string title, int? accessItemId = null, int? environmentToolingItemId = null);
    Task CreateClientSideRiskAsync(int engagementId, string accessItemName, int? accessItemId = null);

    // API Service Methods
    Task<List<RiskDto>> GetRisksByEngagementIdAsync(int engagementId);
    Task<RiskDto> CreateManualRiskAsync(int engagementId, CreateRiskRequest request);
    Task<RiskDto> UpdateRiskStatusAsync(int riskId, UpdateRiskStatusRequest request);
}
