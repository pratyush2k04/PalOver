using Pdr.Api.DTOs.MasterChecklist;
using Pdr.Api.Models.MasterChecklist;
using Pdr.Api.Repositories.Interfaces;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Services;

public class ChecklistService(IChecklistRepository checklistRepository) : IChecklistService
{
    private const string DraftStatusName = "Draft";
    private const string PublishedStatusName = "Published";

    public async Task<ChecklistVersionResponseDto?> GetCurrentDraftVersionAsync()
    {
        var draftVersion = await checklistRepository.GetCurrentDraftVersionAsync();
        return draftVersion is null ? null : MapVersion(draftVersion);
    }

    public async Task<ChecklistVersionResponseDto?> GetPublishedVersionAsync()
    {
        var publishedVersion = await checklistRepository.GetPublishedVersionAsync();
        return publishedVersion is null ? null : MapVersion(publishedVersion);
    }

    public async Task<List<ChecklistVersionSummaryResponseDto>> GetVersionHistoryAsync()
    {
        var versions = await checklistRepository.GetVersionHistoryAsync();
        return versions.Select(MapVersionSummary).ToList();
    }

    public async Task<ChecklistResultDto<ChecklistVersionResponseDto>> GetVersionByIdAsync(long versionId)
    {
        var version = await checklistRepository.GetVersionByIdAsync(versionId);
        if (version is null)
        {
            return ChecklistResultDto<ChecklistVersionResponseDto>.Failure("Checklist version not found.", isNotFound: true);
        }

        return ChecklistResultDto<ChecklistVersionResponseDto>.Success(MapVersion(version));
    }

    public async Task<List<ChecklistCategoryResponseDto>> GetCategoriesAsync()
    {
        var categories = await checklistRepository.GetCategoriesAsync();
        return categories.Select(c => new ChecklistCategoryResponseDto
        {
            Id = c.Id,
            CategoryName = c.CategoryName
        }).ToList();
    }

    public async Task<ChecklistResultDto<ChecklistItemResponseDto>> CreateChecklistItemAsync(CreateChecklistItemRequestDto request)
    {
        var draftVersion = await checklistRepository.GetCurrentDraftVersionAsync();
        if (draftVersion is null)
        {
            return ChecklistResultDto<ChecklistItemResponseDto>.Failure("No Draft version exists to add items to.");
        }

        var category = await checklistRepository.GetCategoryByIdAsync(request.CategoryId);
        if (category is null)
        {
            return ChecklistResultDto<ChecklistItemResponseDto>.Failure("Selected category does not exist.");
        }

        var item = new ChecklistItem
        {
            VersionId = draftVersion.Id,
            Title = request.Title,
            Description = request.Description,
            CategoryId = request.CategoryId,
            IsMandatory = request.IsMandatory,
            DisplayOrder = request.DisplayOrder,
            IsRetired = false
        };

        await checklistRepository.AddChecklistItemAsync(item);

        item.Category = category;
        return ChecklistResultDto<ChecklistItemResponseDto>.Success(MapItem(item));
    }

    public async Task<ChecklistResultDto<ChecklistItemResponseDto>> UpdateChecklistItemAsync(long id, UpdateChecklistItemRequestDto request)
    {
        var item = await checklistRepository.GetChecklistItemAsync(id);
        if (item is null)
        {
            return ChecklistResultDto<ChecklistItemResponseDto>.Failure("Checklist item not found.", isNotFound: true);
        }

        if (item.Version.Status.StatusName != DraftStatusName)
        {
            return ChecklistResultDto<ChecklistItemResponseDto>.Failure("Only items on the Draft version can be edited.");
        }

        var category = await checklistRepository.GetCategoryByIdAsync(request.CategoryId);
        if (category is null)
        {
            return ChecklistResultDto<ChecklistItemResponseDto>.Failure("Selected category does not exist.");
        }

        item.Title = request.Title;
        item.Description = request.Description;
        item.CategoryId = request.CategoryId;
        item.IsMandatory = request.IsMandatory;

        await checklistRepository.UpdateChecklistItemAsync(item);

        item.Category = category;
        return ChecklistResultDto<ChecklistItemResponseDto>.Success(MapItem(item));
    }

    public async Task<ChecklistResultDto<ChecklistItemResponseDto>> RetireChecklistItemAsync(long id)
    {
        var item = await checklistRepository.GetChecklistItemAsync(id);
        if (item is null)
        {
            return ChecklistResultDto<ChecklistItemResponseDto>.Failure("Checklist item not found.", isNotFound: true);
        }

        if (item.Version.Status.StatusName != DraftStatusName)
        {
            return ChecklistResultDto<ChecklistItemResponseDto>.Failure("Only items on the Draft version can be retired.");
        }

        await checklistRepository.RetireChecklistItemAsync(item);

        return ChecklistResultDto<ChecklistItemResponseDto>.Success(MapItem(item));
    }

