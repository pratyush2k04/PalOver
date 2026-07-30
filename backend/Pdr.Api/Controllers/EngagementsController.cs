using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pdr.Api.DTOs.Engagements;
using Pdr.Api.Services;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EngagementsController(IEngagementService engagementService) : ControllerBase
{
    [HttpPost("mark-sow-signed")]
    // TODO: restore once the auth scheme (teammate's work) is merged
    // [Authorize(Roles = "Presales")]
    public async Task<ActionResult<MarkSowSignedResponse>> MarkSowSigned(MarkSowSignedRequest request)
    {
        try
        {
            var response = await engagementService.MarkSowSignedAsync(request);
            return Ok(response);
        }
        catch (BillingBeforeKickoffException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
