using Pdr.Api.Models;

namespace Pdr.Api.Repositories;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task UpdateAsync(User user);
}
