using Pdr.Api.Models.Cadence;

namespace Pdr.Api.DTOs.CommunicationCadences;

public record CreateCommunicationCadenceRequest(
    string StakeholderRole,
    CadenceTypeEnum Cadence,
    List<int> ParticipantUserIds);
