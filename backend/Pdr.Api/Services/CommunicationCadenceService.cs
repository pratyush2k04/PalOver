using Microsoft.EntityFrameworkCore;
using Pdr.Api.Data;
using Pdr.Api.DTOs.CommunicationCadences;
using Pdr.Api.Exceptions;
using Pdr.Api.Models.Cadence;
using Pdr.Api.Repositories.Interfaces;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Services;

public class CommunicationCadenceService(
    AppDbContext dbContext,
    ICommunicationCadenceRepository cadenceRepository) : ICommunicationCadenceService
{
    public async Task<List<CommunicationCadenceResponseDto>> GetCadencesAsync(int engagementId)
    {
        await EnsureEngagementExistsAsync(engagementId);

        var cadences = await cadenceRepository.GetByEngagementIdAsync(engagementId);
        return cadences.Select(MapToResponseDto).ToList();
    }

    public async Task<CommunicationCadenceResponseDto> AddCadenceAsync(int engagementId, CreateCommunicationCadenceRequest request)
    {
        await EnsureEngagementExistsAsync(engagementId);

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

        var assignedUsers = await dbContext.EngagementAssignments
            .AsNoTracking()
            .Where(a => a.EngagementId == engagementId && !a.User.IsDeleted)
            .Select(a => new EngagementParticipantDto(
                a.User.Id,
                a.User.FullName))
            .ToListAsync();

        if (assignedUsers.Count == 0)
        {
            assignedUsers = await dbContext.Users
                .AsNoTracking()
                .Where(u => !u.IsDeleted)
                .Select(u => new EngagementParticipantDto(
                    u.Id,
                    u.FullName))
                .ToListAsync();
        }

        return assignedUsers;
    }

    private async Task<string?> ResolveParticipantNamesAsync(List<int>? participantUserIds)
    {
        if (participantUserIds is not null && participantUserIds.Count > 0)
        {
            var userNames = await dbContext.Users
                .AsNoTracking()
                .Where(u => participantUserIds.Contains(u.Id) && !u.IsDeleted)
                .Select(u => u.FullName)
                .ToListAsync();

            if (userNames.Count > 0)
            {
                return string.Join(", ", userNames);
            }
        }

        return null;
    }

    private async Task EnsureEngagementExistsAsync(int engagementId)
    {
        var exists = await dbContext.Engagements.AnyAsync(e => e.EngagementId == engagementId);
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
