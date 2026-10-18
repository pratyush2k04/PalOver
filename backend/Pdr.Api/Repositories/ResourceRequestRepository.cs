using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pdr.Api.Data;
using Pdr.Api.Models;
using Pdr.Api.Models.EngagementAssignment;
using Pdr.Api.Models.Engagements;
using Pdr.Api.Models.LoginEntity;
using Pdr.Api.Models.ResourceRequests;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Repositories;

public class ResourceRequestRepository(AppDbContext context) : IResourceRequestRepository
{
    public async Task<ResourceRequest> AddAsync(ResourceRequest resourceRequest)
    {
        await context.ResourceRequests.AddAsync(resourceRequest);
        await context.SaveChangesAsync();
        return resourceRequest;
    }

    public async Task<ResourceRequest?> GetByIdAsync(int id)
    {
        return await context.ResourceRequests
            .Include(r => r.RequestSkills)
            .Include(r => r.Allocations)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<Dictionary<int, ResourceRequest>> GetByIdsAsync(IEnumerable<int> ids)
    {
        var idList = ids.Distinct().ToList();
        if (idList.Count == 0) return new();

        return await context.ResourceRequests
            .AsNoTracking()
            .Where(r => idList.Contains(r.Id))
            .ToDictionaryAsync(r => r.Id);
    }

    public async Task<List<ResourceRequest>> GetByEngagementIdAsync(int engagementId)
    {
        return await context.ResourceRequests
            .AsNoTracking()
            .Include(r => r.RequestSkills)
            .Include(r => r.Allocations)
            .Where(r => r.EngagementId == engagementId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<Engagement?> GetEngagementAsync(int engagementId)
    {
        return await context.Engagements
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.EngagementId == engagementId);
    }

    public async Task<EngagementAssignment?> GetUserEngagementAssignmentAsync(int engagementId, int userId)
    {
        return await context.EngagementAssignments
            .AsNoTracking()
            .Include(a => a.User).ThenInclude(u => u.Role)
            .FirstOrDefaultAsync(a => a.EngagementId == engagementId && a.UserId == userId);
    }

    public async Task<int> GetUnconfirmedCountByEngagementIdAsync(int engagementId)
    {
        return await context.ResourceRequests
            .AsNoTracking()
            .CountAsync(r => r.EngagementId == engagementId && r.Status != ResourceRequestStatus.Confirmed);
    }

    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }}
