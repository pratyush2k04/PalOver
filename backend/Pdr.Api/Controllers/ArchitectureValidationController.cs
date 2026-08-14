using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pdr.Api.Attributes;
using Pdr.Api.DTOs.ArchitectureValidation;
using Pdr.Api.Models.LoginEntity;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Controllers;

/// <summary>
/// Manages Pre-sales Architecture Validation and Discovery Feature Breakdown for an engagement.
/// </summary>
[ApiController]
[Authorize]
[Route("api/engagements/{engagementId:int}/architecture-validation")]
[Produces("application/json")]
public class ArchitectureValidationController(
    IArchitectureValidationService service,
    IValidator<SaveArchitectureValidationRequest> saveValidator,
    IValidator<CreateDiscoveryFeatureItemRequest> createFeatureValidator) : ControllerBase
{
    /// <summary>
    /// Retrieves the architecture validation details and discovery breakdown for an engagement.
    /// </summary>
    /// <param name="engagementId">The engagement ID.</param>
    /// <returns>The architecture validation details with sizing calculations and risk evaluation.</returns>
    [HttpGet]
    [AuthorizeRoles(RoleEnum.TechLead, RoleEnum.ProjectManagement, RoleEnum.Leadership)]
    [EndpointSummary("Get architecture validation & discovery breakdown")]
    [ProducesResponseType(typeof(ArchitectureValidationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetValidation(int engagementId)
    {
        var response = await service.GetByEngagementIdAsync(engagementId);
        return Ok(response);
    }

    /// <summary>
    /// Saves architecture validation status and rationale (Tech Lead only).
    /// </summary>
    /// <param name="engagementId">The engagement ID.</param>
    /// <param name="request">The validation payload containing status and rationale.</param>
    /// <returns>The updated architecture validation details.</returns>
    [HttpPut]
    [AuthorizeRoles(RoleEnum.TechLead)]
    [EndpointSummary("Save architecture validation status & rationale (Tech Lead only)")]
    [ProducesResponseType(typeof(ArchitectureValidationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SaveValidation(
        int engagementId,
        [FromBody] SaveArchitectureValidationRequest request)
    {
        var validationResult = await saveValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage);
            return BadRequest(new { errors });
        }

        var result = await service.SaveValidationAsync(engagementId, request);
        return Ok(result);
    }

    /// <summary>
    /// Adds a new discovery feature item to an engagement's breakdown (Tech Lead only).
    /// </summary>
    /// <param name="engagementId">The engagement ID.</param>
    /// <param name="request">The discovery feature item payload.</param>
    /// <returns>The created discovery feature item.</returns>
    [HttpPost("features")]
    [AuthorizeRoles(RoleEnum.TechLead)]
    [EndpointSummary("Add discovery feature item (Tech Lead only)")]
    [ProducesResponseType(typeof(DiscoveryFeatureItemDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddFeatureItem(
        int engagementId,
        [FromBody] CreateDiscoveryFeatureItemRequest request)
    {
        var validationResult = await createFeatureValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage);
            return BadRequest(new { errors });
        }

        var result = await service.AddFeatureItemAsync(engagementId, request);
        return CreatedAtAction(nameof(GetValidation), new { engagementId }, result);
    }
}

/// <summary>
/// Manages item-level operations for discovery feature items.
/// </summary>
[ApiController]
[Authorize]
[Route("api/architecture-validation/features")]
[Produces("application/json")]
public class DiscoveryFeatureItemController(
    IArchitectureValidationService service,
    IValidator<UpdateDiscoveryFeatureItemRequest> updateValidator) : ControllerBase
{
    /// <summary>
    /// Updates an existing discovery feature item (Tech Lead only).
    /// </summary>
    /// <param name="featureId">The feature item ID.</param>
    /// <param name="request">The update payload.</param>
    /// <returns>The updated discovery feature item.</returns>
    [HttpPut("{featureId:int}")]
    [AuthorizeRoles(RoleEnum.TechLead)]
    [EndpointSummary("Update discovery feature item (Tech Lead only)")]
    [ProducesResponseType(typeof(DiscoveryFeatureItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateFeatureItem(
        int featureId,
        [FromBody] UpdateDiscoveryFeatureItemRequest request)
    {
        var validationResult = await updateValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage);
            return BadRequest(new { errors });
        }

        var result = await service.UpdateFeatureItemAsync(featureId, request);
        return Ok(result);
    }

    /// <summary>
    /// Soft-deletes a discovery feature item (Tech Lead only).
    /// </summary>
    /// <param name="featureId">The feature item ID.</param>
    /// <returns>204 No Content.</returns>
    [HttpDelete("{featureId:int}")]
    [AuthorizeRoles(RoleEnum.TechLead)]
    [EndpointSummary("Delete discovery feature item (Tech Lead only)")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteFeatureItem(int featureId)
    {
        await service.DeleteFeatureItemAsync(featureId);
        return NoContent();
    }
}
