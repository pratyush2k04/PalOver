using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RolesController(IRoleRepository roleRepository) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> GetRoles()
    {
        var roles = await roleRepository.GetAllAsync();
        return Ok(roles.Select(r => new { r.Id, r.Name }));
    }

    [HttpGet("{id:int}/users")]
    [Authorize(Roles = "Leadership")]
    public async Task<IActionResult> GetUsersInRole(int id)
    {
        var users = await roleRepository.GetUsersInRoleAsync(id);
        return Ok(users.Select(u => new { u.Id, u.FullName, u.Email, u.RoleId, RoleName = u.Role.Name }));
    }
}
