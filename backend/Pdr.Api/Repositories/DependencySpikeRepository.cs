using Microsoft.EntityFrameworkCore;
using Pdr.Api.Data;
using Pdr.Api.Models.DependencySpikes;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Repositories;

public class DependencySpikeRepository(AppDbContext context) : IDependencySpikeRepository
{
    public async Task<List<DependencySpike>> GetByEngagementIdAsync(int engagementId) =>
        await context.DependencySpikes
            .AsNoTracking()
            .Include(s => s.Owner)
            .Where(s => s.EngagementId == engagementId)
            .OrderBy(s => s.CreatedAt)
            .ToListAsync();

    public async Task<DependencySpike?> GetByIdAsync(int id) =>
        await context.DependencySpikes
            .Include(s => s.Owner)
            .FirstOrDefaultAsync(s => s.Id == id);

    public async Task AddAsync(DependencySpike spike)
    {
        await context.DependencySpikes.AddAsync(spike);
    }

    public async Task SaveChangesAsync() =>
        await context.SaveChangesAsync();

    public async Task<bool> EngagementExistsAsync(int engagementId) =>
        await context.Engagements.AnyAsync(e => e.EngagementId == engagementId);

    public async Task<bool> IsEngagementSignedOffAsync(int engagementId) =>
        await context.DayOneSignOffRecords.AsNoTracking().AnyAsync(r => r.EngagementId == engagementId);
}
