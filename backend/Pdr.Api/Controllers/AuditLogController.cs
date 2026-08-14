using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pdr.Api.DTOs.Audit;
using Pdr.Api.DTOs.Common;
using Pdr.Api.Models.LoginEntity;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Controllers;

/// <summary>
/// Provides read access to the central audit trail.
/// </summary>
[ApiController]
[Authorize]
[Route("api/audit-logs")]
[Produces("application/json")]
public class AuditLogController(
    IAuditLogService auditLogService,
    ICurrentUserService currentUserService) : ControllerBase
{
    /// <summary>
    /// Retrieves a cursor-paginated page of audit log entries, newest first.
    /// </summary>
    /// <param name="cursor">The AuditLogId to page backward from. Omit for the most recent entries.</param>
    /// <param name="limit">Maximum number of entries to return.</param>
    [HttpGet]
    [ProducesResponseType(typeof(CursorPagedResult<AuditLogDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAuditLogs([FromQuery] int? cursor, [FromQuery] int limit = 50)
    {
        if (!currentUserService.IsInRole(RoleEnum.Leadership) && !currentUserService.IsAdmin())
        {
            return Forbid();
        }

        var result = await auditLogService.GetAuditLogsAsync(cursor, limit);
        return Ok(result);
    }
}
