using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pdr.Api.DTOs.Audit;
using Pdr.Api.DTOs.Common;
using Pdr.Api.Models.Audit;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Controllers;

/// <summary>
/// Provides read access to the central audit log.
/// </summary>
[ApiController]
[Authorize]
[Route("api/audit-logs")]
[Produces("application/json")]
public class AuditLogController(IAuditLogService auditLogService) : ControllerBase
{
    /// <summary>
    /// Retrieves a cursor-paginated page of audit log entries, newest first. Leadership/Admin
    /// callers see every user's entries; everyone else sees only their own.
    /// </summary>
    /// <param name="page">Page number</param>
    /// <param name="pageSize">Number of items per page</param>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<AuditLogDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAuditLogs([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] List<AuditCategoryEnum>? categories = null)
    {
        var result = await auditLogService.GetAuditLogsAsync(page, pageSize, categories);
        return Ok(result);
    }
}
