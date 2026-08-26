using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pdr.Api.Attributes;
using Pdr.Api.DTOs.ArchitectureValidation;
using Pdr.Api.Models.LoginEntity;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Controllers;

/// <summary>
/// Manages item-level operations for discovery feature items.
/// </summary>
[ApiController]
[Authorize]
[Route("api/architecture-validation/features")]
[Produces("application/json")]
public class DiscoveryFeatureItemController(IArchitectureValidationService service) : ControllerBase
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
