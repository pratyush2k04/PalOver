using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pdr.Api.DTOs.Handovers;
using Pdr.Api.Services.Interfaces;
using System.Security.Claims;

namespace Pdr.Api.Controllers;

[ApiController]
// [Authorize]
[Route("api/engagements/{engagementId:long}/handover/commitments")]
public class CommitmentsController : ControllerBase
{
    private readonly ICommitmentService _commitmentService;
    private readonly IHandoverAccessService _accessService;

    public CommitmentsController(ICommitmentService commitmentService, IHandoverAccessService accessService)
    {
        _commitmentService = commitmentService;
        _accessService = accessService;
    }

    private async Task CheckAccess(long engagementId)
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "1";
        if (int.TryParse(userIdStr, out int userId))
        {
            await _accessService.CanAccessHandoverAsync(engagementId, userId);
        }
    }

    private string GetUserId()
    {
        return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "1";
    }

    [HttpPost]
    public async Task<ActionResult<CommitmentDto>> AddCommitment(long engagementId, [FromBody] CreateCommitmentRequest request)
    {
        await CheckAccess(engagementId);
        var dto = await _commitmentService.AddCommitmentAsync(engagementId, request, GetUserId());
        return Created("", dto); // Simplifying Created route
    }

    [HttpPut("{commitmentId:long}")]
    public async Task<ActionResult<CommitmentDto>> UpdateCommitment(long engagementId, long commitmentId, [FromBody] UpdateCommitmentRequest request)
    {
        await CheckAccess(engagementId);
        try
        {
            var dto = await _commitmentService.UpdateCommitmentAsync(engagementId, commitmentId, request, GetUserId());
            return Ok(dto);
        }
        catch (System.Exception ex) when (ex.Message.Contains("not found"))
        {
            return NotFound(new { Message = ex.Message });
        }
    }

    [HttpDelete("{commitmentId:long}")]
    public async Task<IActionResult> DeleteCommitment(long engagementId, long commitmentId)
    {
        await CheckAccess(engagementId);
        try
        {
            await _commitmentService.DeleteCommitmentAsync(engagementId, commitmentId);
            return NoContent();
        }
        catch (System.Exception ex) when (ex.Message.Contains("not found"))
        {
            return NotFound(new { Message = ex.Message });
        }
    }
}
