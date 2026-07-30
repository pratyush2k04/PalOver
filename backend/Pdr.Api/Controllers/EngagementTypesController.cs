using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pdr.Api.Data;
using Pdr.Api.Models;

namespace Pdr.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EngagementTypesController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EngagementType>>> GetAll() =>
        Ok(await db.EngagementTypes.OrderBy(t => t.Name).ToListAsync());
}
