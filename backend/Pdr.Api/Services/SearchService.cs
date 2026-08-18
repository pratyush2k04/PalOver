using Pdr.Api.DTOs.Search;
using Pdr.Api.Models.LoginEntity;
using Pdr.Api.Repositories.Interfaces;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Services;

public class SearchService(
    ISearchRepository searchRepository,
    IEngagementRepository engagementRepository,
    ICurrentUserService currentUserService) : ISearchService
{
    private const int MinQueryLength = 2;
    private const int ResultsPerGroupCap = 8;

    public async Task<GlobalSearchResponseDto> SearchAsync(string? query)
    {
        var term = query?.Trim() ?? string.Empty;

        if (term.Length < MinQueryLength)
        {
            return new GlobalSearchResponseDto { Query = term };
        }

        var userId = currentUserService.GetUserId();
        var engagementIds = await engagementRepository.GetEngagementIdsForUserAsync(userId);

        if (engagementIds.Count == 0)
        {
            return new GlobalSearchResponseDto { Query = term };
        }

        var isAdmin = currentUserService.IsAdmin();
        var isPm = currentUserService.IsInRole(RoleEnum.ProjectManagement);
        var roleId = currentUserService.GetRoleId();

        // Sequential, not Task.WhenAll: all four queries share one scoped AppDbContext instance,
        // and DbContext does not support concurrent operations on the same instance.
        var (engagementCount, engagementItems) = await searchRepository.SearchEngagementsAsync(engagementIds, term, ResultsPerGroupCap);
        var (documentCount, documentItems) = await searchRepository.SearchDocumentsAsync(engagementIds, term, ResultsPerGroupCap, roleId, isAdmin, isPm);
        var (peopleCount, peopleItems) = await searchRepository.SearchPeopleAsync(engagementIds, term, ResultsPerGroupCap);
        var (checklistCount, checklistItems) = await searchRepository.SearchChecklistItemsAsync(engagementIds, term, ResultsPerGroupCap);

        return new GlobalSearchResponseDto
        {
            Query = term,
            TotalCount = engagementCount + documentCount + peopleCount + checklistCount,
            Engagements = new SearchResultGroupDto<SearchEngagementResultDto>
            {
                Count = engagementCount,
                Items = engagementItems.Select(e => new SearchEngagementResultDto
                {
                    EngagementId = e.EngagementId,
                    EngagementName = e.EngagementName,
                    ClientName = e.ClientName
                }).ToList()
            },
            Documents = new SearchResultGroupDto<SearchDocumentResultDto>
            {
                Count = documentCount,
                Items = documentItems.Select(d => new SearchDocumentResultDto
                {
                    DocumentId = d.Id,
                    FileName = d.FileName,
                    EngagementId = d.EngagementId,
                    EngagementName = d.Engagement.EngagementName,
                    CategoryName = d.Category.CategoryName
                }).ToList()
            },
            People = new SearchResultGroupDto<SearchPersonResultDto>
            {
                Count = peopleCount,
                Items = peopleItems.Select(p => new SearchPersonResultDto
                {
                    PersonId = p.PersonId,
                    Name = p.Name,
                    Role = p.Role,
                    EngagementId = p.EngagementId,
                    EngagementName = p.Engagement.EngagementName
                }).ToList()
            },
            ChecklistItems = new SearchResultGroupDto<SearchChecklistItemResultDto>
            {
                Count = checklistCount,
                Items = checklistItems.Select(i => new SearchChecklistItemResultDto
                {
                    ItemId = i.Id,
                    Title = i.Title,
                    Status = i.Status,
                    EngagementId = i.EngagementChecklist.EngagementId,
                    EngagementName = i.EngagementChecklist.Engagement.EngagementName,
                    CategoryName = i.Category.CategoryName
                }).ToList()
            }
        };
    }
}
