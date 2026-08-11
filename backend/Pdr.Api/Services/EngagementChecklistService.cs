using Pdr.Api.Constants;
using Pdr.Api.DTOs.EngagementChecklist;
using Pdr.Api.Exceptions;
using Pdr.Api.Models.EngagementChecklist;
using Pdr.Api.Repositories.Interfaces;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Services;

public class EngagementChecklistService(
    IEngagementChecklistRepository checklistRepository,
    IMasterChecklistRepository masterChecklistRepository,
    IPersonRepository personRepository,
    ICurrentUserService currentUserService) : IEngagementChecklistService
{
    public async Task<EngagementChecklistResponseDto?> GetByEngagementIdAsync(int engagementId)
    {
        var checklist = await checklistRepository.GetByEngagementIdAsync(engagementId);

        // If no checklist exists yet (e.g. existing seed engagements), apply one on the fly
        if (checklist is null)
        {
            return await ApplyMasterChecklistToEngagementAsync(engagementId);
        }

        return MapToDto(checklist);
    }

    public async Task<EngagementChecklistResponseDto> ApplyMasterChecklistToEngagementAsync(int engagementId)
    {
        var publishedVersion = await masterChecklistRepository.GetPublishedVersionAsync()
            ?? throw new InvalidOperationException(MasterChecklistConstants.Messages.NoPublishedVersion);

        var checklist = new EngagementChecklist
        {
            EngagementId = engagementId,
            SourceMasterVersionId = publishedVersion.Id,
            VersionNumber = publishedVersion.VersionNumber,
            CreatedBy = MasterChecklistConstants.Audit.SystemUser,
            UpdatedBy = MasterChecklistConstants.Audit.SystemUser,
            Items = publishedVersion.Items
                .Where(i => !i.IsRetired)
                .Select(i => new EngagementChecklistItem
                {
                    SourceMasterItemId = i.Id,
                    Title = i.Title,
                    Description = i.Description,
                    CategoryId = i.CategoryId,
                    IsMandatory = i.IsMandatory,
                    DisplayOrder = i.DisplayOrder,
                    IsApplicable = true,
                    Status = MasterChecklistConstants.ItemStatuses.Incomplete,
                    CreatedBy = MasterChecklistConstants.Audit.SystemUser,
                    UpdatedBy = MasterChecklistConstants.Audit.SystemUser
                })
                .ToList()
        };

        await checklistRepository.AddAsync(checklist);
        await checklistRepository.SaveChangesAsync();

        // Reload with navigation properties for the response
        var saved = await checklistRepository.GetByEngagementIdAsync(engagementId)
            ?? throw new InvalidOperationException(MasterChecklistConstants.Messages.FailedToSaveChecklist);

        return MapToDto(saved);
    }

    public async Task<EngagementChecklistResponseDto> UpdateItemStatusAsync(int engagementId, int itemId, UpdateItemStatusRequestDto dto)
    {
        var item = await checklistRepository.GetItemByIdAsync(engagementId, itemId)
            ?? throw new EngagementChecklistItemNotFoundException(itemId);

        var normalizedStatus = dto.Status?.Trim();
        if (normalizedStatus != MasterChecklistConstants.ItemStatuses.Complete && normalizedStatus != MasterChecklistConstants.ItemStatuses.Incomplete)
        {
            throw new InvalidChecklistItemStatusException(dto.Status ?? string.Empty);
        }

        if (!item.IsApplicable)
        {
            throw new CannotUpdateNonApplicableItemException(itemId);
        }

        if (item.Status == "Blocked")
        {
            throw new CannotUpdateBlockedItemException(itemId);
        }

        var currentUserId = currentUserService.GetUserId();
        item.Status = normalizedStatus;
        item.UpdatedAt = DateTime.UtcNow;
        item.UpdatedBy = currentUserId.ToString();

        await checklistRepository.SaveChangesAsync();

        var updated = await checklistRepository.GetByEngagementIdAsync(engagementId)
            ?? throw new InvalidOperationException(MasterChecklistConstants.Messages.FailedToUpdateChecklist);

        return MapToDto(updated);
    }

    public async Task<EngagementChecklistResponseDto> UpdateItemApplicabilityAsync(int engagementId, int itemId, UpdateItemApplicabilityRequestDto dto)
    {
        var item = await checklistRepository.GetItemByIdAsync(engagementId, itemId)
            ?? throw new EngagementChecklistItemNotFoundException(itemId);

        var currentUserId = currentUserService.GetUserId();

        if (dto.IsApplicable)
        {
            item.IsApplicable = true;
            item.Status = "Incomplete"; // AC4: restoring an item always returns it as Incomplete
            item.BlockReason = null;
            item.NotApplicableReason = null;
            item.ExemptionStatus = MasterChecklistConstants.ExemptionStatuses.None;
            item.ExemptionRequestedById = null;
            item.ExemptionRequestedAt = null;
            item.ExemptionReviewedById = null;
            item.ExemptionReviewedAt = null;
            item.ExemptionRejectionReason = null;
        }
        else
        {
            if (string.IsNullOrWhiteSpace(dto.Reason))
            {
                throw new ChecklistNotApplicableReasonRequiredException();
            }

            var trimmedReason = dto.Reason.Trim();

            // A Not Applicable item shouldn't carry a stale Blocked state
            if (item.Status == "Blocked")
            {
                item.Status = "Incomplete";
                item.BlockReason = null;
            }

            if (!item.IsMandatory)
            {
                item.IsApplicable = false;
                item.NotApplicableReason = trimmedReason;
                item.ExemptionStatus = MasterChecklistConstants.ExemptionStatuses.None;
            }
            else
            {
                item.ExemptionStatus = MasterChecklistConstants.ExemptionStatuses.Pending;
                item.NotApplicableReason = trimmedReason;
                item.ExemptionRequestedById = currentUserId;
                item.ExemptionRequestedAt = DateTime.UtcNow;
                // Mandatory item stays IsApplicable = true until approved by Leadership
            }
        }

        item.UpdatedAt = DateTime.UtcNow;
        item.UpdatedBy = currentUserId.ToString();

        await checklistRepository.SaveChangesAsync();

        var updated = await checklistRepository.GetByEngagementIdAsync(engagementId)
            ?? throw new InvalidOperationException(MasterChecklistConstants.Messages.FailedToUpdateChecklist);

        return MapToDto(updated);
    }

    public async Task<EngagementChecklistResponseDto> ReviewExemptionAsync(int engagementId, int itemId, ReviewExemptionRequestDto dto)
    {
        var item = await checklistRepository.GetItemByIdAsync(engagementId, itemId)
            ?? throw new EngagementChecklistItemNotFoundException(itemId);

        if (item.ExemptionStatus != MasterChecklistConstants.ExemptionStatuses.Pending)
        {
            throw new ExemptionNotPendingException(itemId);
        }

        var currentUserId = currentUserService.GetUserId();

        if (dto.Approved)
        {
            item.IsApplicable = false;
            item.ExemptionStatus = MasterChecklistConstants.ExemptionStatuses.Approved;
            item.ExemptionReviewedById = currentUserId;
            item.ExemptionReviewedAt = DateTime.UtcNow;
        }
        else
        {
            item.IsApplicable = true;
            item.ExemptionStatus = MasterChecklistConstants.ExemptionStatuses.Rejected;
            item.ExemptionRejectionReason = dto.RejectionReason?.Trim();
            item.ExemptionReviewedById = currentUserId;
            item.ExemptionReviewedAt = DateTime.UtcNow;
        }

        item.UpdatedAt = DateTime.UtcNow;
        item.UpdatedBy = currentUserId.ToString();

        await checklistRepository.SaveChangesAsync();

        var updated = await checklistRepository.GetByEngagementIdAsync(engagementId)
            ?? throw new InvalidOperationException(MasterChecklistConstants.Messages.FailedToUpdateChecklist);

        return MapToDto(updated);
    }

    public async Task<EngagementChecklistResponseDto> UpdateItemOwnerAsync(int engagementId, int itemId, UpdateItemOwnerRequestDto dto)
    {
        var item = await checklistRepository.GetItemByIdAsync(engagementId, itemId)
            ?? throw new EngagementChecklistItemNotFoundException(itemId);

        if (!item.IsApplicable)
        {
            throw new CannotUpdateNonApplicableItemException(itemId);
        }

        if (dto.PersonId is not null)
        {
            await EnsurePersonBelongsToEngagementAsync(engagementId, dto.PersonId.Value);
        }

        var currentUserId = currentUserService.GetUserId();
        item.OwnerId = dto.PersonId;
        item.UpdatedAt = DateTime.UtcNow;
        item.UpdatedBy = currentUserId.ToString();

        await checklistRepository.SaveChangesAsync();

        var updated = await checklistRepository.GetByEngagementIdAsync(engagementId)
            ?? throw new InvalidOperationException("Failed to retrieve updated checklist.");

        return MapToDto(updated);
    }

    public async Task<EngagementChecklistResponseDto> BlockItemAsync(int engagementId, int itemId, BlockChecklistItemRequestDto dto)
    {
        var item = await checklistRepository.GetItemByIdAsync(engagementId, itemId)
            ?? throw new EngagementChecklistItemNotFoundException(itemId);

        if (!item.IsApplicable)
        {
            throw new CannotUpdateNonApplicableItemException(itemId);
        }

        if (string.IsNullOrWhiteSpace(dto.Reason))
        {
            throw new ChecklistBlockReasonRequiredException();
        }

        await EnsurePersonBelongsToEngagementAsync(engagementId, dto.PersonId);

        var currentUserId = currentUserService.GetUserId();
        item.Status = "Blocked";
        item.BlockReason = dto.Reason.Trim();
        item.OwnerId = dto.PersonId; // owner is confirmed/assigned as part of flagging Blocked
        item.UpdatedAt = DateTime.UtcNow;
        item.UpdatedBy = currentUserId.ToString();

        await checklistRepository.SaveChangesAsync();

        var updated = await checklistRepository.GetByEngagementIdAsync(engagementId)
            ?? throw new InvalidOperationException("Failed to retrieve updated checklist.");

        return MapToDto(updated);
    }

    public async Task<EngagementChecklistResponseDto> UnblockItemAsync(int engagementId, int itemId)
    {
        var item = await checklistRepository.GetItemByIdAsync(engagementId, itemId)
            ?? throw new EngagementChecklistItemNotFoundException(itemId);

        if (item.Status != "Blocked")
        {
            throw new ItemNotBlockedException(itemId);
        }

        var currentUserId = currentUserService.GetUserId();
        item.Status = "Incomplete";
        item.BlockReason = null;
        item.UpdatedAt = DateTime.UtcNow;
        item.UpdatedBy = currentUserId.ToString();

        await checklistRepository.SaveChangesAsync();

        var updated = await checklistRepository.GetByEngagementIdAsync(engagementId)
            ?? throw new InvalidOperationException("Failed to retrieve updated checklist.");

        return MapToDto(updated);
    }

    private async Task EnsurePersonBelongsToEngagementAsync(int engagementId, int personId)
    {
        var people = await personRepository.GetByEngagementIdAsync(engagementId);
        if (!people.Any(p => p.PersonId == personId))
        {
            throw new PersonNotFoundException(personId, engagementId);
        }
    }

    private static EngagementChecklistResponseDto MapToDto(EngagementChecklist checklist)
    {
        var incompleteMandatoryItems = checklist.Items
            .Where(i => i.IsApplicable && i.IsMandatory && i.Status != MasterChecklistConstants.ItemStatuses.Complete)
            .OrderBy(i => i.DisplayOrder)
            .Select(i => new MandatoryIncompleteItemDto(
                i.Id,
                i.Title,
                i.Status,
                i.Category.CategoryName))
            .ToList();

        var pendingExemptionsCount = checklist.Items.Count(i => i.ExemptionStatus == MasterChecklistConstants.ExemptionStatuses.Pending);

        var categories = checklist.Items
            .GroupBy(i => new { i.CategoryId, i.Category.CategoryName })
            .OrderBy(g => g.Key.CategoryName)
            .Select(g => new EngagementChecklistCategoryGroupDto(
                g.Key.CategoryId,
                g.Key.CategoryName,
                g.OrderBy(i => i.DisplayOrder)
                 .Select(i => new EngagementChecklistItemDto(
                     i.Id,
                     i.EngagementChecklistId,
                     i.SourceMasterItemId,
                     i.Title,
                     i.Description,
                     i.CategoryId,
                     i.Category.CategoryName,
                     i.IsMandatory,
                     i.DisplayOrder,
                     i.IsApplicable,
                     i.Status,
                     i.NotApplicableReason,
                     i.ExemptionStatus,
                     i.ExemptionRequestedById,
                     i.ExemptionRequestedAt,
                     i.ExemptionReviewedById,
                     i.ExemptionReviewedAt,
                     i.ExemptionRejectionReason,
                     i.OwnerId,
                     i.Owner != null ? i.Owner.Name : null,
                     i.BlockReason))
                 .ToList()))
            .ToList();

        return new EngagementChecklistResponseDto(
            checklist.Id,
            checklist.EngagementId,
            checklist.SourceMasterVersionId,
            checklist.VersionNumber,
            pendingExemptionsCount,
            incompleteMandatoryItems,
            categories);
    }
}
