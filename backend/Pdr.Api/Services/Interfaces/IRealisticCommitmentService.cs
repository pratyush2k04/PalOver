using System.Threading.Tasks;
using Pdr.Api.DTOs.Handovers;

namespace Pdr.Api.Services.Interfaces;

public interface IRealisticCommitmentService
{
    Task<RealisticCommitmentDto> CreateRealisticCommitmentAsync(int engagementId, int commitmentId, CreateRealisticCommitmentRequest request, string userIdStr);
    Task<RealisticCommitmentDto> UpdateRealisticCommitmentAsync(int engagementId, int commitmentId, UpdateRealisticCommitmentRequest request, string userIdStr);
}
