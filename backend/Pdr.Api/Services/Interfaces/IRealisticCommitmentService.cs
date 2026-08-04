using System.Threading.Tasks;
using Pdr.Api.DTOs.Handovers;

namespace Pdr.Api.Services.Interfaces;

public interface IRealisticCommitmentService
{
    Task<RealisticCommitmentDto> CreateRealisticCommitmentAsync(long engagementId, long commitmentId, CreateRealisticCommitmentRequest request, string userIdStr);
    Task<RealisticCommitmentDto> UpdateRealisticCommitmentAsync(long engagementId, long commitmentId, UpdateRealisticCommitmentRequest request, string userIdStr);
}
