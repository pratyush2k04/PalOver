using System.Threading.Tasks;
using Pdr.Api.Models.LoginEntity;

namespace Pdr.Api.Services.Interfaces;

public interface IEngagementAccessService
{
    Task ValidateAccessAsync(int engagementId);
    Task<bool> CanAccessAsync(int engagementId);
    Task RequireEngagementRoleAsync(int engagementId, params EngagementRoleEnum[] requiredRoles);
    Task<bool> HasEngagementRoleAsync(int engagementId, params EngagementRoleEnum[] roles);
    Task<int> ResolveItemEngagementIdAsync(ItemEntityType entityType, int itemId);
}
