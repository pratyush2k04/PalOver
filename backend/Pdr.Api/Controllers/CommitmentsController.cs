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
[Route("api/engagements/{engagementId:long}/handover/commitments")]
public class CommitmentsController : ControllerBase
{
    private readonly ICommitmentService _commitmentService;
    private readonly IHandoverAccessService _accessService;
    private readonly IValidator<CreateCommitmentRequest> _createValidator;
    private readonly IValidator<UpdateCommitmentRequest> _updateValidator;

    public CommitmentsController(
        ICommitmentService commitmentService,
        IHandoverAccessService accessService,
        IValidator<CreateCommitmentRequest> createValidator,
        IValidator<UpdateCommitmentRequest> updateValidator)
    {
        _commitmentService = commitmentService;
        _accessService = accessService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    private async Task CheckAccessAsync(long engagementId)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        await _accessService.CanAccessHandoverAsync(engagementId, userId);
    }

    private string GetUserId() => User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

    [HttpPost]
    public async Task<ActionResult<CommitmentDto>> AddCommitment(long engagementId, [FromBody] CreateCommitmentRequest request)
    {
        await CheckAccessAsync(engagementId);

        var validationResult = await _createValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            var firstError = validationResult.Errors.FirstOrDefault()?.ErrorMessage ?? "Invalid commitment request.";
            return BadRequest(new { message = firstError });
        }

        var dto = await _commitmentService.AddCommitmentAsync(engagementId, request, GetUserId());
        return CreatedAtAction(nameof(AddCommitment), new { engagementId, commitmentId = dto.CommitmentId }, dto);
    }

    [HttpPut("{commitmentId:long}")]
    public async Task<ActionResult<CommitmentDto>> UpdateCommitment(long engagementId, long commitmentId, [FromBody] UpdateCommitmentRequest request)
    {
        await CheckAccessAsync(engagementId);

        var validationResult = await _updateValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            var firstError = validationResult.Errors.FirstOrDefault()?.ErrorMessage ?? "Invalid commitment request.";
            return BadRequest(new { message = firstError });
        }

        var dto = await _commitmentService.UpdateCommitmentAsync(engagementId, commitmentId, request, GetUserId());
        return Ok(dto);
    }

    [HttpDelete("{commitmentId:long}")]
    public async Task<IActionResult> DeleteCommitment(long engagementId, long commitmentId)
    {
        await CheckAccessAsync(engagementId);

        await _commitmentService.DeleteCommitmentAsync(engagementId, commitmentId);
        return NoContent();
    }
}
