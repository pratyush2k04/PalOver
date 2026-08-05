using Microsoft.EntityFrameworkCore;
using Pdr.Api.Data;
using Pdr.Api.Models.Cadence;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Repositories;

public class CommunicationCadenceRepository(AppDbContext context) : ICommunicationCadenceRepository
{
    public async Task<List<CommunicationCadence>> GetByEngagementIdAsync(int engagementId) =>
        await context.CommunicationCadences
            .AsNoTracking()
            .Where(c => c.EngagementId == engagementId)
            .OrderBy(c => c.CreatedAt)
            .ToListAsync();

    public async Task<CommunicationCadence> AddAsync(CommunicationCadence cadence)
    {
        context.CommunicationCadences.Add(cadence);
        await context.SaveChangesAsync();
        return cadence;
    }

    public async Task<bool> ExistsByRoleAsync(int engagementId, string stakeholderRole) =>
        await context.CommunicationCadences
            .AnyAsync(c => c.EngagementId == engagementId &&
                           c.StakeholderRole.ToLower() == stakeholderRole.ToLower());
}
