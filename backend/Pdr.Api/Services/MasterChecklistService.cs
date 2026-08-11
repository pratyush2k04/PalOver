using Pdr.Api.Constants;
using Pdr.Api.DTOs.MasterChecklist;
using Pdr.Api.Models.MasterChecklist;
using Pdr.Api.Repositories.Interfaces;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Services;

public class MasterChecklistService(IMasterChecklistRepository checklistRepository) : IMasterChecklistService
{
    public async Task<MasterChecklistVersionResponseDto?> GetCurrentDraftVersionAsync()
    {
        var draftVersion = await checklistRepository.GetCurrentDraftVersionAsync();
        return draftVersion is null ? null : MapVersion(draftVersion);
    }

    public async Task<MasterChecklistVersionResponseDto?> GetPublishedVersionAsync()
    {
        var publishedVersion = await checklistRepository.GetPublishedVersionAsync();
        return publishedVersion is null ? null : MapVersion(publishedVersion);
    }

    public async Task<List<MasterChecklistVersionSummaryResponseDto>> GetVersionHistoryAsync()
    {
        var versions = await checklistRepository.GetVersionHistoryAsync();
        return versions.Select(MapVersionSummary).ToList();
    }

    public async Task<ChecklistResultDto<MasterChecklistVersionResponseDto>> GetVersionByIdAsync(int versionId)
    {
        var version = await checklistRepository.GetVersionByIdAsync(versionId);
        if (version is null)
        {
            return ChecklistResultDto<MasterChecklistVersionResponseDto>.Failure(MasterChecklistConstants.Messages.VersionNotFound, isNotFound: true);
        }

        return ChecklistResultDto<MasterChecklistVersionResponseDto>.Success(MapVersion(version));
    }

    public async Task<List<MasterChecklistCategoryResponseDto>> GetCategoriesAsync()
    {
        var categories = await checklistRepository.GetCategoriesAsync();
        return categories.Select(c => new MasterChecklistCategoryResponseDto
        {
            Id = c.Id,
            CategoryName = c.CategoryName
        }).ToList();
    }

    public async Task<ChecklistResultDto<MasterChecklistItemResponseDto>> CreateChecklistItemAsync(MasterCreateChecklistItemRequestDto request)
    {
        var draftVersion = await checklistRepository.GetCurrentDraftVersionAsync();
        if (draftVersion is null)
        {
            return ChecklistResultDto<MasterChecklistItemResponseDto>.Failure(MasterChecklistConstants.Messages.NoDraftToAddItems);
        }

        var category = await checklistRepository.GetCategoryByIdAsync(request.CategoryId);
        if (category is null)
        {
            return ChecklistResultDto<MasterChecklistItemResponseDto>.Failure(MasterChecklistConstants.Messages.CategoryNotFound);
        }

        var item = new MasterChecklistItem
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
        return ChecklistResultDto<MasterChecklistItemResponseDto>.Success(MapItem(item));
    }

    public async Task<ChecklistResultDto<MasterChecklistItemResponseDto>> UpdateChecklistItemAsync(int id, UpdateChecklistItemRequestDto request)
    {
        var item = await checklistRepository.GetChecklistItemAsync(id);
        if (item is null)
        {
            return ChecklistResultDto<MasterChecklistItemResponseDto>.Failure(MasterChecklistConstants.Messages.ItemNotFound, isNotFound: true);
        }

        if (item.Version.Status.StatusName != MasterChecklistConstants.StatusNames.Draft)
        {
            return ChecklistResultDto<MasterChecklistItemResponseDto>.Failure(MasterChecklistConstants.Messages.OnlyDraftEditable);
        }

        var category = await checklistRepository.GetCategoryByIdAsync(request.CategoryId);
        if (category is null)
        {
            return ChecklistResultDto<MasterChecklistItemResponseDto>.Failure(MasterChecklistConstants.Messages.CategoryNotFound);
        }

        item.Title = request.Title;
        item.Description = request.Description;
        item.CategoryId = request.CategoryId;
        item.IsMandatory = request.IsMandatory;

        await checklistRepository.UpdateChecklistItemAsync(item);

        item.Category = category;
        return ChecklistResultDto<MasterChecklistItemResponseDto>.Success(MapItem(item));
    }

    public async Task<ChecklistResultDto<MasterChecklistItemResponseDto>> RetireChecklistItemAsync(int id)
    {
        var item = await checklistRepository.GetChecklistItemAsync(id);
        if (item is null)
        {
            return ChecklistResultDto<MasterChecklistItemResponseDto>.Failure(MasterChecklistConstants.Messages.ItemNotFound, isNotFound: true);
        }

        if (item.Version.Status.StatusName != MasterChecklistConstants.StatusNames.Draft)
        {
            return ChecklistResultDto<MasterChecklistItemResponseDto>.Failure(MasterChecklistConstants.Messages.OnlyDraftRetirable);
        }

        await checklistRepository.RetireChecklistItemAsync(item);

        return ChecklistResultDto<MasterChecklistItemResponseDto>.Success(MapItem(item));
    }