    public async Task<ChecklistResultDto<List<ChecklistItemResponseDto>>> ReorderChecklistItemsAsync(ReorderChecklistItemsRequestDto request)
    {
        var draftVersion = await checklistRepository.GetCurrentDraftVersionAsync();
        if (draftVersion is null)
        {
            return ChecklistResultDto<List<ChecklistItemResponseDto>>.Failure("No Draft version exists to reorder.");
        }

        var draftItemIds = draftVersion.Items.Select(i => i.Id).ToHashSet();
        var requestedIds = request.Items.Select(i => i.ItemId).ToList();

        if (requestedIds.Any(itemId => !draftItemIds.Contains(itemId)))
        {
            return ChecklistResultDto<List<ChecklistItemResponseDto>>.Failure("One or more items do not belong to the current Draft version.");
        }

        if (requestedIds.Count != draftItemIds.Count || requestedIds.Distinct().Count() != requestedIds.Count)
        {
            return ChecklistResultDto<List<ChecklistItemResponseDto>>.Failure("Reorder request must include every item in the current Draft version exactly once.");
        }

        if (request.Items.Select(i => i.DisplayOrder).Distinct().Count() != request.Items.Count)
        {
            return ChecklistResultDto<List<ChecklistItemResponseDto>>.Failure("Display order values must be unique.");
        }

        var orderByItemId = request.Items.ToDictionary(i => i.ItemId, i => i.DisplayOrder);
        var itemsToUpdate = draftVersion.Items
            .Where(i => orderByItemId.ContainsKey(i.Id))
            .ToList();

        foreach (var item in itemsToUpdate)
        {
            item.DisplayOrder = orderByItemId[item.Id];
        }

        await checklistRepository.ReorderChecklistItemsAsync(itemsToUpdate);

        var orderedItems = draftVersion.Items.OrderBy(i => i.DisplayOrder).Select(MapItem).ToList();
        return ChecklistResultDto<List<ChecklistItemResponseDto>>.Success(orderedItems);
    }

    public async Task<ChecklistResultDto<ChecklistVersionResponseDto>> PublishVersionAsync(long versionId)
    {
        var draftVersion = await checklistRepository.GetVersionByIdAsync(versionId);
        if (draftVersion is null)
        {
            return ChecklistResultDto<ChecklistVersionResponseDto>.Failure("Checklist version not found.", isNotFound: true);
        }

        if (draftVersion.Status.StatusName != DraftStatusName)
        {
            return ChecklistResultDto<ChecklistVersionResponseDto>.Failure("Only a Draft version can be published.");
        }

        var publishedStatus = await checklistRepository.GetStatusByNameAsync(PublishedStatusName);
        var draftStatus = await checklistRepository.GetStatusByNameAsync(DraftStatusName);
        if (publishedStatus is null || draftStatus is null)
        {
            return ChecklistResultDto<ChecklistVersionResponseDto>.Failure("Checklist statuses are not configured correctly.");
        }

        draftVersion.StatusId = publishedStatus.Id;
        draftVersion.Status = publishedStatus;

        var newDraftVersion = new ChecklistVersion
        {
            VersionNumber = draftVersion.VersionNumber + 1,
            StatusId = draftStatus.Id,
            Status = draftStatus,
            Items = draftVersion.Items
                .Where(i => !i.IsRetired)
                .Select(i => new ChecklistItem
                {
                    Title = i.Title,
                    Description = i.Description,
                    CategoryId = i.CategoryId,
                    Category = i.Category,
                    IsMandatory = i.IsMandatory,
                    DisplayOrder = i.DisplayOrder,
                    IsRetired = false
                })
                .ToList()
        };

        await checklistRepository.PublishVersionAsync(draftVersion, newDraftVersion);

        return ChecklistResultDto<ChecklistVersionResponseDto>.Success(MapVersion(draftVersion));
    }

    private static ChecklistVersionResponseDto MapVersion(ChecklistVersion version)
    {
        return new ChecklistVersionResponseDto
        {
            Id = version.Id,
            VersionNumber = version.VersionNumber,
            StatusId = version.StatusId,
            StatusName = version.Status.StatusName,
            Items = version.Items.OrderBy(i => i.DisplayOrder).Select(MapItem).ToList()
        };
    }

    private static ChecklistVersionSummaryResponseDto MapVersionSummary(ChecklistVersion version)
    {
        return new ChecklistVersionSummaryResponseDto
        {
            Id = version.Id,
            VersionNumber = version.VersionNumber,
            StatusId = version.StatusId,
            StatusName = version.Status.StatusName,
            CreatedAt = version.CreatedAt,
            UpdatedAt = version.UpdatedAt
        };
    }

    private static ChecklistItemResponseDto MapItem(ChecklistItem item)
    {
        return new ChecklistItemResponseDto
        {
            Id = item.Id,
            VersionId = item.VersionId,
            Title = item.Title,
            Description = item.Description,
            CategoryId = item.CategoryId,
            CategoryName = item.Category.CategoryName,
            IsMandatory = item.IsMandatory,
            DisplayOrder = item.DisplayOrder,
            IsRetired = item.IsRetired
        };
    }
}
