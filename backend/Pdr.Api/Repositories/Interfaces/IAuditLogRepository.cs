using Pdr.Api.Models.Audit;

namespace Pdr.Api.Repositories.Interfaces;

public interface IAuditLogRepository
{
    Task<AuditAction?> GetAuditActionByIdAsync(int auditActionId);
    Task AddAsync(AuditLog auditLog);
    Task SaveChangesAsync();
    Task<List<AuditLog>> GetPageAsync(int? cursor, int limit, int? userId, int? engagementId = null);
}
