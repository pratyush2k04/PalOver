using System.Collections.Generic;
using System.Threading.Tasks;
using Pdr.Api.Models;
using Pdr.Api.Models.EngagementAssignment;
using Pdr.Api.Models.ResourceRequests;

namespace Pdr.Api.Repositories.Interfaces;

public interface IResourceRequestRepository
{
    Task<ResourceRequest> AddAsync(ResourceRequest resourceRequest);
    Task<ResourceRequest?> GetByIdAsync(int id);
    Task<List<ResourceRequest>> GetByEngagementIdAsync(int engagementId);
    Task<Engagement?> GetEngagementAsync(int engagementId);
    Task<EngagementAssignment?> GetUserEngagementAssignmentAsync(int engagementId, int userId);
    Task<List<EngagementAssignment>> GetLeadershipAssignmentsAsync(int engagementId);
    Task SaveChangesAsync();
}
