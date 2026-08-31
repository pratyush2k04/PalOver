using Microsoft.EntityFrameworkCore;
using Pdr.Api.Data;
using Pdr.Api.Models.Handover;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Repositories;

public class HandoverQueryQuestionRepository(AppDbContext context) : IHandoverQueryQuestionRepository
{
    public async Task<HandoverQueryQuestion?> GetByQueryIdAsync(int queryId) =>
        await context.HandoverQueryQuestions
            .AsNoTracking()
            .FirstOrDefaultAsync(q => q.QueryId == queryId);

    public async Task<Dictionary<int, HandoverQueryQuestion>> GetByQueryIdsAsync(List<int> queryIds) =>
        await context.HandoverQueryQuestions
            .AsNoTracking()
            .Where(q => queryIds.Contains(q.QueryId))
            .ToDictionaryAsync(q => q.QueryId);

    public async Task AddAsync(HandoverQueryQuestion question) =>
        await context.HandoverQueryQuestions.AddAsync(question);

    public async Task SaveChangesAsync() =>
        await context.SaveChangesAsync();
}
