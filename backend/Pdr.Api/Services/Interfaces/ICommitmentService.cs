using System.Threading.Tasks;
using Pdr.Api.DTOs.Handovers;

namespace Pdr.Api.Services.Interfaces;

public interface ICommitmentService
{
    Task<CommitmentDto> AddCommitmentAsync(int engagementId, CreateCommitmentRequest request, string userIdStr);
    Task<CommitmentDto> UpdateCommitmentAsync(int engagementId, int commitmentId, UpdateCommitmentRequest request, string userIdStr);
    Task DeleteCommitmentAsync(int engagementId, int commitmentId);
}
