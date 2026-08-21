using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pdr.Api.DTOs.Audit;
using Pdr.Api.DTOs.Common;
using Pdr.Api.Models.Audit;
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
    /// <param name="page">Page number</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <param name="userId">Optional user ID filter</param>
    /// <param name="categories">Optional category filter</param>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<EngagementAuditLogDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetEngagementAuditLogs([FromQuery] int engagementId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] int? userId = null, [FromQuery] List<AuditCategoryEnum>? categories = null)
    {
        var result = await auditLogService.GetEngagementAuditLogsAsync(engagementId, page, pageSize, userId, categories);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves a list of distinct users who have an entry in the audit log for a specific engagement.
    /// This list only includes users that performed actions the current user is authorized to view.
    /// </summary>
    /// <param name="engagementId">The engagement ID.</param>
    [HttpGet("users")]
    [ProducesResponseType(typeof(List<SimpleUserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetEngagementAuditUsers([FromQuery] int engagementId)
    {
        var result = await auditLogService.GetEngagementAuditUsersAsync(engagementId);
        return Ok(result);
    }
}
