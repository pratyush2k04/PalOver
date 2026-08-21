using Pdr.Api.Models.Audit;
using Pdr.Api.DTOs.Common;

namespace Pdr.Api.Repositories.Interfaces;

public interface IAuditLogRepository
{
    Task<AuditAction?> GetAuditActionByIdAsync(int auditActionId);
    Task AddAsync(AuditLog auditLog);
    Task SaveChangesAsync();
    Task<(List<AuditLog> Items, int TotalCount)> GetPageAsync(int page, int pageSize, int? userId, int? engagementId = null, List<int>? validActionIds = null);
    Task<List<SimpleUserDto>> GetEngagementAuditUsersAsync(int engagementId);
}
