using Microsoft.EntityFrameworkCore;
using Pdr.Api.Data;
using Pdr.Api.Models;
using Pdr.Api.Models.AccessTracker;
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
            .ThenBy(a => a.AccessItemId)
            .ToListAsync();

    // Fetch a single access item with owner info for status/owner updates
    public async Task<AccessItem?> GetAccessItemByIdAsync(int accessItemId) =>
        await context.AccessItems
            .Include(a => a.Owner)
            .Include(a => a.Engagement)
            .FirstOrDefaultAsync(a => a.AccessItemId == accessItemId);

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

    // Returns the UserId of the Tech Lead assigned to this engagement (for US3 deadline calculation)
    public async Task<int?> GetTechLeadUserIdAsync(int engagementId) =>
        await context.EngagementAssignments
            .AsNoTracking()
            .Include(ea => ea.User)
            .Where(ea => ea.EngagementId == engagementId && ea.User.RoleId == (int)RoleEnum.TechLead)
            .Select(ea => (int?)ea.UserId)
            .FirstOrDefaultAsync();
}
