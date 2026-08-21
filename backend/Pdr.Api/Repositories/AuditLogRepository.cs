using Microsoft.EntityFrameworkCore;
using Pdr.Api.Data;
using Pdr.Api.Models.Audit;
using Pdr.Api.Repositories.Interfaces;

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

    public async Task<List<AuditLog>> GetPageAsync(int? cursor, int limit, int? userId, int? engagementId = null)
    {
        return await context.AuditLogs
            .AsNoTracking()
            .Include(x => x.User)
            .Include(x => x.AuditAction)
            .Where(x => cursor == null || x.Id < cursor)
            .Where(x => userId == null || x.UserId == userId)
            .Where(x => engagementId == null || x.EngagementId == engagementId)
            .OrderByDescending(x => x.Id)
            .Take(limit + 1)
            .ToListAsync();
    }
}
