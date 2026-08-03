using System.Threading.Tasks;
using Pdr.Api.DTOs.Handovers;

namespace Pdr.Api.Services.Interfaces;

public interface ICommitmentService
{
    Task<CommitmentDto> AddCommitmentAsync(long engagementId, CreateCommitmentRequest request, string userIdStr);
    Task<CommitmentDto> UpdateCommitmentAsync(long engagementId, long commitmentId, UpdateCommitmentRequest request, string userIdStr);
    Task DeleteCommitmentAsync(long engagementId, long commitmentId);
}
