using Pdr.Api.Models.MasterChecklist;

namespace Pdr.Api.Repositories.Interfaces;

public interface IChecklistRepository
{
    Task<ChecklistVersion?> GetCurrentDraftVersionAsync();
    Task<ChecklistVersion?> GetPublishedVersionAsync();
    Task<List<ChecklistVersion>> GetVersionHistoryAsync();
    Task<ChecklistVersion?> GetVersionByIdAsync(long versionId);
    Task<List<ChecklistCategory>> GetCategoriesAsync();
    Task<ChecklistCategory?> GetCategoryByIdAsync(long categoryId);
    Task<ChecklistItem?> GetChecklistItemAsync(long id);
    Task<ChecklistStatus?> GetStatusByNameAsync(string statusName);
    Task AddChecklistItemAsync(ChecklistItem item);
    Task UpdateChecklistItemAsync(ChecklistItem item);
    Task RetireChecklistItemAsync(ChecklistItem item);
    Task ReorderChecklistItemsAsync(List<ChecklistItem> items);
    Task PublishVersionAsync(ChecklistVersion draftVersion, ChecklistVersion newDraftVersion);
    Task SaveChangesAsync();
}
