using Pdr.Api.Models;

namespace Pdr.Api.Repositories;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByIdAsync(int id);
    Task<(List<User> Users, int TotalCount)> SearchAsync(string? search, int page, int pageSize);
    Task UpdateAsync(User user);
}
