using System.Threading.Tasks;
using Pdr.Api.DTOs.Handovers;

namespace Pdr.Api.Services.Interfaces;

public interface IHandoverService
{
    Task<HandoverInfoDto> GetHandoverAsync(long engagementId);
}
