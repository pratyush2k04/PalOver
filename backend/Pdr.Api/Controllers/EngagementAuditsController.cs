using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pdr.Api.DTOs.Audit;
using Pdr.Api.DTOs.Common;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Controllers;

/// <summary>
/// Provides read access to the engagement audit log.
/// </summary>
[ApiController]
[Authorize]
[Route("api/engagementaudits")]
[Produces("application/json")]
public class EngagementAuditsController(IAuditLogService auditLogService) : ControllerBase
{
    /// <summary>
    /// Retrieves a cursor-paginated page of audit log entries for a specific engagement, newest first.
    /// Returns entries irrespective of who the actor was.
    /// </summary>
    /// <param name="engagementId">The engagement ID.</param>
    /// <param name="cursor">The Id to page backward from. Omit for the most recent entries.</param>
    /// <param name="limit">Maximum number of entries to return.</param>
    [HttpGet]
    [ProducesResponseType(typeof(CursorPagedResult<AuditLogDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetEngagementAuditLogs([FromQuery] int engagementId, [FromQuery] int? cursor, [FromQuery] int limit = 50)
    {
        var result = await auditLogService.GetEngagementAuditLogsAsync(engagementId, cursor, limit);
        return Ok(result);
    }
}
