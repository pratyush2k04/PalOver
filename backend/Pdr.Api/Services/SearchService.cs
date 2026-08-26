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
    private const int DefaultResultsPerGroupCap = 8;
    private const int FilteredResultsPerGroupCap = 20;

    public async Task<GlobalSearchResponseDto> SearchAsync(string? query, string[]? types = null)
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

        var filterTypes = types?
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .Select(t => t.Trim().ToLowerInvariant())
            .ToHashSet() ?? [];

        var isFiltered = filterTypes.Count > 0;
        var cap = isFiltered ? FilteredResultsPerGroupCap : DefaultResultsPerGroupCap;

        var shouldSearchEngagements = !isFiltered || filterTypes.Contains("engagement") || filterTypes.Contains("engagements");
        var shouldSearchDocuments = !isFiltered || filterTypes.Contains("document") || filterTypes.Contains("documents");
        var shouldSearchPeople = !isFiltered || filterTypes.Contains("person") || filterTypes.Contains("people");
        var shouldSearchChecklists = !isFiltered || filterTypes.Contains("checklist") || filterTypes.Contains("checklistitem") || filterTypes.Contains("checklistitems") || filterTypes.Contains("checklist-item");

        var isAdmin = currentUserService.IsAdmin();
        var isPm = currentUserService.IsInRole(RoleEnum.ProjectManagement);
        var roleId = currentUserService.GetRoleId();

        // Sequential, not Task.WhenAll: all queries share one scoped AppDbContext instance,
        // and DbContext does not support concurrent operations on the same instance.
        var (engagementCount, engagementItems) = shouldSearchEngagements
            ? await searchRepository.SearchEngagementsAsync(engagementIds, term, cap)
            : (0, []);

        var (documentCount, documentItems) = shouldSearchDocuments
            ? await searchRepository.SearchDocumentsAsync(engagementIds, term, cap, roleId, isAdmin, isPm)
            : (0, []);

        var (peopleCount, peopleItems) = shouldSearchPeople
            ? await searchRepository.SearchPeopleAsync(engagementIds, term, cap)
            : (0, []);

        var (checklistCount, checklistItems) = shouldSearchChecklists
            ? await searchRepository.SearchChecklistItemsAsync(engagementIds, term, cap)
            : (0, []);

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
