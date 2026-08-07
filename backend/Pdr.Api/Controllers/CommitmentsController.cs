using System.Security.Claims;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pdr.Api.DTOs.Handovers;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/engagements/{engagementId:int}/handover/commitments")]
public class CommitmentsController : ControllerBase
{
    private readonly ICommitmentService _commitmentService;
    private readonly IHandoverAccessService _accessService;

    public CommitmentsController(
        ICommitmentService commitmentService,
        IHandoverAccessService accessService)
    {
        _commitmentService = commitmentService;
        _accessService = accessService;
    }

    private async Task CheckAccessAsync(int engagementId)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        await _accessService.CanAccessHandoverAsync(engagementId, userId);
    }

    private string GetUserId() => User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

    [HttpPost]
    public async Task<ActionResult<CommitmentDto>> AddCommitment(int engagementId, [FromBody] CreateCommitmentRequest request)
    {
        await CheckAccessAsync(engagementId);

        var dto = await _commitmentService.AddCommitmentAsync(engagementId, request, GetUserId());
        return CreatedAtAction(nameof(AddCommitment), new { engagementId, commitmentId = dto.CommitmentId }, dto);
    }

    [HttpPut("{commitmentId:int}")]
    public async Task<ActionResult<CommitmentDto>> UpdateCommitment(int engagementId, int commitmentId, [FromBody] UpdateCommitmentRequest request)
    {
        await CheckAccessAsync(engagementId);

        var dto = await _commitmentService.UpdateCommitmentAsync(engagementId, commitmentId, request, GetUserId());
        return Ok(dto);
    }

    [HttpDelete("{commitmentId:int}")]
    public async Task<IActionResult> DeleteCommitment(int engagementId, int commitmentId)
    {
        await CheckAccessAsync(engagementId);

        await _commitmentService.DeleteCommitmentAsync(engagementId, commitmentId);
        return NoContent();
    }
}
