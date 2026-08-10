using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pdr.Api.DTOs.People;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Controllers;

[ApiController]
[Authorize(Roles = "Pre Sales,Project Management,Tech Lead")]
[Route("api/engagements/{engagementId:int}/people")]
public class PeopleController(
    IPersonService personService,
    IValidator<PersonDto> validator) : ControllerBase
{
    private string? GetUserId() => User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PersonDto>>> GetPeople(int engagementId)
    {
        var people = await personService.GetPeopleByEngagementAsync(engagementId);
        return Ok(people);
    }

    [HttpPost]
    public async Task<ActionResult<PersonDto>> CreatePerson(int engagementId, [FromBody] PersonDto request)
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage);
            return BadRequest(new { errors });
        }

        var created = await personService.CreatePersonAsync(engagementId, request, GetUserId());
        return CreatedAtAction(nameof(GetPeople), new { engagementId }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<PersonDto>> UpdatePerson(int engagementId, int id, [FromBody] PersonDto request)
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage);
            return BadRequest(new { errors });
        }

        var updated = await personService.UpdatePersonAsync(engagementId, id, request, GetUserId());
        return Ok(updated);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeletePerson(int engagementId, int id)
    {
        await personService.DeletePersonAsync(engagementId, id, GetUserId());
        return NoContent();
    }
}
