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

    public async Task<IEnumerable<KickoffActionItemDto>> GetOpenActionItemsAsync(int engagementId, string? currentUserId)
    {
        await ValidateUserAccessAsync(engagementId, currentUserId);

        var actions = await repository.GetOpenActionItemsByEngagementIdAsync(engagementId);
        return actions.Select(MapActionItemToDto);
    }

    public async Task<KickoffActionItemDto> CreateActionItemAsync(int engagementId, KickoffActionItemDto dto, string? currentUserId)
    {
        await ValidateUserAccessAsync(engagementId, currentUserId);

        var record = await repository.GetByEngagementIdAsync(engagementId);
        if (record is null)
        {
            // Auto-create kickoff record if it doesn't exist yet
            record = new KickoffRecord
            {
                EngagementId = engagementId,
                Outcomes = "Kickoff meeting held.",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CreatedBy = currentUserId,
                UpdatedBy = currentUserId
            };
            await repository.AddAsync(record);
            await repository.SaveChangesAsync();
        }

        string ownerName = dto.OwnerName;
        if (dto.AssignedToUserId.HasValue)
        {
            var user = await repository.GetUserByIdAsync(dto.AssignedToUserId.Value);
            if (user is not null)
            {
                ownerName = user.FullName;
            }
        }

        var actionItem = new KickoffActionItem
        {
            KickoffRecordId = record.KickoffRecordId,
            EngagementId = engagementId,
            Title = dto.Title.Trim(),
            OwnerName = ownerName.Trim(),
            AssignedToUserId = dto.AssignedToUserId,
            DueDate = dto.DueDate,
            Status = dto.Status,
            IsNotified = dto.AssignedToUserId.HasValue || dto.IsNotified,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedBy = currentUserId,
            UpdatedBy = currentUserId
        };

        await repository.AddActionItemAsync(actionItem);
        await repository.SaveChangesAsync();

        return MapActionItemToDto(actionItem);
    }

    public async Task<KickoffActionItemDto> UpdateActionItemStatusAsync(int engagementId, int actionItemId, KickoffActionItemStatus status, string? currentUserId)
    {
        await ValidateUserAccessAsync(engagementId, currentUserId);

        var item = await repository.GetActionItemByIdAsync(actionItemId);
        if (item is null || item.EngagementId != engagementId)
        {
            throw new KickoffActionItemNotFoundException(actionItemId);
        }

        item.Status = status;
        item.UpdatedAt = DateTime.UtcNow;
        item.UpdatedBy = currentUserId;

        repository.UpdateActionItem(item);
        await repository.SaveChangesAsync();

        return MapActionItemToDto(item);
    }

    public async Task<KickoffActionItemDto> UpdateActionItemAsync(int engagementId, int actionItemId, KickoffActionItemDto dto, string? currentUserId)
    {
        await ValidateUserAccessAsync(engagementId, currentUserId);

        var item = await repository.GetActionItemByIdAsync(actionItemId);
        if (item is null || item.EngagementId != engagementId)
        {
            throw new KickoffActionItemNotFoundException(actionItemId);
        }

        string ownerName = dto.OwnerName;
        if (dto.AssignedToUserId.HasValue)
        {
            var user = await repository.GetUserByIdAsync(dto.AssignedToUserId.Value);
            if (user is not null)
            {
                ownerName = user.FullName;
            }
        }

        item.Title = dto.Title.Trim();
        item.OwnerName = ownerName.Trim();
        item.AssignedToUserId = dto.AssignedToUserId;
        item.DueDate = dto.DueDate;
        item.Status = dto.Status;
        item.IsNotified = dto.AssignedToUserId.HasValue || dto.IsNotified;
        item.UpdatedAt = DateTime.UtcNow;
        item.UpdatedBy = currentUserId;

        repository.UpdateActionItem(item);
        await repository.SaveChangesAsync();

        return MapActionItemToDto(item);
    }

    public async Task DeleteActionItemAsync(int engagementId, int actionItemId, string? currentUserId)
    {
        await ValidateUserAccessAsync(engagementId, currentUserId);

        var item = await repository.GetActionItemByIdAsync(actionItemId);
        if (item is null || item.EngagementId != engagementId)
        {
            throw new KickoffActionItemNotFoundException(actionItemId);
        }

        repository.DeleteActionItem(item);
        await repository.SaveChangesAsync();
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
            UpdatedAt = record.UpdatedAt,
            ActionItems = record.ActionItems?.Select(MapActionItemToDto).ToList() ?? new()
        };
    }

    private static KickoffActionItemDto MapActionItemToDto(KickoffActionItem item)
    {
        return new KickoffActionItemDto
        {
            KickoffActionItemId = item.KickoffActionItemId,
            KickoffRecordId = item.KickoffRecordId,
            EngagementId = item.EngagementId,
            Title = item.Title,
            OwnerName = item.OwnerName,
            AssignedToUserId = item.AssignedToUserId,
            DueDate = item.DueDate,
            Status = item.Status,
            IsNotified = item.IsNotified
        };
    }
}
