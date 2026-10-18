using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pdr.Api.Attributes;
using Pdr.Api.DTOs.People;
using Pdr.Api.Models.LoginEntity;
using Pdr.Api.Services.Interfaces;
using Pdr.Api.Exceptions;

namespace Pdr.Api.Controllers;

/// <summary>
/// Manages key project personnel and stakeholder allocations for an engagement.
/// </summary>
[ApiController]
[Authorize]
[RequireEngagementAccess]
[Route("api/engagements/{engagementId:int}/people")]
[Produces("application/json")]
public class PeopleController(IPersonService personService) : ControllerBase
{
    private string? GetUserId() => User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    /// <summary>
    /// Retrieves all key personnel mapped to an engagement.
    /// </summary>
    /// <param name="engagementId">The engagement ID.</param>
    /// <returns>A list of personnel assigned to the engagement.</returns>
    [HttpGet]
    [EndpointSummary("Get engagement personnel")]
    [ProducesResponseType(typeof(IEnumerable<PersonDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IEnumerable<PersonDto>>> GetPeople(int engagementId)
    {
        var people = await personService.GetPeopleByEngagementAsync(engagementId);
        return Ok(people);
    }

    /// <summary>
    /// Creates a new person record for an engagement.
    /// </summary>
    /// <param name="engagementId">The engagement ID.</param>
    /// <param name="request">The person details.</param>
    /// <returns>The created person record.</returns>
    [HttpPost]
    [EndpointSummary("Create engagement person")]
    [ProducesResponseType(typeof(PersonDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<PersonDto>> CreatePerson(int engagementId, [FromBody] PersonDto request)
    {
        var created = await personService.CreatePersonAsync(engagementId, request, GetUserId());
        return CreatedAtAction(nameof(GetPeople), new { engagementId }, created);
    }

    /// <summary>
    /// Updates details of an existing person assigned to an engagement.
    /// </summary>
    /// <param name="engagementId">The engagement ID.</param>
    /// <param name="id">The person ID.</param>
    /// <param name="request">The updated person details.</param>
    /// <returns>The updated person record.</returns>
    [HttpPut("{id:int}")]
    [EndpointSummary("Update engagement person")]
    [ProducesResponseType(typeof(PersonDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PersonDto>> UpdatePerson(int engagementId, int id, [FromBody] PersonDto request)
    {
        var updated = await personService.UpdatePersonAsync(engagementId, id, request, GetUserId());
        return Ok(updated);
    }

    /// <summary>
    /// Removes a person from an engagement.
    /// </summary>
    /// <param name="engagementId">The engagement ID.</param>
    /// <param name="id">The person ID to delete.</param>
    /// <returns>No content on success.</returns>
    [HttpDelete("{id:int}")]
    [EndpointSummary("Delete engagement person")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletePerson(int engagementId, int id)
    {
        await personService.DeletePersonAsync(engagementId, id, GetUserId());
        return NoContent();
    }

    /// <summary>
    /// Imports handover contacts to the People Directory.
    /// Adds incoming contact from latest transition, deletes all outgoing contacts.
    /// </summary>
    /// <param name="engagementId">The engagement ID.</param>
    /// <returns>Import summary with counts of added, already present, and deleted contacts.</returns>
    [HttpPost("import-from-handover")]
    [EndpointSummary("Import contacts from handover")]
    [ProducesResponseType(typeof(ImportHandoverContactsResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ImportHandoverContactsResponseDto>> ImportFromHandover(int engagementId)
    {
        var result = await personService.ImportHandoverContactsAsync(engagementId, GetUserId());
        return Ok(result);
    }
}
