using Pdr.Api.DTOs.Kickoff;
using Pdr.Api.Exceptions;
using Pdr.Api.Models;
using Pdr.Api.Repositories.Interfaces;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Services;

public class KickoffService(IKickoffRepository repository) : IKickoffService
{
    public async Task<KickoffRecordDto> GetKickoffRecordAsync(int engagementId, string? currentUserId)
    {
        await ValidateUserAccessAsync(engagementId, currentUserId);

        var record = await repository.GetByEngagementIdAsync(engagementId);
        if (record is null)
        {
            throw new KickoffRecordNotFoundException(engagementId);
        }

        return MapToDto(record);
    }

    public async Task<KickoffRecordDto> SaveKickoffRecordAsync(int engagementId, SaveKickoffRecordRequest request, string? currentUserId)
    {
        await ValidateUserAccessAsync(engagementId, currentUserId);

        var existingRecord = await repository.GetByEngagementIdAsync(engagementId);

        if (existingRecord is not null)
        {
            existingRecord.Outcomes = request.Outcomes.Trim();
            existingRecord.UpdatedAt = DateTime.UtcNow;
            existingRecord.UpdatedBy = currentUserId;

            repository.Update(existingRecord);
            await repository.SaveChangesAsync();

            return MapToDto(existingRecord);
        }
        else
        {
            var newRecord = new KickoffRecord
            {
                EngagementId = engagementId,
                Outcomes = request.Outcomes.Trim(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CreatedBy = currentUserId,
                UpdatedBy = currentUserId
            };

            await repository.AddAsync(newRecord);
            await repository.SaveChangesAsync();

            return MapToDto(newRecord);
        }
    }

    private async Task ValidateUserAccessAsync(int engagementId, string? currentUserId)
    {
        var exists = await repository.EngagementExistsAsync(engagementId);
        if (!exists)
        {
            throw new EngagementNotFoundException(engagementId);
        }

        if (int.TryParse(currentUserId, out int userId) && userId > 0)
        {
            var hasAccess = await repository.UserHasAccessToEngagementAsync(engagementId, userId);
            if (!hasAccess)
            {
                throw new HandoverAccessDeniedException("You do not have access to this engagement.");
            }
        }
    }

    private static KickoffRecordDto MapToDto(KickoffRecord record)
    {
        return new KickoffRecordDto
        {
            KickoffRecordId = record.KickoffRecordId,
            EngagementId = record.EngagementId,
            Outcomes = record.Outcomes,
            CreatedAt = record.CreatedAt,
            UpdatedAt = record.UpdatedAt
        };
    }
}
