using System.Threading.Tasks;

namespace Pdr.Api.Services.Interfaces;

public interface IHandoverAccessService
{
    Task<bool> CanAccessHandoverAsync(long engagementId, int userId);
}
