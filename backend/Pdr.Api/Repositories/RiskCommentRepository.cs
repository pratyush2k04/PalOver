using Microsoft.EntityFrameworkCore;
using Pdr.Api.Data;
using Pdr.Api.Models.LoginEntity;
using Pdr.Api.Models.Risks;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Repositories;

public class RiskCommentRepository(AppDbContext context) : IRiskCommentRepository
{
    public async Task<List<RiskComment>> GetCommentsByRiskIdAsync(int riskId)
    {
        return await context.RiskComments
            .AsNoTracking()
            .Include(c => c.Author)
            .Where(c => c.RiskId == riskId)
            .OrderBy(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<RiskComment?> GetCommentByIdAsync(int commentId)
    {
        return await context.RiskComments
            .Include(c => c.Author)
            .FirstOrDefaultAsync(c => c.Id == commentId);
    }

    public async Task AddCommentAsync(RiskComment comment)
    {
        await context.RiskComments.AddAsync(comment);
    }

    public async Task UpdateCommentAsync(RiskComment comment)
    {
        context.RiskComments.Update(comment);
        await Task.CompletedTask;
    }

    public async Task<List<User>> GetEngagementMembersAsync(int engagementId)
    {
        return await context.EngagementAssignments
            .AsNoTracking()
            .Where(ea => ea.EngagementId == engagementId)
            .Include(ea => ea.User)
            .Select(ea => ea.User)
            .Distinct()
            .ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }
}
