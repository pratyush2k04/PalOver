using System.Threading.Tasks;

namespace Pdr.Api.Services.Interfaces;

public interface IHandoverAccessService
{
    Task<bool> CanAccessHandoverAsync(int engagementId, int userId);
    Task<bool> CanAccessHandoverAsync(int engagementId, int userId, string requiredRole);
    Task<bool> CanAccessHandoverAsync(int engagementId, int userId, params string[] requiredRoles);
}
