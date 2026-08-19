using Pdr.Api.DTOs.AccessTracker;

namespace Pdr.Api.Services.Interfaces;

public interface IAccessTrackerService
{
    Task<List<AccessItemDto>> GetAccessItemsAsync(int engagementId);
    Task<AccessItemDto> AddCustomAccessItemAsync(int engagementId, AddCustomAccessItemRequest request);
    Task<AccessItemDto> UpdateAccessItemStatusAsync(int accessItemId, UpdateAccessItemStatusRequest request);
    Task<AccessItemDto> UpdateAccessItemOwnerAsync(int accessItemId, UpdateAccessItemOwnerRequest request);
    Task DeleteAccessItemAsync(int accessItemId);

    /// <summary>
    /// Evaluates the 2-day billing risk rule for all client-side access items
    /// in the given engagement. Intended as a test endpoint for User Story 3.
    /// The same logic will be run by a daily background job in production.
    /// </summary>
    Task EvaluateClientSideRisksAsync(int engagementId);
}