    public async Task<ChecklistResultDto<List<MasterChecklistItemResponseDto>>> ReorderChecklistItemsAsync(ReorderMasterChecklistItemsRequestDto request)
    {
        var draftVersion = await checklistRepository.GetCurrentDraftVersionAsync();
        if (draftVersion is null)
        {
            return ChecklistResultDto<List<MasterChecklistItemResponseDto>>.Failure(MasterChecklistConstants.Messages.NoDraftToReorder);
        }

        var draftItemIds = draftVersion.Items.Select(i => i.Id).ToHashSet();
        var requestedIds = request.Items.Select(i => i.ItemId).ToList();

        if (requestedIds.Any(itemId => !draftItemIds.Contains(itemId)))
        {
            return ChecklistResultDto<List<MasterChecklistItemResponseDto>>.Failure(MasterChecklistConstants.Messages.ItemDoesNotBelongToDraft);
        }

        if (requestedIds.Count != draftItemIds.Count || requestedIds.Distinct().Count() != requestedIds.Count)
        {
            return ChecklistResultDto<List<MasterChecklistItemResponseDto>>.Failure(MasterChecklistConstants.Messages.ReorderMustIncludeAllItems);
        }

        if (request.Items.Select(i => i.DisplayOrder).Distinct().Count() != request.Items.Count)
        {
            return ChecklistResultDto<List<MasterChecklistItemResponseDto>>.Failure(MasterChecklistConstants.Messages.DisplayOrderMustBeUnique);
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
        return ChecklistResultDto<List<MasterChecklistItemResponseDto>>.Success(orderedItems);
    }

    public async Task<ChecklistResultDto<MasterChecklistVersionResponseDto>> PublishVersionAsync(int versionId)
    {
        var draftVersion = await checklistRepository.GetVersionByIdAsync(versionId);
        if (draftVersion is null)
        {
            return ChecklistResultDto<MasterChecklistVersionResponseDto>.Failure(MasterChecklistConstants.Messages.VersionNotFound, isNotFound: true);
        }

        if (draftVersion.Status.StatusName != MasterChecklistConstants.StatusNames.Draft)
        {
            return ChecklistResultDto<MasterChecklistVersionResponseDto>.Failure(MasterChecklistConstants.Messages.OnlyDraftPublishable);
        }

        var publishedStatus = await checklistRepository.GetStatusByNameAsync(MasterChecklistConstants.StatusNames.Published);
        var draftStatus = await checklistRepository.GetStatusByNameAsync(MasterChecklistConstants.StatusNames.Draft);
        if (publishedStatus is null || draftStatus is null)
        {
            return ChecklistResultDto<MasterChecklistVersionResponseDto>.Failure(MasterChecklistConstants.Messages.StatusesNotConfigured);
        }

        draftVersion.StatusId = publishedStatus.Id;
        draftVersion.Status = publishedStatus;

        var newDraftVersion = new MasterChecklistVersion
        {
            VersionNumber = draftVersion.VersionNumber + 1,
            StatusId = draftStatus.Id,
            Status = draftStatus,
            Items = draftVersion.Items
                .Where(i => !i.IsRetired)
                .Select(i => new MasterChecklistItem
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

        return ChecklistResultDto<MasterChecklistVersionResponseDto>.Success(MapVersion(draftVersion));
    }

    private static MasterChecklistVersionResponseDto MapVersion(MasterChecklistVersion version)
    {
        return new MasterChecklistVersionResponseDto
        {
            Id = version.Id,
            VersionNumber = version.VersionNumber,
            StatusId = version.StatusId,
            StatusName = version.Status.StatusName,
            Items = version.Items.OrderBy(i => i.DisplayOrder).Select(MapItem).ToList()
        };
    }

    private static MasterChecklistVersionSummaryResponseDto MapVersionSummary(MasterChecklistVersion version)
    {
        return new MasterChecklistVersionSummaryResponseDto
        {
            Id = version.Id,
            VersionNumber = version.VersionNumber,
            StatusId = version.StatusId,
            StatusName = version.Status.StatusName,
            CreatedAt = version.CreatedAt,
            UpdatedAt = version.UpdatedAt
        };
    }

    private static MasterChecklistItemResponseDto MapItem(MasterChecklistItem item)
    {
        return new MasterChecklistItemResponseDto
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
