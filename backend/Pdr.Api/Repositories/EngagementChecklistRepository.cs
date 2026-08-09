using Microsoft.EntityFrameworkCore;
using Pdr.Api.Data;
using Pdr.Api.Models.EngagementChecklist;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Repositories;

public class EngagementChecklistRepository(AppDbContext context) : IEngagementChecklistRepository
{
    public async Task<EngagementChecklist?> GetByEngagementIdAsync(int engagementId)
    {
        return await context.EngagementChecklists
            .Include(c => c.Items.OrderBy(i => i.DisplayOrder))
            .ThenInclude(i => i.Category)
            .FirstOrDefaultAsync(c => c.EngagementId == engagementId);
    }

    public async Task<EngagementChecklistItem?> GetItemByIdAsync(int engagementId, int itemId)
    {
        return await context.EngagementChecklistItems
            .Include(i => i.Category)
            .FirstOrDefaultAsync(i => i.Id == itemId && i.EngagementChecklist.EngagementId == engagementId);
    }

    public async Task AddAsync(EngagementChecklist checklist)
    {
        await context.EngagementChecklists.AddAsync(checklist);
    }

    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }
}
