using Pdr.Api.DTOs.Audit;
using Pdr.Api.DTOs.Common;
using Pdr.Api.Models.Audit;

namespace Pdr.Api.Services.Interfaces;

public interface IAuditLogService
{
    Task LogAsync(AuditActionEnum action, IDictionary<string, object?> payload, int? engagementId = null, int? actingUserId = null);

    Task<PagedResult<AuditLogDto>> GetAuditLogsAsync(int page, int pageSize);
    Task<PagedResult<EngagementAuditLogDto>> GetEngagementAuditLogsAsync(int engagementId, int page, int pageSize, int? userId = null, List<AuditCategoryEnum>? categories = null);
    Task<List<SimpleUserDto>> GetEngagementAuditUsersAsync(int engagementId);
}
