using Pdr.Api.Models.LoginEntity;

namespace Pdr.Api.Repositories;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task UpdateAsync(User user);
}
