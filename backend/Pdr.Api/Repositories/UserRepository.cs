using Microsoft.EntityFrameworkCore;
using Pdr.Api.Data;
using Pdr.Api.Models.LoginEntity;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Repositories;

public class UserRepository(AppDbContext context) : IUserRepository
{
    public async Task<User?> GetByEmailAsync(string email)
    {
        return await context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<List<User>> GetByIdsAsync(IEnumerable<int> ids)
    {
        return await context.Users
            .AsNoTracking()
            .Include(u => u.Role)
            .Where(u => ids.Contains(u.Id))
            .ToListAsync();
    }

    public async Task<List<User>> GetAdminsAsync()
    {
        return await context.Users
            .AsNoTracking()
            .Where(u => u.IsAdmin)
            .ToListAsync();
    }

    public async Task<(List<User> Users, int TotalCount)> SearchAsync(string? search, int page, int pageSize)
    {
        var query = context.Users.Include(u => u.Role).AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(u =>
                EF.Functions.ILike(u.FullName, $"%{search}%") ||
                EF.Functions.ILike(u.Email, $"%{search}%"));
        }

        var totalCount = await query.CountAsync();

        var users = await query
            .OrderBy(u => u.FullName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (users, totalCount);
    }

    public async Task UpdateAsync(User user)
    {
        context.Users.Update(user);
        await context.SaveChangesAsync();
    }
}
