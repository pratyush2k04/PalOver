using Pdr.Api.DTOs.ClientContactTransitions;

namespace Pdr.Api.Services.Interfaces;

public interface IClientContactTransitionService
{
    Task<ClientContactTransitionResponseDto> GetByEngagementIdAsync(int engagementId);
    Task<ClientContactTransitionResponseDto> SaveTransitionAsync(int engagementId, SaveClientContactTransitionRequest request, string? currentUserId);
    Task<CurrentClientContactDto> GetCurrentClientContactAsync(int engagementId);
}
