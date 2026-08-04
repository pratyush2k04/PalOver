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
    private readonly IRealisticCommitmentService _realisticCommitmentService;
    private readonly IHandoverAccessService _accessService;
    private readonly IValidator<CreateCommitmentRequest> _createValidator;
    private readonly IValidator<UpdateCommitmentRequest> _updateValidator;
    private readonly IValidator<CreateRealisticCommitmentRequest> _createRealisticValidator;
    private readonly IValidator<UpdateRealisticCommitmentRequest> _updateRealisticValidator;

    public CommitmentsController(
        ICommitmentService commitmentService,
        IRealisticCommitmentService realisticCommitmentService,
        IHandoverAccessService accessService,
        IValidator<CreateCommitmentRequest> createValidator,
        IValidator<UpdateCommitmentRequest> updateValidator,
        IValidator<CreateRealisticCommitmentRequest> createRealisticValidator,
        IValidator<UpdateRealisticCommitmentRequest> updateRealisticValidator)
    {
        _commitmentService = commitmentService;
        _realisticCommitmentService = realisticCommitmentService;
        _accessService = accessService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _createRealisticValidator = createRealisticValidator;
        _updateRealisticValidator = updateRealisticValidator;
    }

    private async Task CheckAccessAsync(long engagementId, string requiredRole)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        await _accessService.CanAccessHandoverAsync(engagementId, userId, requiredRole);
    }

    private string GetUserId() => User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

    [HttpPost]
    public async Task<ActionResult<CommitmentDto>> AddCommitment(long engagementId, [FromBody] CreateCommitmentRequest request)
    {
        await CheckAccessAsync(engagementId, "Pre Sales");

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
        await CheckAccessAsync(engagementId, "Pre Sales");

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
        await CheckAccessAsync(engagementId, "Pre Sales");

        await _commitmentService.DeleteCommitmentAsync(engagementId, commitmentId);
        return NoContent();
    }

    [HttpPost("{commitmentId:long}/realistic")]
    public async Task<ActionResult<RealisticCommitmentDto>> CreateRealisticCommitment(long engagementId, long commitmentId, [FromBody] CreateRealisticCommitmentRequest request)
    {
        await CheckAccessAsync(engagementId, "Project Management");

        var validationResult = await _createRealisticValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            var firstError = validationResult.Errors.FirstOrDefault()?.ErrorMessage ?? "Invalid realistic commitment request.";
            return BadRequest(new { message = firstError });
        }

        var dto = await _realisticCommitmentService.CreateRealisticCommitmentAsync(engagementId, commitmentId, request, GetUserId());
        return CreatedAtAction(nameof(CreateRealisticCommitment), new { engagementId, commitmentId }, dto);
    }

    [HttpPut("{commitmentId:long}/realistic")]
    public async Task<ActionResult<RealisticCommitmentDto>> UpdateRealisticCommitment(long engagementId, long commitmentId, [FromBody] UpdateRealisticCommitmentRequest request)
    {
        await CheckAccessAsync(engagementId, "Project Management");

        var validationResult = await _updateRealisticValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            var firstError = validationResult.Errors.FirstOrDefault()?.ErrorMessage ?? "Invalid realistic commitment request.";
            return BadRequest(new { message = firstError });
        }

        var dto = await _realisticCommitmentService.UpdateRealisticCommitmentAsync(engagementId, commitmentId, request, GetUserId());
        return Ok(dto);
    }
}
