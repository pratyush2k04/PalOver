using System.Collections.Generic;
using System.Threading.Tasks;
using Pdr.Api.DTOs.ResourceRequests;

namespace Pdr.Api.Services.Interfaces;

public interface IResourceRequestService
{
    Task<ResourceRequestDto> CreateResourceRequestAsync(int engagementId, CreateResourceRequestDto dto);
    Task<List<ResourceRequestDto>> GetForEngagementAsync(int engagementId);
    Task<ResourceRequestDto> AllocateResourcesAsync(int engagementId, int id, AllocateResourcesDto dto);
    Task<ResourceRequestDto> ConfirmAllocationAsync(int engagementId, int id, int allocationId, ConfirmAllocationDto dto);
    Task<ResourceRequestDto> RemoveAllocationAsync(int engagementId, int id, int allocationId, RemoveAllocationDto dto);
    Task<ResourcingGapDto> GetResourcingGapAsync(int engagementId);
}
