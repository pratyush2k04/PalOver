using Pdr.Api.Models.MasterChecklist;

namespace Pdr.Api.Repositories.Interfaces;

public interface IMasterChecklistRepository
{
    Task<MasterChecklistVersion?> GetCurrentDraftVersionAsync();
    Task<MasterChecklistVersion?> GetPublishedVersionAsync();
    Task<List<MasterChecklistVersion>> GetVersionHistoryAsync();
    Task<MasterChecklistVersion?> GetVersionByIdAsync(int versionId);
    Task<List<MasterChecklistCategory>> GetCategoriesAsync();
    Task<MasterChecklistCategory?> GetCategoryByIdAsync(int categoryId);
    Task<MasterChecklistItem?> GetChecklistItemAsync(int id);
    Task<MasterChecklistStatus?> GetStatusByNameAsync(string statusName);
    Task AddChecklistItemAsync(MasterChecklistItem item);
    Task UpdateChecklistItemAsync(MasterChecklistItem item);
    Task RetireChecklistItemAsync(MasterChecklistItem item);
    Task ReorderChecklistItemsAsync(List<MasterChecklistItem> items);
    Task PublishVersionAsync(MasterChecklistVersion draftVersion, MasterChecklistVersion newDraftVersion);
    Task SaveChangesAsync();
}
