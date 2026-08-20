using Pdr.Api.DTOs.Audit;
using Pdr.Api.DTOs.Common;
using Pdr.Api.Models.Audit;

namespace Pdr.Api.Services.Interfaces;

public interface IAuditLogService
{
    Task LogAsync(AuditActionEnum action, IDictionary<string, object?> payload, int? engagementId = null, int? actingUserId = null);

    Task<CursorPagedResult<AuditLogDto>> GetAuditLogsAsync(int? cursor, int limit);
    Task<CursorPagedResult<EngagementAuditLogDto>> GetEngagementAuditLogsAsync(int engagementId, int? cursor, int limit);
}
