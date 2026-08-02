using Pdr.Api.DTOs.MasterChecklist;

namespace Pdr.Api.Services.Interfaces;

public interface IChecklistService
{
    Task<ChecklistVersionResponseDto?> GetCurrentDraftVersionAsync();
    Task<ChecklistVersionResponseDto?> GetPublishedVersionAsync();
    Task<List<ChecklistVersionSummaryResponseDto>> GetVersionHistoryAsync();
    Task<ChecklistResultDto<ChecklistVersionResponseDto>> GetVersionByIdAsync(long versionId);
    Task<List<ChecklistCategoryResponseDto>> GetCategoriesAsync();
    Task<ChecklistResultDto<ChecklistItemResponseDto>> CreateChecklistItemAsync(CreateChecklistItemRequestDto request);
    Task<ChecklistResultDto<ChecklistItemResponseDto>> UpdateChecklistItemAsync(long id, UpdateChecklistItemRequestDto request);
    Task<ChecklistResultDto<ChecklistItemResponseDto>> RetireChecklistItemAsync(long id);
    Task<ChecklistResultDto<List<ChecklistItemResponseDto>>> ReorderChecklistItemsAsync(ReorderChecklistItemsRequestDto request);
    Task<ChecklistResultDto<ChecklistVersionResponseDto>> PublishVersionAsync(long versionId);
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
