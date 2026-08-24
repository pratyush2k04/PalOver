using Microsoft.AspNetCore.Mvc;
using Pdr.Api.Attributes;
using Pdr.Api.DTOs.Kpi;
using Pdr.Api.Models.LoginEntity;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Controllers;

/// <summary>
/// Provides the Leadership KPI and readiness report (US-080).
/// </summary>
[ApiController]
[Route("api/kpi-reports")]
[Produces("application/json")]
public class KpiReportController(IKpiReportService kpiReportService) : ControllerBase
{
    /// <summary>
    /// Retrieves the KPI report (readiness time, access delay, escalation count, on-time kickoff
    /// rate) for the engagements the requesting Leadership user is allocated to, optionally
    /// filtered by date range, PM, and engagement type. Read-only — never mutates engagement data.
    /// </summary>
    [HttpGet]
    [AuthorizeRoles(RoleEnum.Leadership)]
    [EndpointSummary("Get the Leadership KPI report (Leadership only)")]
    [ProducesResponseType(typeof(KpiReportResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetReport([FromQuery] KpiReportRequestDto request)
    {
        var report = await kpiReportService.GetReportAsync(request);
        return Ok(report);
    }

    /// <summary>
    /// Retrieves the PM and engagement-type filter options relevant to the requesting Leadership
    /// user's allocated engagements.
    /// </summary>
    [HttpGet("filters")]
    [AuthorizeRoles(RoleEnum.Leadership)]
    [EndpointSummary("Get KPI report filter options (Leadership only)")]
    [ProducesResponseType(typeof(KpiReportFilterOptionsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetFilterOptions()
    {
        var options = await kpiReportService.GetFilterOptionsAsync();
        return Ok(options);
    }
}
