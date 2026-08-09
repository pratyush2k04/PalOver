using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pdr.Api.Attributes;
using Pdr.Api.DTOs.EngagementChecklist;
using Pdr.Api.Models;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Controllers;

[ApiController]
[Route("api/engagements/{engagementId:int}/workspace/checklist")]
[Authorize]
public class EngagementChecklistController(IEngagementChecklistService engagementChecklistService) : ControllerBase
{
    [HttpGet]
    [AuthorizeRoles(RoleEnum.ProjectManagement, RoleEnum.PreSales, RoleEnum.TechLead, RoleEnum.Leadership)]
    public async Task<ActionResult<EngagementChecklistResponseDto>> GetChecklist(int engagementId)
    {
        var result = await engagementChecklistService.GetByEngagementIdAsync(engagementId);
        return Ok(result);
    }

    [HttpPatch("items/{itemId:int}/status")]
    [AuthorizeRoles(RoleEnum.ProjectManagement, RoleEnum.TechLead)]
    public async Task<ActionResult<EngagementChecklistResponseDto>> UpdateItemStatus(
        int engagementId,
        int itemId,
        [FromBody] UpdateItemStatusRequestDto dto)
    {
        var result = await engagementChecklistService.UpdateItemStatusAsync(engagementId, itemId, dto);
        return Ok(result);
    }

    [HttpPost("items/{itemId:int}/applicability")]
    [AuthorizeRoles(RoleEnum.ProjectManagement)]
    public async Task<ActionResult<EngagementChecklistResponseDto>> UpdateItemApplicability(
        int engagementId,
        int itemId,
        [FromBody] UpdateItemApplicabilityRequestDto dto)
    {
        var result = await engagementChecklistService.UpdateItemApplicabilityAsync(engagementId, itemId, dto);
        return Ok(result);
    }

    [HttpPost("items/{itemId:int}/exemption/review")]
    [AuthorizeRoles(RoleEnum.Leadership)]
    public async Task<ActionResult<EngagementChecklistResponseDto>> ReviewExemption(
        int engagementId,
        int itemId,
        [FromBody] ReviewExemptionRequestDto dto)
    {
        var result = await engagementChecklistService.ReviewExemptionAsync(engagementId, itemId, dto);
        return Ok(result);
    }
}
