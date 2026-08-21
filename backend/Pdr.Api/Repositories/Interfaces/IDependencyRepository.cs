using Pdr.Api.Models.Engagements.Dependencies;

namespace Pdr.Api.Repositories.Interfaces;

public interface IDependencyRepository
{
    Task<IEnumerable<Dependency>> GetByEngagementIdAsync(int engagementId);
    Task<Dependency?> GetByIdAsync(int id);
    Task AddAsync(Dependency dependency);
    void Update(Dependency dependency);
    void Delete(Dependency dependency);
    Task<int> SaveChangesAsync();
}
