using Pdr.Api.DTOs.EnvironmentTooling;
using Pdr.Api.Exceptions;
using Pdr.Api.Exceptions.EnvironmentToolingExceptions;
using Pdr.Api.Models.EnvironmentTooling;
using Pdr.Api.Repositories.Interfaces;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Services;

public class EnvironmentToolingService(
    IEnvironmentToolingRepository repository,
    IRiskService riskService) : IEnvironmentToolingService
{
    public async Task<EnvironmentToolingReadinessResponse> GetItemsByEngagementIdAsync(int engagementId)
    {
        if (!await repository.EngagementExistsAsync(engagementId))
        {
            throw new EngagementNotFoundException(engagementId);
        }

        var items = await repository.GetByEngagementIdAsync(engagementId);
        var dtos = items.Select(MapToDto).ToList();

        var readyCount = dtos.Count(i => i.Status == EnvironmentToolingStatus.Ready || i.Status == EnvironmentToolingStatus.NotApplicable);
        var totalCount = dtos.Count;

        return new EnvironmentToolingReadinessResponse
        {
            EngagementId = engagementId,
            ReadyCount = readyCount,
            TotalCount = totalCount,
            IsTechnicallyReady = readyCount == totalCount && totalCount > 0,
            OutstandingItems = dtos.Where(i => i.Status == EnvironmentToolingStatus.NotReady).ToList(),
            Items = dtos
        };
    }

    public async Task<EnvironmentToolingItemDto> AddCustomItemAsync(int engagementId, AddCustomEnvironmentToolingItemRequest request)
    {
        if (!await repository.EngagementExistsAsync(engagementId))
        {
            throw new EngagementNotFoundException(engagementId);
        }

        if (!await repository.UserExistsAsync(request.OwnerId))
        {
            throw new UserNotFoundException(request.OwnerId);
        }

        var item = new EnvironmentToolingItem
        {
            EngagementId = engagementId,
            CustomName = request.CustomName,
            Name = request.CustomName,
            OwnerId = request.OwnerId,
            Status = EnvironmentToolingStatus.NotReady
        };

        await repository.AddAsync(item);
        await repository.SaveChangesAsync();

        var createdItem = await repository.GetByIdAsync(item.EnvironmentToolingItemId);
        return MapToDto(createdItem!);
    }

    public async Task<EnvironmentToolingItemDto> UpdateStatusAsync(int id, UpdateEnvironmentToolingStatusRequest request)
    {
        var item = await repository.GetByIdAsync(id);
        if (item is null)
        {
            throw new EnvironmentToolingItemNotFoundException(id);
        }

        // Validate NotApplicableReason
        if (request.Status == EnvironmentToolingStatus.NotApplicable)
        {
            if (string.IsNullOrWhiteSpace(request.NotApplicableReason))
            {
                throw new NotApplicableReasonRequiredException();
            }
            item.NotApplicableReason = request.NotApplicableReason;
        }
        else
        {
            item.NotApplicableReason = null;
        }

        // Risk integration on BlockReason changes
        var itemName = GetItemDisplayName(item);
        var wasBlocked = !string.IsNullOrWhiteSpace(item.BlockReason);
        var isNowBlocked = !string.IsNullOrWhiteSpace(request.BlockReason);

        if (!wasBlocked && isNowBlocked)
        {
            item.BlockReason = request.BlockReason;
            await riskService.CreateRiskAsync(item.EngagementId, $"Blocked Environment/Tooling: {itemName}", request.BlockReason!, environmentToolingItemId: item.EnvironmentToolingItemId);
        }
        else if (wasBlocked && !isNowBlocked)
        {
            item.BlockReason = null;
            await riskService.ResolveRiskAsync(item.EngagementId, $"Blocked Environment/Tooling: {itemName}", environmentToolingItemId: item.EnvironmentToolingItemId);
        }
        else if (isNowBlocked)
        {
            item.BlockReason = request.BlockReason;
        }

        item.Status = request.Status;
        await repository.SaveChangesAsync();

        return MapToDto(item);
    }

    public async Task<EnvironmentToolingItemDto> UpdateOwnerAsync(int id, UpdateEnvironmentToolingOwnerRequest request)
    {
        var item = await repository.GetByIdAsync(id);
        if (item is null)
        {
            throw new EnvironmentToolingItemNotFoundException(id);
        }

        if (!await repository.UserExistsAsync(request.OwnerId))
        {
            throw new UserNotFoundException(request.OwnerId);
        }

        item.OwnerId = request.OwnerId;
        await repository.SaveChangesAsync();

        var updatedItem = await repository.GetByIdAsync(id);
        return MapToDto(updatedItem!);
    }

    private static string GetItemDisplayName(EnvironmentToolingItem item) =>
        item.EnvironmentToolingDefinition != null
            ? item.EnvironmentToolingDefinition.Name
            : (item.CustomName ?? item.Name);

    private static EnvironmentToolingItemDto MapToDto(EnvironmentToolingItem item) =>
        new()
        {
            EnvironmentToolingItemId = item.EnvironmentToolingItemId,
            Name = GetItemDisplayName(item),
            EnvironmentToolingDefinitionId = item.EnvironmentToolingDefinitionId,
            EngagementId = item.EngagementId,
            OwnerId = item.OwnerId,
            OwnerFullName = item.Owner?.FullName ?? string.Empty,
            CustomName = item.CustomName,
            Status = item.Status,
            NotApplicableReason = item.NotApplicableReason,
            BlockReason = item.BlockReason
        };
}
