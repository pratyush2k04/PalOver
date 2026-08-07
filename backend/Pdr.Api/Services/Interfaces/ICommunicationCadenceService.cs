using Pdr.Api.DTOs.CommunicationCadences;

namespace Pdr.Api.Services.Interfaces;

public interface ICommunicationCadenceService
{
    Task<List<CommunicationCadenceResponseDto>> GetCadencesAsync(int engagementId);
    Task<CommunicationCadenceResponseDto> AddCadenceAsync(int engagementId, CreateCommunicationCadenceRequest request);
    Task<List<EngagementParticipantDto>> GetAssignedParticipantsAsync(int engagementId);
}
