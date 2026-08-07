using Microsoft.EntityFrameworkCore;
using Pdr.Api.Data;
using Pdr.Api.Models.ChangeRequest;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Repositories;

public class ChangeRequestRepository(AppDbContext context) : IChangeRequestRepository
{
    public async Task<bool> EngagementExistsAsync(int engagementId)
    {
        return await context.Engagements.AnyAsync(e => e.EngagementId == engagementId);
    }

    public async Task<ChangeRequest?> GetByIdAsync(int id)
    {
        return await context.ChangeRequests
            .FirstOrDefaultAsync(cr => cr.Id == id);
    }

    public async Task<List<ChangeRequest>> GetByEngagementIdAsync(int engagementId, ChangeRequestStatus? statusFilter = null)
    {
        var query = context.ChangeRequests
            .AsNoTracking()
            .Where(cr => cr.EngagementId == engagementId);

        if (statusFilter.HasValue)
        {
            query = query.Where(cr => cr.Status == statusFilter.Value);
        }

        return await query
            .OrderByDescending(cr => cr.CreatedAt)
            .ToListAsync();
    }

    public async Task<ChangeRequest> AddAsync(ChangeRequest changeRequest)
    {
        await context.ChangeRequests.AddAsync(changeRequest);
        await context.SaveChangesAsync();
        return changeRequest;
    }

    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }
}
