using Microsoft.EntityFrameworkCore;
using Pdr.Api.Data;
using Pdr.Api.Models;
using Pdr.Api.Models.Documents;
using Pdr.Api.Models.EngagementChecklist;
using Pdr.Api.Models.Engagements;
using Pdr.Api.Models.LoginEntity;
using Pdr.Api.Models.People;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Repositories;

public class SearchRepository(AppDbContext context) : ISearchRepository
{
    public async Task<(int Count, List<Engagement> Items)> SearchEngagementsAsync(List<int> engagementIds, string term, int take)
    {
        var query = context.Engagements
            .AsNoTracking()
            .Where(e => engagementIds.Contains(e.EngagementId)
                && (EF.Functions.ILike(e.EngagementName, $"%{term}%") || EF.Functions.ILike(e.ClientName, $"%{term}%")));

        var count = await query.CountAsync();
        var items = await query
            .OrderBy(e => e.EngagementName)
            .Take(take)
            .ToListAsync();

        return (count, items);
    }

    public async Task<(int Count, List<Document> Items)> SearchDocumentsAsync(
        List<int> engagementIds, string term, int take, int? roleId, bool isAdmin, bool isPm)
    {
        var query = context.Documents
            .AsNoTracking()
            .Include(d => d.Category)
            .Include(d => d.Engagement)
            .Where(d => engagementIds.Contains(d.EngagementId) && EF.Functions.ILike(d.FileName, $"%{term}%"));

        if (!isAdmin && !isPm)
        {
            query = query.Where(d =>
                d.Visibility == DocumentVisibilityNames.TeamWide
                || (d.Visibility == DocumentVisibilityNames.PmOnly && roleId == (int)UserRoleEnum.Leadership)
                || (d.Visibility == DocumentVisibilityNames.Custom && roleId.HasValue && d.AllowedRoles.Contains(roleId.Value)));
        }

        var count = await query.CountAsync();
        var items = await query
            .OrderByDescending(d => d.CreatedAt)
            .Take(take)
            .ToListAsync();

        return (count, items);
    }

    public async Task<(int Count, List<Person> Items)> SearchPeopleAsync(List<int> engagementIds, string term, int take)
    {
        var query = context.People
            .AsNoTracking()
            .Include(p => p.Engagement)
            .Where(p => engagementIds.Contains(p.EngagementId)
                && (EF.Functions.ILike(p.Name, $"%{term}%") || EF.Functions.ILike(p.Role, $"%{term}%")));

        var count = await query.CountAsync();
        var items = await query
            .OrderBy(p => p.Name)
            .Take(take)
            .ToListAsync();

        return (count, items);
    }

    public async Task<(int Count, List<EngagementChecklistItem> Items)> SearchChecklistItemsAsync(List<int> engagementIds, string term, int take)
    {
        var query = context.EngagementChecklistItems
            .AsNoTracking()
            .Include(i => i.Category)
            .Include(i => i.EngagementChecklist)
                .ThenInclude(c => c.Engagement)
            .Where(i => engagementIds.Contains(i.EngagementChecklist.EngagementId)
                && EF.Functions.ILike(i.Title, $"%{term}%"));

        var count = await query.CountAsync();
        var items = await query
            .OrderByDescending(i => i.CreatedAt)
            .Take(take)
            .ToListAsync();

        return (count, items);
    }
}
