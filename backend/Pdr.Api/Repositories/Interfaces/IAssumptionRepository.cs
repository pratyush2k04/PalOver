using Pdr.Api.Models.Engagements;

namespace Pdr.Api.Repositories.Interfaces;

public interface IAssumptionRepository
{
    Task<List<Assumption>> GetByEngagementIdAsync(int engagementId);
    Task<Assumption?> GetByIdAsync(int id);
    Task<Dictionary<int, Assumption>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default);
    Task<Assumption> AddAsync(Assumption assumption);
    Task<Assumption> UpdateAsync(Assumption assumption);
    Task DeleteAsync(Assumption assumption);
    Task<int> GetUnvalidatedCountByEngagementIdAsync(int engagementId);
}
