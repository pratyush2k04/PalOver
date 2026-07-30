using Pdr.Api.Models.LoginEntity;

namespace Pdr.Api.Repositories.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task UpdateAsync(User user);
}
