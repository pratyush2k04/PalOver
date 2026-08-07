using Pdr.Api.DTOs.Engagements;

namespace Pdr.Api.Services.Interfaces;

public interface IKickoffReadinessService
{
    Task<KickoffReadinessDto> GetReadinessAsync(int engagementId);
    Task<bool> IsFullyReadyAsync(int engagementId);
    Task MarkKickoffReadyAsync(int engagementId, bool isLeadership);
    Task RevertKickoffReadyAsync(int engagementId);
}
