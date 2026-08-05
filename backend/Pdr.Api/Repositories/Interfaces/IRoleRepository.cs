using Pdr.Api.Models.LoginEntity;

namespace Pdr.Api.Repositories.Interfaces;

public interface IRoleRepository
{
    Task<Role?> GetByIdAsync(int id);
    Task<List<Role>> GetAllAsync();
    Task<List<User>> GetUsersInRoleAsync(int roleId);
}
