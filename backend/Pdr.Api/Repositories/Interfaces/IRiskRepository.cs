using Pdr.Api.Models.Risk;

namespace Pdr.Api.Repositories.Interfaces;

public interface IRiskRepository
{
    Task<List<Risk>> GetByEngagementIdAsync(int engagementId);
    Task<Risk?> GetByIdAsync(int riskId);
    Task<Risk?> GetOpenByTitleAsync(int engagementId, string title);
    Task<Risk?> GetOpenRiskAsync(int engagementId, string title, int? accessItemId = null, int? environmentToolingItemId = null);
    Task AddAsync(Risk risk);
    Task SaveChangesAsync();

    // Validation lookup
    Task<bool> EngagementExistsAsync(int engagementId);
}
