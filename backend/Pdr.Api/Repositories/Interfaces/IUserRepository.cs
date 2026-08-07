using Pdr.Api.Models.LoginEntity;

namespace Pdr.Api.Repositories.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByIdAsync(int id);
    Task<List<User>> GetByIdsAsync(IEnumerable<int> ids);
    Task<(List<User> Users, int TotalCount)> SearchAsync(string? search, int page, int pageSize);
    Task UpdateAsync(User user);
}
