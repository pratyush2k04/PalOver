using Microsoft.EntityFrameworkCore;
using Pdr.Api.Data;
using Pdr.Api.Models;

namespace Pdr.Api.Repositories;

public class RoleRepository(AppDbContext context) : IRoleRepository
{
    public async Task<Role?> GetByIdAsync(int id)
    {
        return await context.Roles.FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<List<Role>> GetAllAsync()
    {
        return await context.Roles.OrderBy(r => r.Name).ToListAsync();
    }
}
