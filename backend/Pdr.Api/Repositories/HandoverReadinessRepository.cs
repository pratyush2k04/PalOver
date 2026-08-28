using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pdr.Api.Data;
using Pdr.Api.Models.HandoverReadiness;
using Pdr.Api.Models.LoginEntity;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Repositories;

public class HandoverReadinessRepository(AppDbContext context) : IHandoverReadinessRepository
{
    public async Task<List<HandoverReadinessItem>> GetItemsByEngagementIdAsync(int engagementId)
    {
        return await context.HandoverReadinessItems
            .Include(h => h.Owner)
            .Include(h => h.HandoverReadinessDefinition)
            .Where(h => h.EngagementId == engagementId && !h.IsDeleted)
            .OrderBy(h => h.HandoverReadinessDefinition != null ? h.HandoverReadinessDefinition.DisplayOrder : int.MaxValue)
            .ThenBy(h => h.Id)
            .ToListAsync();
    }

    public async Task<HandoverReadinessItem?> GetByIdAsync(int id)
    {
        return await context.HandoverReadinessItems
            .Include(h => h.Owner)
            .Include(h => h.HandoverReadinessDefinition)
            .FirstOrDefaultAsync(h => h.Id == id && !h.IsDeleted);
    }

    public async Task<List<HandoverReadinessDefinition>> GetActiveDefinitionsAsync()
    {
        return await context.HandoverReadinessDefinitions
            .Where(d => d.IsActive)
            .OrderBy(d => d.DisplayOrder)
            .ToListAsync();
    }

    public async Task AddItemAsync(HandoverReadinessItem item)
    {
        await context.HandoverReadinessItems.AddAsync(item);
    }

    public async Task AddItemsAsync(IEnumerable<HandoverReadinessItem> items)
    {
        await context.HandoverReadinessItems.AddRangeAsync(items);
    }

    public async Task<bool> EngagementExistsAsync(int engagementId)
    {
        return await context.Engagements.AnyAsync(e => e.EngagementId == engagementId && !e.IsDeleted);
    }

    public async Task<int?> GetPreSalesOwnerIdAsync(int engagementId)
    {
        var preSalesAssignment = await context.EngagementAssignments
            .FirstOrDefaultAsync(a => a.EngagementId == engagementId && a.User.RoleId == (int)RoleEnum.PreSales);

        if (preSalesAssignment != null)
            return preSalesAssignment.UserId;

        var preSalesUser = await context.Users.FirstOrDefaultAsync(u => u.RoleId == (int)RoleEnum.PreSales);
        if (preSalesUser != null)
            return preSalesUser.Id;

        return 1;
    }

    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }
}
