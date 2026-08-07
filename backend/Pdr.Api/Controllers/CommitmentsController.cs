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
[Produces("application/json")]
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

    private async Task CheckAccessAsync(int engagementId, string requiredRole)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        await _accessService.CanAccessHandoverAsync(engagementId, userId, requiredRole);
    }

    private string GetUserId() => User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

    /// <summary>
    /// Adds a new commitment to the handover.
    /// </summary>
    /// <param name="engagementId">The unique identifier of the engagement.</param>
    /// <param name="request">The commitment data to add.</param>
    /// <returns>The created commitment.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(CommitmentDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CommitmentDto>> AddCommitment(int engagementId, [FromBody] CreateCommitmentRequest request)
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

    /// <summary>
    /// Updates an existing commitment.
    /// </summary>
    /// <param name="engagementId">The unique identifier of the engagement.</param>
    /// <param name="commitmentId">The unique identifier of the commitment to update.</param>
    /// <param name="request">The updated commitment data.</param>
    /// <returns>The updated commitment.</returns>
    [HttpPut("{commitmentId:int}")]
    [ProducesResponseType(typeof(CommitmentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CommitmentDto>> UpdateCommitment(int engagementId, int commitmentId, [FromBody] UpdateCommitmentRequest request)
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

    /// <summary>
    /// Deletes a specific commitment from the handover.
    /// </summary>
    /// <param name="engagementId">The unique identifier of the engagement.</param>
    /// <param name="commitmentId">The unique identifier of the commitment to delete.</param>
    /// <returns>No content on success.</returns>
    [HttpDelete("{commitmentId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCommitment(int engagementId, int commitmentId)
    {
        await CheckAccessAsync(engagementId, "Pre Sales");

        await _commitmentService.DeleteCommitmentAsync(engagementId, commitmentId);
        return NoContent();
    }

    /// <summary>
    /// Creates a realistic commitment estimation for a given commitment.
    /// </summary>
    /// <param name="engagementId">The unique identifier of the engagement.</param>
    /// <param name="commitmentId">The unique identifier of the commitment.</param>
    /// <param name="request">The realistic commitment data.</param>
    /// <returns>The created realistic commitment.</returns>
    [HttpPost("{commitmentId:int}/realistic")]
    [ProducesResponseType(typeof(RealisticCommitmentDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<RealisticCommitmentDto>> CreateRealisticCommitment(int engagementId, int commitmentId, [FromBody] CreateRealisticCommitmentRequest request)
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

    /// <summary>
    /// Updates an existing realistic commitment estimation.
    /// </summary>
    /// <param name="engagementId">The unique identifier of the engagement.</param>
    /// <param name="commitmentId">The unique identifier of the commitment.</param>
    /// <param name="request">The updated realistic commitment data.</param>
    /// <returns>The updated realistic commitment.</returns>
    [HttpPut("{commitmentId:int}/realistic")]
    [ProducesResponseType(typeof(RealisticCommitmentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RealisticCommitmentDto>> UpdateRealisticCommitment(int engagementId, int commitmentId, [FromBody] UpdateRealisticCommitmentRequest request)
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
