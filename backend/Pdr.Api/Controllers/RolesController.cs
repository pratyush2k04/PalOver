using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "IsAdmin")]
public class RolesController(IRoleRepository roleRepository) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetRoles()
    {
        var roles = await roleRepository.GetAllAsync();
        return Ok(roles.Select(r => new { r.Id, r.Name }));
    }
}
