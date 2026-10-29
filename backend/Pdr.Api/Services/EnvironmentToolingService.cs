using Pdr.Api.Constants;
using Pdr.Api.DTOs.EnvironmentTooling;
using Pdr.Api.Exceptions;

using Pdr.Api.Models.EnvironmentTooling;
using Pdr.Api.Models.LoginEntity;
using Pdr.Api.Repositories.Interfaces;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Services;

public class EnvironmentToolingService(
    IEngagementRepository engagementRepository,
    IEnvironmentToolingRepository repository,
    IRiskService riskService,
    ITechnicalReadinessNotificationService readinessNotificationService,
    IEngagementAccessService engagementAccessService) : IEnvironmentToolingService
{
    public async Task<EnvironmentToolingReadinessResponse> GetItemsByEngagementIdAsync(int engagementId)
    {
        await engagementAccessService.ValidateAccessAsync(engagementId);

        if (!await repository.EngagementExistsAsync(engagementId))
        {
            throw new EngagementNotFoundException(engagementId);
        }

        var items = await repository.GetByEngagementIdAsync(engagementId);
        var dtos = items.Select(MapToDto).ToList();

        var readyCount = dtos.Count(i => i.Status == EnvironmentToolingStatus.Granted);
        var totalCount = dtos.Count;

        return new EnvironmentToolingReadinessResponse
        {
            EngagementId = engagementId,
            ReadyCount = readyCount,
            TotalCount = totalCount,
            IsTechnicallyReady = readyCount == totalCount && totalCount > 0,
            OutstandingItems = dtos.Where(i => i.Status != EnvironmentToolingStatus.Granted).ToList(),
            Items = dtos
        };
    }

    public async Task<EnvironmentToolingItemDto> AddCustomItemAsync(int engagementId, AddCustomEnvironmentToolingItemRequest request)
    {
        await engagementAccessService.RequireEngagementRoleAsync(engagementId, EngagementRoleEnum.TechLead);

        if (!await repository.EngagementExistsAsync(engagementId))
        {
            throw new EngagementNotFoundException(engagementId);
        }

        await engagementRepository.EnsureNotSignedOffAsync(engagementId);

        if (!await repository.UserExistsAsync(request.OwnerId))
        {
            throw new UserNotFoundException(request.OwnerId);
        }

        var wasReady = await readinessNotificationService.IsReadyAsync(engagementId);

        var item = new EnvironmentToolingItem
        {
            EngagementId = engagementId,
            CustomName = request.CustomName,
            Name = request.CustomName,
            OwnerId = request.OwnerId,
            Status = EnvironmentToolingStatus.InProgress
        };

        await repository.AddAsync(item);
        await repository.SaveChangesAsync();

        await readinessNotificationService.CheckAndNotifyIfReadyAsync(engagementId, wasReady);

        var createdItem = await repository.GetByIdAsync(item.Id);
        return MapToDto(createdItem!);
    }

    private static readonly HashSet<(EnvironmentToolingStatus From, EnvironmentToolingStatus To)> AllowedTransitions =
    [
        (EnvironmentToolingStatus.InProgress, EnvironmentToolingStatus.Granted),
        (EnvironmentToolingStatus.InProgress, EnvironmentToolingStatus.Blocked),
        (EnvironmentToolingStatus.Granted,    EnvironmentToolingStatus.Blocked),
        (EnvironmentToolingStatus.Blocked,  EnvironmentToolingStatus.InProgress),
        (EnvironmentToolingStatus.Blocked,  EnvironmentToolingStatus.Granted),
    ];

    public async Task<EnvironmentToolingItemDto> UpdateStatusAsync(int id, UpdateEnvironmentToolingStatusRequest request)
    {
        var item = await repository.GetByIdAsync(id);
        if (item is null)
        {
            throw new EnvironmentToolingItemNotFoundException(id);
        }

        await engagementAccessService.RequireEngagementRoleAsync(item.EngagementId, EngagementRoleEnum.TechLead);
        await engagementRepository.EnsureNotSignedOffAsync(item.EngagementId);

        // Guard: same-status transition is never allowed
        if (item.Status == request.Status)
        {
            throw new InvalidEnvironmentToolingStatusTransitionException(item.Status, request.Status);
        }

        // Guard: check against the allowed transition table
        if (!AllowedTransitions.Contains((item.Status, request.Status)))
        {
            throw new InvalidEnvironmentToolingStatusTransitionException(item.Status, request.Status);
        }

        // Validate BlockReason
        if (request.Status == EnvironmentToolingStatus.Blocked)
        {
            if (string.IsNullOrWhiteSpace(request.BlockReason))
            {
                throw new BlockReasonRequiredException();
            }
            item.BlockReason = request.BlockReason;
        }
        else
        {
            item.BlockReason = null;
        }

        var wasReady = await readinessNotificationService.IsReadyAsync(item.EngagementId);

        item.Status = request.Status;
        await repository.SaveChangesAsync();

        await riskService.SyncAutoCreatedRisksAsync(item.EngagementId);

        await readinessNotificationService.CheckAndNotifyIfReadyAsync(item.EngagementId, wasReady);

        return MapToDto(item);
    }

    public async Task<EnvironmentToolingItemDto> UpdateOwnerAsync(int id, UpdateEnvironmentToolingOwnerRequest request)
    {
        var item = await repository.GetByIdAsync(id);
        if (item is null)
        {
            throw new EnvironmentToolingItemNotFoundException(id);
        }

        await engagementAccessService.RequireEngagementRoleAsync(item.EngagementId, EngagementRoleEnum.TechLead);
        await engagementRepository.EnsureNotSignedOffAsync(item.EngagementId);

        if (!await repository.UserExistsAsync(request.OwnerId))
        {
            throw new UserNotFoundException(request.OwnerId);
        }

        item.OwnerId = request.OwnerId;
        await repository.SaveChangesAsync();

        var updatedItem = await repository.GetByIdAsync(id);
        return MapToDto(updatedItem!);
    }

    public async Task DeleteItemAsync(int id)
    {
        var item = await repository.GetByIdAsync(id)
            ?? throw new EnvironmentToolingItemNotFoundException(id);

        await engagementAccessService.RequireEngagementRoleAsync(item.EngagementId, EngagementRoleEnum.TechLead);
        await engagementRepository.EnsureNotSignedOffAsync(item.EngagementId);

        var wasReady = await readinessNotificationService.IsReadyAsync(item.EngagementId);

        await repository.DeleteAsync(item);

        await riskService.SyncAutoCreatedRisksAsync(item.EngagementId);

        await readinessNotificationService.CheckAndNotifyIfReadyAsync(item.EngagementId, wasReady);
    }

    private static string GetItemDisplayName(EnvironmentToolingItem item) =>
        item.EnvironmentToolingDefinition != null
            ? item.EnvironmentToolingDefinition.Name
            : (item.CustomName ?? item.Name);

    private static EnvironmentToolingItemDto MapToDto(EnvironmentToolingItem item) =>
        new()
        {
            Id = item.Id,
            Name = GetItemDisplayName(item),
            EnvironmentToolingDefinitionId = item.EnvironmentToolingDefinitionId,
            EngagementId = item.EngagementId,
            OwnerId = item.OwnerId,
            OwnerFullName = item.Owner?.FullName ?? string.Empty,
            CustomName = item.CustomName,
            Status = item.Status,
            BlockReason = item.BlockReason
        };
}
