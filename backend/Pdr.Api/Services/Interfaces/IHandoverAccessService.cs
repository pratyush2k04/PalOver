using System.Threading.Tasks;

namespace Pdr.Api.Services.Interfaces;

public interface IHandoverAccessService
{
    Task<bool> CanAccessHandoverAsync(int engagementId, int userId);
}
