using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pdr.Api.DTOs.Risk;
using Pdr.Api.Exceptions;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Controllers;

[ApiController]
[Authorize]
public class RiskController(IRiskService riskService) : ControllerBase
{
    // GET /api/engagements/{engagementId}/risks
    // Roles: Tech Lead, Project Management, Leadership
    [HttpGet("api/engagements/{engagementId:int}/risks")]
    [Authorize(Roles = "Tech Lead,Project Management,Leadership")]
    public async Task<IActionResult> GetRisks(int engagementId)
    {
        try
        {
            var risks = await riskService.GetRisksByEngagementIdAsync(engagementId);
            return Ok(risks);
        }
        catch (AppException ex)
        {
            return StatusCode(ex.StatusCode, new { message = ex.Message });
        }
    }

    // POST /api/engagements/{engagementId}/risks
    // Roles: Tech Lead, Project Management
    [HttpPost("api/engagements/{engagementId:int}/risks")]
    [Authorize(Roles = "Tech Lead,Project Management")]
    public async Task<IActionResult> CreateRisk(int engagementId, [FromBody] CreateRiskRequest request)
    {
        try
        {
            var risk = await riskService.CreateManualRiskAsync(engagementId, request);
            return CreatedAtAction(nameof(GetRisks), new { engagementId }, risk);
        }
        catch (AppException ex)
        {
            return StatusCode(ex.StatusCode, new { message = ex.Message });
        }
    }

    // PATCH /api/risks/{id}/status
    // Roles: Tech Lead, Project Management
    [HttpPatch("api/risks/{id:int}/status")]
    [Authorize(Roles = "Tech Lead,Project Management")]
    public async Task<IActionResult> UpdateRiskStatus(int id, [FromBody] UpdateRiskStatusRequest request)
    {
        try
        {
            var risk = await riskService.UpdateRiskStatusAsync(id, request);
            return Ok(risk);
        }
        catch (AppException ex)
        {
            return StatusCode(ex.StatusCode, new { message = ex.Message });
        }
    }
}
