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
            .AsNoTracking()
            .Include(c => c.Items.OrderBy(i => i.DisplayOrder))
            .ThenInclude(i => i.Category)
            .Include(c => c.Items)
            .ThenInclude(i => i.Owner)
            .Include(c => c.Items)
            .ThenInclude(i => i.EvidenceSubmittedBy)
            .Include(c => c.Items)
            .ThenInclude(i => i.EvidenceDocument)
            .FirstOrDefaultAsync(c => c.EngagementId == engagementId);
    }

    public async Task<EngagementChecklistItem?> GetItemByIdAsync(int engagementId, int itemId)
    {
        return await context.EngagementChecklistItems
            .Include(i => i.Category)
            .Include(i => i.Owner)
            .Include(i => i.EvidenceSubmittedBy)
            .Include(i => i.EvidenceDocument)
            .FirstOrDefaultAsync(i => i.Id == itemId && i.EngagementChecklist.EngagementId == engagementId);
    }

    public async Task<Dictionary<int, EngagementChecklistItem>> GetItemsByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
    {
        var idList = ids.Distinct().ToList();
        if (idList.Count == 0) return new Dictionary<int, EngagementChecklistItem>();

        return await context.EngagementChecklistItems
            .AsNoTracking()
            .Where(i => idList.Contains(i.Id))
            .ToDictionaryAsync(i => i.Id, cancellationToken);
    }

    public async Task AddAsync(EngagementChecklist checklist)
    {
        await context.EngagementChecklists.AddAsync(checklist);
    }

    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }}
