using Microsoft.EntityFrameworkCore;
using Pdr.Api.Data;
using Pdr.Api.Models.LoginEntity;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Repositories;

public class RoleRepository(AppDbContext context) : IRoleRepository
{
    public async Task<Role?> GetByIdAsync(int id)
    {
        return await context.UserRoles.FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<Dictionary<int, Role>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
    {
        var idList = ids.Distinct().ToList();
        if (idList.Count == 0) return new Dictionary<int, Role>();

        return await context.UserRoles
            .AsNoTracking()
            .Where(r => idList.Contains(r.Id))
            .ToDictionaryAsync(r => r.Id, cancellationToken);
    }

    public async Task<List<Role>> GetAllAsync()
    {
        return await context.UserRoles.OrderBy(r => r.Name).ToListAsync();
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
