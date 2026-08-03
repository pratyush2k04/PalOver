using System.Threading.Tasks;
using Pdr.Api.DTOs.Handovers;

namespace Pdr.Api.Services.Interfaces;

public interface IHandoverService
{
    Task<HandoverInfoDto> GetHandoverAsync(long engagementId);
    Task<HandoverInfoDto> SaveHandoverDraftAsync(long engagementId, SaveHandoverDraftRequest request, string userIdStr);
    Task<HandoverInfoDto> SubmitHandoverAsync(long engagementId, string userIdStr);
}
