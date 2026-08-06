using Microsoft.EntityFrameworkCore;
using Pdr.Api.Data;
using Pdr.Api.Models;
using Pdr.Api.Models.LoginEntity;
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
            .Include(k => k.ActionItems)
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

    public async Task<IEnumerable<KickoffActionItem>> GetOpenActionItemsByEngagementIdAsync(int engagementId)
    {
        return await context.KickoffActionItems
            .Where(a => a.EngagementId == engagementId && a.Status != KickoffActionItemStatus.Done)
            .OrderBy(a => a.DueDate)
            .ToListAsync();
    }

    public async Task<KickoffActionItem?> GetActionItemByIdAsync(int actionItemId)
    {
        return await context.KickoffActionItems
            .FirstOrDefaultAsync(a => a.KickoffActionItemId == actionItemId);
    }

    public async Task AddActionItemAsync(KickoffActionItem item)
    {
        await context.KickoffActionItems.AddAsync(item);
    }

    public void UpdateActionItem(KickoffActionItem item)
    {
        context.KickoffActionItems.Update(item);
    }

    public void DeleteActionItem(KickoffActionItem item)
    {
        item.IsDeleted = true;
        context.KickoffActionItems.Update(item);
    }

    public async Task<User?> GetUserByIdAsync(int userId)
    {
        return await context.Users.FirstOrDefaultAsync(u => u.Id == userId);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await context.SaveChangesAsync() > 0;
    }
}
