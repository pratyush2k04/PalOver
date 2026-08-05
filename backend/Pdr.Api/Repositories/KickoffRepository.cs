using Microsoft.EntityFrameworkCore;
using Pdr.Api.Data;
using Pdr.Api.Models;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Repositories;

public class KickoffRepository(AppDbContext context) : IKickoffRepository
{
    public async Task<bool> EngagementExistsAsync(int engagementId)
    {
        return await context.Set<Engagement>().AnyAsync(e => e.EngagementId == engagementId);
    }

    public async Task<bool> UserHasAccessToEngagementAsync(int engagementId, int userId)
    {
        return await context.EngagementAssignments
            .AnyAsync(ea => ea.EngagementId == engagementId && ea.UserId == userId);
    }

    public async Task<KickoffRecord?> GetByEngagementIdAsync(int engagementId)
    {
        return await context.KickoffRecords
            .FirstOrDefaultAsync(k => k.EngagementId == engagementId);
    }

    public async Task AddAsync(KickoffRecord record)
    {
        await context.KickoffRecords.AddAsync(record);
    }

    public void Update(KickoffRecord record)
    {
        context.KickoffRecords.Update(record);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await context.SaveChangesAsync() > 0;
    }
}
