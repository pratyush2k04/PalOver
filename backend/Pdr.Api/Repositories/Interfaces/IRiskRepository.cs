using Pdr.Api.Models.Risks;

namespace Pdr.Api.Repositories.Interfaces;

public interface IRiskRepository
{
    Task<List<Risk>> GetByEngagementIdAsync(int engagementId, RiskStatus? status = null, int? ownerId = null);
    Task<Risk?> GetByIdAsync(int id);
    Task AddAsync(Risk risk);
    Task UpdateAsync(Risk risk);
    Task SaveChangesAsync();

    Task<List<RiskBlockerSourceItem>> GetActiveAccessBlockersAsync(int engagementId);
    Task<List<RiskBlockerSourceItem>> GetActiveToolingBlockersAsync(int engagementId);
    Task<List<RiskBlockerSourceItem>> GetActiveChecklistBlockersAsync(int engagementId);
    Task<List<RiskBlockerSourceItem>> GetActiveHandoverReadinessBlockersAsync(int engagementId);
    Task<RiskBlockerSourceItem?> GetActiveResourceGapBlockerAsync(int engagementId, int thresholdDays);
    Task<List<Risk>> GetOpenRiskBlockersAsync(int engagementId);
    Task<List<Risk>> GetAutoCreatedRisksAsync(int engagementId);

    Task<List<Risk>> GetOpenRisksAsync();
    Task<List<RiskEscalationHistory>> GetEscalationHistoriesByRiskIdsAsync(IEnumerable<int> riskIds);
    Task<List<RiskEscalationHistory>> GetEscalationHistoryByRiskIdAsync(int riskId);
    Task AddEscalationHistoryAsync(RiskEscalationHistory history);}
