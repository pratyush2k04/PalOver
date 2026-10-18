using Microsoft.EntityFrameworkCore;
using Pdr.Api.Data;
using Pdr.Api.Models;
using Pdr.Api.Models.Engagements;
using Pdr.Api.Models.LoginEntity;
using Pdr.Api.Models.AccessTracker;
using Pdr.Api.DTOs.AccessTracker;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Repositories;

public class AccessTrackerRepository(AppDbContext context) : IAccessTrackerRepository
{
    // Fetch all access items for an engagement, ordered by definition display order then item id
    public async Task<List<AccessItem>> GetAccessItemsByEngagementIdAsync(int engagementId) =>
        await context.AccessItems
            .AsNoTracking()
            .Include(a => a.Owner)
            .Include(a => a.Engagement)
            .Include(a => a.AccessItemDefinition)
            .Where(a => a.EngagementId == engagementId)
            .OrderBy(a => a.AccessItemDefinition != null ? a.AccessItemDefinition.DisplayOrder : int.MaxValue)
            .ThenBy(a => a.Id)
            .ToListAsync();

    public async Task<AccessItem?> GetAccessItemByIdAsync(int id) =>
        await context.AccessItems
            .Include(a => a.Owner)
            .Include(a => a.Engagement)
            .FirstOrDefaultAsync(a => a.Id == id);

    public async Task<Dictionary<int, AccessItem>> GetAccessItemsByIdsAsync(IEnumerable<int> ids)
    {
        var idList = ids.Distinct().ToList();
        if (idList.Count == 0) return new();

        return await context.AccessItems
            .AsNoTracking()
            .Where(a => idList.Contains(a.Id))
            .ToDictionaryAsync(a => a.Id);
    }

    public async Task AddAccessItemAsync(AccessItem item)
    {
        await context.AccessItems.AddAsync(item);
    }

    public async Task SaveChangesAsync() =>
        await context.SaveChangesAsync();

    // Validation lookups
    public async Task<bool> EngagementExistsAsync(int engagementId) =>
        await context.Engagements.AnyAsync(e => e.EngagementId == engagementId);

    public async Task<bool> UserExistsAsync(int userId) =>
        await context.Users.AnyAsync(u => u.Id == userId);

    // Returns the full engagement for BillingStartDate + KickoffDate access (used in US3 evaluation)
    public async Task<Engagement?> GetEngagementAsync(int engagementId) =>
        await context.Engagements
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.EngagementId == engagementId);

    // Access items that have left the pending state (Granted or Blocked), for the KPI access-delay report
    public async Task<List<AccessItemDelayInfoDto>> GetResolvedAccessItemsByEngagementIdsAsync(List<int> engagementIds) =>
        await context.AccessItems
            .AsNoTracking()
            .Where(a => engagementIds.Contains(a.EngagementId) && a.Status != AccessItemStatus.InProgress)
            .Select(a => new AccessItemDelayInfoDto(a.Id, a.EngagementId, a.Status, a.CreatedAt, a.GrantedDate))
            .ToListAsync();
}
