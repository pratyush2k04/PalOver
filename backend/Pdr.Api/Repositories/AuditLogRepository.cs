using Microsoft.EntityFrameworkCore;
using Pdr.Api.Data;
using Pdr.Api.Models.Audit;
using Pdr.Api.Repositories.Interfaces;
using Pdr.Api.DTOs.Common;

namespace Pdr.Api.Repositories;

public class AuditLogRepository(AppDbContext context) : IAuditLogRepository
{
    public async Task<AuditAction?> GetAuditActionByIdAsync(int auditActionId)
    {
        return await context.AuditActions
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == auditActionId);
    }

    public async Task AddAsync(AuditLog auditLog)
    {
        await context.AuditLogs.AddAsync(auditLog);
    }

    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }

    public async Task<(List<AuditLog> Items, int TotalCount)> GetPageAsync(int page, int pageSize, int? userId, int? engagementId = null, List<int>? validActionIds = null)
    {
        var query = context.AuditLogs
            .AsNoTracking()
            .Include(x => x.User)
            .Include(x => x.AuditAction)
            .AsQueryable();

        if (userId.HasValue)
        {
            query = query.Where(x => x.UserId == userId);
        }

        if (engagementId.HasValue)
        {
            query = query.Where(x => x.EngagementId == engagementId);
        }

        if (validActionIds != null)
        {
            query = query.Where(x => validActionIds.Contains(x.AuditActionId));
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(x => x.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<List<SimpleUserDto>> GetEngagementAuditUsersAsync(int engagementId)
    {
        return await context.AuditLogs
            .AsNoTracking()
            .Include(x => x.User)
            .Where(x => x.EngagementId == engagementId)
            .Select(x => x.User)
            .Distinct()
            .Select(u => new SimpleUserDto
            {
                Id = u.Id,
                Name = u.FullName
            })
            .OrderBy(u => u.Name)
            .ToListAsync();
    }
}
