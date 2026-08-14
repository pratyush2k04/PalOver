using Pdr.Api.Models.Risks;

namespace Pdr.Api.Repositories.Interfaces;

public interface IRiskRepository
{
    Task<List<Risk>> GetByEngagementIdAsync(int engagementId, RiskStatus? status = null, int? ownerId = null);
    Task<Risk?> GetByIdAsync(int id);
    Task AddAsync(Risk risk);
    Task UpdateAsync(Risk risk);
    Task SaveChangesAsync();
}
