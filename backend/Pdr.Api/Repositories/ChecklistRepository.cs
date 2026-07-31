using Microsoft.EntityFrameworkCore;
using Pdr.Api.Data;
using Pdr.Api.Models.MasterChecklist;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Repositories;

public class ChecklistRepository(AppDbContext context) : IChecklistRepository
{
    public async Task<ChecklistVersion?> GetCurrentDraftVersionAsync()
    {
        return await context.ChecklistVersions
            .Include(v => v.Status)
            .Include(v => v.Items.OrderBy(i => i.DisplayOrder))
            .ThenInclude(i => i.Category)
            .Where(v => v.Status.StatusName == "Draft")
            .OrderByDescending(v => v.VersionNumber)
            .FirstOrDefaultAsync();
    }

    public async Task<ChecklistVersion?> GetPublishedVersionAsync()
    {
        return await context.ChecklistVersions
            .Include(v => v.Status)
            .Include(v => v.Items.OrderBy(i => i.DisplayOrder))
            .ThenInclude(i => i.Category)
            .Where(v => v.Status.StatusName == "Published")
            .OrderByDescending(v => v.VersionNumber)
            .FirstOrDefaultAsync();
    }

    public async Task<List<ChecklistVersion>> GetVersionHistoryAsync()
    {
        return await context.ChecklistVersions
            .Include(v => v.Status)
            .OrderByDescending(v => v.VersionNumber)
            .ToListAsync();
    }

    public async Task<ChecklistVersion?> GetVersionByIdAsync(long versionId)
    {
        return await context.ChecklistVersions
            .Include(v => v.Status)
            .Include(v => v.Items.OrderBy(i => i.DisplayOrder))
            .ThenInclude(i => i.Category)
            .FirstOrDefaultAsync(v => v.Id == versionId);
    }

    public async Task<List<ChecklistCategory>> GetCategoriesAsync()
    {
        return await context.ChecklistCategories
            .OrderBy(c => c.CategoryName)
            .ToListAsync();
    }

    public async Task<ChecklistCategory?> GetCategoryByIdAsync(long categoryId)
    {
        return await context.ChecklistCategories
            .FirstOrDefaultAsync(c => c.Id == categoryId);
    }

    public async Task<ChecklistItem?> GetChecklistItemAsync(long id)
    {
        return await context.ChecklistItems
            .Include(i => i.Version)
            .ThenInclude(v => v.Status)
            .Include(i => i.Category)
            .FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<ChecklistStatus?> GetStatusByNameAsync(string statusName)
    {
        return await context.ChecklistStatuses
            .FirstOrDefaultAsync(s => s.StatusName == statusName);
    }

    public async Task AddChecklistItemAsync(ChecklistItem item)
    {
        context.ChecklistItems.Add(item);
        await SaveChangesAsync();
    }

    public async Task UpdateChecklistItemAsync(ChecklistItem item)
    {
        context.ChecklistItems.Update(item);
        await SaveChangesAsync();
    }

    public async Task RetireChecklistItemAsync(ChecklistItem item)
    {
        item.IsRetired = true;
        context.ChecklistItems.Update(item);
        await SaveChangesAsync();
    }

    public async Task ReorderChecklistItemsAsync(List<ChecklistItem> items)
    {
        context.ChecklistItems.UpdateRange(items);
        await SaveChangesAsync();
    }

    public async Task PublishVersionAsync(ChecklistVersion draftVersion, ChecklistVersion newDraftVersion)
    {
        context.ChecklistVersions.Update(draftVersion);
        context.ChecklistVersions.Add(newDraftVersion);
        await SaveChangesAsync();
    }

    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }
}
