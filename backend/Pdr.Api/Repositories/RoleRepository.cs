using Microsoft.EntityFrameworkCore;
using Pdr.Api.Data;
using Pdr.Api.Models.LoginEntity;
using Pdr.Api.Repositories.Interfaces;

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

    public async Task<List<User>> GetUsersInRoleAsync(int roleId)
    {
        return await context.Users
            .AsNoTracking()
            .Include(u => u.Role)
            .Where(u => u.RoleId == roleId)
            .OrderBy(u => u.FullName)
            .ToListAsync();
    }
}
