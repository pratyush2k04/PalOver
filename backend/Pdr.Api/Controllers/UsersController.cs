using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pdr.Api.DTOs.Users;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Controllers;

[ApiController]
[Route("api/[controller]")]

public class UsersController(IUserManagementService userManagementService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetUsers([FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var result = await userManagementService.GetUsersAsync(search, page, pageSize);
        return Ok(result);
    }

    [HttpPatch("{id}/role")]
    [Authorize(Policy = "IsAdmin")]
    public async Task<IActionResult> ChangeRole(int id, [FromBody] ChangeUserRoleRequestDto request)
    {
        try
        {
            var user = await userManagementService.ChangeUserRoleAsync(id, request.NewRoleId);
            return Ok(user);
        }
        catch (UserNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (RoleNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}
