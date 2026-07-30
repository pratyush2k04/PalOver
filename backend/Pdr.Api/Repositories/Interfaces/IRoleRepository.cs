using Pdr.Api.Models;

namespace Pdr.Api.Repositories;

public interface IRoleRepository
{
    Task<Role?> GetByIdAsync(int id);
    Task<List<Role>> GetAllAsync();
}
