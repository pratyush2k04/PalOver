using System.Text.Json;
using Pdr.Api.Constants;
using Pdr.Api.DTOs.Kickoff;
using Pdr.Api.Exceptions;
using Pdr.Api.Models;
using Pdr.Api.Models.Kickoff;
using Pdr.Api.Models.LoginEntity;
using Pdr.Api.Repositories.Interfaces;
using Pdr.Api.Services.Interfaces;
using Pdr.Api.Services.Notifications.Mappers;

namespace Pdr.Api.Services;

public class KickoffService(
    IKickoffRepository repository,
    IEngagementRepository engagementRepository,
    INotificationService notificationService,
    IEngagementAccessService engagementAccessService) : IKickoffService
{
    public async Task<KickoffRecordDto> GetKickoffRecordAsync(int engagementId, string? currentUserId)
    {
        await EnsureEngagementExistsAsync(engagementId);

        var record = await repository.GetByEngagementIdAsync(engagementId);
        if (record is null)
        {
            return new KickoffRecordDto
            {
                EngagementId = engagementId,
                Outcomes = string.Empty,
                ActionItems = new List<KickoffActionItemDto>()
            };
        }

        return MapToDto(record);
    }

    public async Task<KickoffRecordDto> SaveKickoffRecordAsync(int engagementId, SaveKickoffRecordRequest request, string? currentUserId)
    {
        await engagementAccessService.RequireEngagementRoleAsync(engagementId, EngagementRoleEnum.ProjectManager);
        await EnsureEngagementExistsAsync(engagementId);
        await engagementRepository.EnsureNotSignedOffAsync(engagementId);

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
        await EnsureEngagementExistsAsync(engagementId);

        var actions = await repository.GetOpenActionItemsByEngagementIdAsync(engagementId);
        return actions.Select(MapActionItemToDto);
    }

    public async Task<KickoffActionItemDto> CreateActionItemAsync(int engagementId, KickoffActionItemDto dto, string? currentUserId)
    {
        await engagementAccessService.RequireEngagementRoleAsync(engagementId, EngagementRoleEnum.ProjectManager);
        await EnsureEngagementExistsAsync(engagementId);
        await engagementRepository.EnsureNotSignedOffAsync(engagementId);

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

        if (actionItem.AssignedToUserId.HasValue)
        {
            var data = JsonSerializer.Serialize(
                new KickoffActionItemAssignedMapper.KickoffActionItemAssignedPayload(actionItem.KickoffActionItemId, engagementId));

            await notificationService.CreateFromTemplateAsync(
                NotificationConstants.TemplateKeys.KickoffActionItemAssigned,
                actionItem.AssignedToUserId.Value,
                data,
                DateTime.UtcNow);
        }

        return MapActionItemToDto(actionItem);
    }

    public async Task<KickoffActionItemDto> UpdateActionItemStatusAsync(int engagementId, int actionItemId, KickoffActionItemStatus status, string? currentUserId)
    {
        await EnsureEngagementExistsAsync(engagementId);
        await engagementRepository.EnsureNotSignedOffAsync(engagementId);

        var item = await repository.GetActionItemByIdAsync(actionItemId);
        if (item is null || item.EngagementId != engagementId)
        {
            throw new KickoffActionItemNotFoundException(actionItemId);
        }

        if (item.AssignedToUserId is null || !int.TryParse(currentUserId, out var userId) || item.AssignedToUserId != userId)
        {
            throw new KickoffActionItemStatusUpdateForbiddenException();
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
        await engagementAccessService.RequireEngagementRoleAsync(engagementId, EngagementRoleEnum.ProjectManager);
        await EnsureEngagementExistsAsync(engagementId);
        await engagementRepository.EnsureNotSignedOffAsync(engagementId);

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

    private async Task EnsureEngagementExistsAsync(int engagementId)
    {
        var exists = await repository.EngagementExistsAsync(engagementId);
        if (!exists)
        {
            throw new EngagementNotFoundException(engagementId);
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
