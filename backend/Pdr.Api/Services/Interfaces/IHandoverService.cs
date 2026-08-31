using System.Threading.Tasks;
using Pdr.Api.DTOs.Handovers;

namespace Pdr.Api.Services.Interfaces;

public interface IHandoverService
{
    Task<HandoverInfoDto> GetHandoverAsync(int engagementId);
    Task<HandoverInfoDto> SaveHandoverDraftAsync(int engagementId, SaveHandoverDraftRequest request, string userIdStr);
    Task<HandoverInfoDto> SubmitHandoverAsync(int engagementId, string userIdStr);
    Task<HandoverInfoDto> MarkReviewedAsync(int engagementId, string userIdStr);
}
