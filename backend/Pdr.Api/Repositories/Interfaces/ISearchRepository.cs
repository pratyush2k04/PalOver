using Pdr.Api.Models;
using Pdr.Api.Models.Documents;
using Pdr.Api.Models.EngagementChecklist;
using Pdr.Api.Models.Engagements;

namespace Pdr.Api.Repositories.Interfaces;

public interface ISearchRepository
{
    Task<(int Count, List<Engagement> Items)> SearchEngagementsAsync(List<int> engagementIds, string term, int take);

    Task<(int Count, List<Document> Items)> SearchDocumentsAsync(
        List<int> engagementIds, string term, int take, int? roleId, bool isAdmin, bool isPm);

    Task<(int Count, List<Person> Items)> SearchPeopleAsync(List<int> engagementIds, string term, int take);

    Task<(int Count, List<EngagementChecklistItem> Items)> SearchChecklistItemsAsync(List<int> engagementIds, string term, int take);
}
