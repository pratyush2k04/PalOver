using Pdr.Api.DTOs.EngagementChecklist;
using Pdr.Api.Exceptions;
using Pdr.Api.Models.EngagementChecklist;
using Pdr.Api.Repositories.Interfaces;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Services;

public class EngagementChecklistService(
    IEngagementChecklistRepository checklistRepository,
    IMasterChecklistRepository masterChecklistRepository,
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
            ?? throw new InvalidOperationException("No published master checklist version found.");

        var checklist = new EngagementChecklist
        {
            EngagementId = engagementId,
            SourceMasterVersionId = publishedVersion.Id,
            VersionNumber = publishedVersion.VersionNumber,
            CreatedBy = "System",
            UpdatedBy = "System",
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
                    Status = "Incomplete",
                    CreatedBy = "System",
                    UpdatedBy = "System"
                })
                .ToList()
        };

        await checklistRepository.AddAsync(checklist);
        await checklistRepository.SaveChangesAsync();

        // Reload with navigation properties for the response
        var saved = await checklistRepository.GetByEngagementIdAsync(engagementId)
            ?? throw new InvalidOperationException("Failed to retrieve saved checklist.");

        return MapToDto(saved);
    }

    public async Task<EngagementChecklistResponseDto> UpdateItemStatusAsync(int engagementId, int itemId, UpdateItemStatusRequestDto dto)
    {
        var item = await checklistRepository.GetItemByIdAsync(engagementId, itemId)
            ?? throw new EngagementChecklistItemNotFoundException(itemId);

        var normalizedStatus = dto.Status?.Trim();
        if (normalizedStatus != "Complete" && normalizedStatus != "Incomplete")
        {
            throw new InvalidChecklistItemStatusException(dto.Status ?? string.Empty);
        }

        if (!item.IsApplicable)
        {
            throw new CannotUpdateNonApplicableItemException(itemId);
        }

        var currentUserId = currentUserService.GetUserId();
        item.Status = normalizedStatus;
        item.UpdatedAt = DateTime.UtcNow;
        item.UpdatedBy = currentUserId.ToString();

        await checklistRepository.SaveChangesAsync();

        var updated = await checklistRepository.GetByEngagementIdAsync(engagementId)
            ?? throw new InvalidOperationException("Failed to retrieve updated checklist.");

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
            item.NotApplicableReason = null;
            item.ExemptionStatus = "None";
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

            if (!item.IsMandatory)
            {
                item.IsApplicable = false;
                item.NotApplicableReason = trimmedReason;
                item.ExemptionStatus = "None";
            }
            else
            {
                item.ExemptionStatus = "Pending";
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
            ?? throw new InvalidOperationException("Failed to retrieve updated checklist.");

        return MapToDto(updated);
    }

    public async Task<EngagementChecklistResponseDto> ReviewExemptionAsync(int engagementId, int itemId, ReviewExemptionRequestDto dto)
    {
        var item = await checklistRepository.GetItemByIdAsync(engagementId, itemId)
            ?? throw new EngagementChecklistItemNotFoundException(itemId);

        if (item.ExemptionStatus != "Pending")
        {
            throw new ExemptionNotPendingException(itemId);
        }

        var currentUserId = currentUserService.GetUserId();

        if (dto.Approved)
        {
            item.IsApplicable = false;
            item.ExemptionStatus = "Approved";
            item.ExemptionReviewedById = currentUserId;
            item.ExemptionReviewedAt = DateTime.UtcNow;
        }
        else
        {
            item.IsApplicable = true;
            item.ExemptionStatus = "Rejected";
            item.ExemptionRejectionReason = dto.RejectionReason?.Trim();
            item.ExemptionReviewedById = currentUserId;
            item.ExemptionReviewedAt = DateTime.UtcNow;
        }

        item.UpdatedAt = DateTime.UtcNow;
        item.UpdatedBy = currentUserId.ToString();

        await checklistRepository.SaveChangesAsync();

        var updated = await checklistRepository.GetByEngagementIdAsync(engagementId)
            ?? throw new InvalidOperationException("Failed to retrieve updated checklist.");

        return MapToDto(updated);
    }

    private static EngagementChecklistResponseDto MapToDto(EngagementChecklist checklist)
    {
        var incompleteMandatoryItems = checklist.Items
            .Where(i => i.IsApplicable && i.IsMandatory && i.Status != "Complete")
            .OrderBy(i => i.DisplayOrder)
            .Select(i => new MandatoryIncompleteItemDto(
                i.Id,
                i.Title,
                i.Status,
                i.Category.CategoryName))
            .ToList();

        var pendingExemptionsCount = checklist.Items.Count(i => i.ExemptionStatus == "Pending");

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
                     i.ExemptionRejectionReason))
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
