using Pdr.Api.DTOs.CommunicationCadences;
using Pdr.Api.Exceptions;
using Pdr.Api.Models.Cadence;
using Pdr.Api.Models.LoginEntity;
using Pdr.Api.Repositories.Interfaces;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Services;

public class CommunicationCadenceService(
    IEngagementRepository engagementRepository,
    IUserRepository userRepository,
    ICommunicationCadenceRepository cadenceRepository,
    IEngagementAccessService engagementAccessService) : ICommunicationCadenceService
{
    public async Task<List<CommunicationCadenceResponseDto>> GetCadencesAsync(int engagementId)
    {
        await EnsureEngagementExistsAsync(engagementId);

        var cadences = await cadenceRepository.GetByEngagementIdAsync(engagementId);
        return cadences.Select(MapToResponseDto).ToList();
    }

    public async Task<CommunicationCadenceResponseDto> AddCadenceAsync(int engagementId, CreateCommunicationCadenceRequest request)
    {
        await engagementAccessService.RequireEngagementRoleAsync(engagementId, EngagementRoleEnum.ProjectManager);
        await EnsureEngagementExistsAsync(engagementId);
        await engagementRepository.EnsureNotSignedOffAsync(engagementId);

        var roleName = request.StakeholderRole.Trim();
        if (await cadenceRepository.ExistsByRoleAsync(engagementId, roleName))
        {
            throw new StakeholderRoleAlreadyExistsException(roleName);
        }

        var participantNames = await ResolveParticipantNamesAsync(request.ParticipantUserIds);

        var cadence = new CommunicationCadence
        {
            EngagementId = engagementId,
            StakeholderRole = roleName,
            Cadence = request.Cadence,
            Participants = participantNames
        };

        var created = await cadenceRepository.AddAsync(cadence);
        return MapToResponseDto(created);
    }

    public async Task<List<EngagementParticipantDto>> GetAssignedParticipantsAsync(int engagementId)
    {
        await EnsureEngagementExistsAsync(engagementId);

        var assignments = await engagementRepository.GetAssignmentsAsync(engagementId);
        var assignedUsers = assignments
            .Select(a => new EngagementParticipantDto(a.User.Id, a.User.FullName))
            .ToList();

        if (assignedUsers.Count == 0)
        {
            var activeUsers = await userRepository.GetAllActiveAsync();
            assignedUsers = activeUsers
                .Select(u => new EngagementParticipantDto(u.Id, u.FullName))
                .ToList();
        }

        return assignedUsers;
    }

    private async Task<string?> ResolveParticipantNamesAsync(List<int>? participantUserIds)
    {
        if (participantUserIds is not null && participantUserIds.Count > 0)
        {
            var users = await userRepository.GetByIdsAsync(participantUserIds);
            var userNames = users.Select(u => u.FullName).ToList();

            if (userNames.Count > 0)
            {
                return string.Join(", ", userNames);
            }
        }

        return null;
    }

    private async Task EnsureEngagementExistsAsync(int engagementId)
    {
        var exists = await engagementRepository.EngagementExistsAsync(engagementId);
        if (!exists)
        {
            throw new EngagementNotFoundException(engagementId);
        }
    }

    private static CommunicationCadenceResponseDto MapToResponseDto(CommunicationCadence cadence) =>
        new(
            cadence.Id,
            cadence.StakeholderRole,
            cadence.Cadence.ToString(),
            cadence.Participants);
}
