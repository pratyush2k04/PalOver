using Pdr.Api.Models.Engagements;

namespace Pdr.Api.Repositories.Interfaces;

public interface IAssumptionRepository
{
    Task<List<Assumption>> GetByEngagementIdAsync(int engagementId);
    Task<Assumption?> GetByIdAsync(int id);
    Task<Assumption> AddAsync(Assumption assumption);
    Task<Assumption> UpdateAsync(Assumption assumption);
    Task DeleteAsync(Assumption assumption);
    Task<int> GetUnvalidatedCountByEngagementIdAsync(int engagementId);
    Task<int> GetInvalidWithoutRiskCountByEngagementIdAsync(int engagementId);
    Task<bool> IsEngagementSignedOffAsync(int engagementId);
}
