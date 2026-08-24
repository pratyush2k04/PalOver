using System.Threading.Tasks;

namespace Pdr.Api.Services.Interfaces;

public interface IEngagementAccessService
{
    Task ValidateAccessAsync(int engagementId);
    Task<bool> CanAccessAsync(int engagementId);
}
