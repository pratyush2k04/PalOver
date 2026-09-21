using System.Threading.Tasks;
using Pdr.Api.Models.LoginEntity;

namespace Pdr.Api.Services.Interfaces;

public interface IHandoverAccessService
{
    Task<bool> CanAccessHandoverAsync(int engagementId, int userId);
    Task<bool> CanAccessHandoverAsync(int engagementId, int userId, string requiredRole);
    Task<bool> CanAccessHandoverAsync(int engagementId, int userId, params string[] requiredRoles);
    Task<bool> CanAccessHandoverAsync(int engagementId, int userId, EngagementRoleEnum requiredRole);
    Task<bool> CanAccessHandoverAsync(int engagementId, int userId, params EngagementRoleEnum[] requiredRoles);
}
