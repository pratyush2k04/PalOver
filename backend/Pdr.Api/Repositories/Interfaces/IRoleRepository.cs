using Pdr.Api.Models.LoginEntity;

namespace Pdr.Api.Repositories.Interfaces;

public interface IRoleRepository
{
    Task<Role?> GetByIdAsync(int id);
    Task<Dictionary<int, Role>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default);
    Task<List<Role>> GetAllAsync();
    Task<List<User>> GetUsersInRoleAsync(int roleId);
}
