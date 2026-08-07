namespace Pdr.Api.DTOs.CommunicationCadences;

public record CommunicationCadenceResponseDto(
    int Id,
    string StakeholderRole,
    string Cadence,
    string? Participants);
