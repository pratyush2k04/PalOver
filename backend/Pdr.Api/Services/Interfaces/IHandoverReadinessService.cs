using System.Collections.Generic;
using System.Threading.Tasks;
using Pdr.Api.DTOs.HandoverReadiness;

namespace Pdr.Api.Services.Interfaces;

public interface IHandoverReadinessService
{
    Task<List<HandoverReadinessItemDto>> GetItemsAsync(int engagementId);
    Task<HandoverReadinessItemDto> AddCustomItemAsync(int engagementId, AddCustomHandoverReadinessItemRequest request);
    Task<HandoverReadinessItemDto> UpdateItemStatusAsync(int id, UpdateHandoverReadinessStatusRequest request);
    Task DeleteItemAsync(int id);
    Task<HandoverReadinessItemDto> SaveItemEvidenceAsync(int engagementId, int itemId, SaveHandoverReadinessEvidenceRequestDto dto);
}
