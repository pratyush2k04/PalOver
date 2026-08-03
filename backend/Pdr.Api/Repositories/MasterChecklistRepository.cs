using Microsoft.EntityFrameworkCore;
using Pdr.Api.Data;
using Pdr.Api.Models.MasterChecklist;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Repositories;

public class MasterChecklistRepository(AppDbContext context) : IMasterChecklistRepository
{
    public async Task<MasterChecklistVersion?> GetCurrentDraftVersionAsync()
    {
        return await context.MasterChecklistVersions
            .Include(v => v.Status)
            .Include(v => v.Items.OrderBy(i => i.DisplayOrder))
            .ThenInclude(i => i.Category)
            .Where(v => v.Status.StatusName == "Draft")
            .OrderByDescending(v => v.VersionNumber)
            .FirstOrDefaultAsync();
    }

    public async Task<MasterChecklistVersion?> GetPublishedVersionAsync()
    {
        return await context.MasterChecklistVersions
            .Include(v => v.Status)
            .Include(v => v.Items.OrderBy(i => i.DisplayOrder))
            .ThenInclude(i => i.Category)
            .Where(v => v.Status.StatusName == "Published")
            .OrderByDescending(v => v.VersionNumber)
            .FirstOrDefaultAsync();
    }

    public async Task<List<MasterChecklistVersion>> GetVersionHistoryAsync()
    {
        return await context.MasterChecklistVersions
            .Include(v => v.Status)
            .OrderByDescending(v => v.VersionNumber)
            .ToListAsync();
    }

    public async Task<MasterChecklistVersion?> GetVersionByIdAsync(long versionId)
    {
        return await context.MasterChecklistVersions
            .Include(v => v.Status)
            .Include(v => v.Items.OrderBy(i => i.DisplayOrder))
            .ThenInclude(i => i.Category)
            .FirstOrDefaultAsync(v => v.Id == versionId);
    }

    public async Task<List<MasterChecklistCategory>> GetCategoriesAsync()
    {
        return await context.MasterChecklistCategories
            .OrderBy(c => c.CategoryName)
            .ToListAsync();
    }

    public async Task<MasterChecklistCategory?> GetCategoryByIdAsync(long categoryId)
    {
        return await context.MasterChecklistCategories
            .FirstOrDefaultAsync(c => c.Id == categoryId);
    }

    public async Task<MasterChecklistItem?> GetChecklistItemAsync(long id)
    {
        return await context.MasterChecklistItems
            .Include(i => i.Version)
            .ThenInclude(v => v.Status)
            .Include(i => i.Category)
            .FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<MasterChecklistStatus?> GetStatusByNameAsync(string statusName)
    {
        return await context.MasterChecklistStatuses
            .FirstOrDefaultAsync(s => s.StatusName == statusName);
    }

    public async Task AddChecklistItemAsync(MasterChecklistItem item)
    {
        context.MasterChecklistItems.Add(item);
        await SaveChangesAsync();
    }

    public async Task UpdateChecklistItemAsync(MasterChecklistItem item)
    {
        context.MasterChecklistItems.Update(item);
        await SaveChangesAsync();
    }

    public async Task RetireChecklistItemAsync(MasterChecklistItem item)
    {
        item.IsRetired = true;
        context.MasterChecklistItems.Update(item);
        await SaveChangesAsync();
    }

    public async Task ReorderChecklistItemsAsync(List<MasterChecklistItem> items)
    {
        context.MasterChecklistItems.UpdateRange(items);
        await SaveChangesAsync();
    }

    public async Task PublishVersionAsync(MasterChecklistVersion draftVersion, MasterChecklistVersion newDraftVersion)
    {
        context.MasterChecklistVersions.Update(draftVersion);
        context.MasterChecklistVersions.Add(newDraftVersion);
        await SaveChangesAsync();
    }

    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }
}
