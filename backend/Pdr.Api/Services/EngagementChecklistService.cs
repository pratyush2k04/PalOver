using Pdr.Api.DTOs.EngagementChecklist;
using Pdr.Api.Models.EngagementChecklist;
using Pdr.Api.Repositories.Interfaces;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Services;

public class EngagementChecklistService(
    IEngagementChecklistRepository checklistRepository,
    IMasterChecklistRepository masterChecklistRepository) : IEngagementChecklistService
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

    private static EngagementChecklistResponseDto MapToDto(EngagementChecklist checklist)
    {
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
                     i.Status))
                 .ToList()))
            .ToList();

        return new EngagementChecklistResponseDto(
            checklist.Id,
            checklist.EngagementId,
            checklist.SourceMasterVersionId,
            checklist.VersionNumber,
            categories);
    }
}
