using Microsoft.EntityFrameworkCore;
using Pdr.Api.Data;
using Pdr.Api.Models.Engagements.Dependencies;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Repositories;

public class DependencyRepository(AppDbContext context) : IDependencyRepository
{
    public async Task<IEnumerable<Dependency>> GetByEngagementIdAsync(int engagementId)
    {
        return await context.Dependencies
            .Include(d => d.Owner)
            .Include(d => d.LinkedRisk)
            .Where(d => d.EngagementId == engagementId)
            .OrderBy(d => d.NeededByDate)
            .ToListAsync();
    }

    public async Task<Dependency?> GetByIdAsync(int id)
    {
        return await context.Dependencies
            .Include(d => d.Owner)
            .Include(d => d.LinkedRisk)
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task AddAsync(Dependency dependency)
    {
        await context.Dependencies.AddAsync(dependency);
    }

    public void Update(Dependency dependency)
    {
        context.Dependencies.Update(dependency);
    }

    public void Delete(Dependency dependency)
    {
        context.Dependencies.Remove(dependency);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await context.SaveChangesAsync();
    }
}
