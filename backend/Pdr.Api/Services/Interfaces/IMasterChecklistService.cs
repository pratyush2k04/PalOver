using Pdr.Api.DTOs.MasterChecklist;

namespace Pdr.Api.Services.Interfaces;

public interface IMasterChecklistService
{
    Task<MasterChecklistVersionResponseDto?> GetCurrentDraftVersionAsync();
    Task<MasterChecklistVersionResponseDto?> GetPublishedVersionAsync();
    Task<List<MasterChecklistVersionSummaryResponseDto>> GetVersionHistoryAsync();
    Task<ChecklistResultDto<MasterChecklistVersionResponseDto>> GetVersionByIdAsync(int versionId);
    Task<List<MasterChecklistCategoryResponseDto>> GetCategoriesAsync();
    Task<ChecklistResultDto<MasterChecklistItemResponseDto>> CreateChecklistItemAsync(MasterCreateChecklistItemRequestDto request);
    Task<ChecklistResultDto<MasterChecklistItemResponseDto>> UpdateChecklistItemAsync(int id, UpdateChecklistItemRequestDto request);
    Task<ChecklistResultDto<MasterChecklistItemResponseDto>> RetireChecklistItemAsync(int id);
    Task<ChecklistResultDto<List<MasterChecklistItemResponseDto>>> ReorderChecklistItemsAsync(ReorderMasterChecklistItemsRequestDto request);
    Task<ChecklistResultDto<MasterChecklistVersionResponseDto>> PublishVersionAsync(int versionId);
}

public class ChecklistResultDto<T>
{
    public bool IsSuccess { get; set; }
    public bool IsNotFound { get; set; }
    public T? Data { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;

    public static ChecklistResultDto<T> Success(T data) => new()
    {
        IsSuccess = true,
        Data = data
    };

    public static ChecklistResultDto<T> Failure(string errorMessage, bool isNotFound = false) => new()
    {
        IsSuccess = false,
        IsNotFound = isNotFound,
        ErrorMessage = errorMessage
    };
}
