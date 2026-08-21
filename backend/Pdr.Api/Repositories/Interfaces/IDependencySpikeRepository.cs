using Pdr.Api.Models.DependencySpikes;

namespace Pdr.Api.Repositories.Interfaces;

public interface IDependencySpikeRepository
{
    Task<List<DependencySpike>> GetByEngagementIdAsync(int engagementId);
    Task<DependencySpike?> GetByIdAsync(int id);
    Task AddAsync(DependencySpike spike);
    Task SaveChangesAsync();
    Task<bool> EngagementExistsAsync(int engagementId);
}
