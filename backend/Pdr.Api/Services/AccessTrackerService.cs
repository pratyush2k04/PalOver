using Pdr.Api.Constants;
using System.Text.Json;
using Pdr.Api.DTOs.AccessTracker;
using Pdr.Api.Exceptions;

using Pdr.Api.Models.AccessTracker;
using Pdr.Api.Models.LoginEntity;
using Pdr.Api.Repositories.Interfaces;
using Pdr.Api.Services.Interfaces;
using Pdr.Api.Services.Notifications.Mappers;

namespace Pdr.Api.Services;

public class AccessTrackerService(
    IAccessTrackerRepository repository,
    IRiskService riskService,
    INotificationService notificationService,
    ICurrentUserService currentUserService) : IAccessTrackerService
{
    // -------------------------------------------------------------------------
    // State machine: defines which transitions are allowed
    // -------------------------------------------------------------------------
    private static readonly HashSet<(AccessItemStatus From, AccessItemStatus To)> AllowedTransitions =
    [
        (AccessItemStatus.InProgress, AccessItemStatus.Granted),
        (AccessItemStatus.InProgress, AccessItemStatus.Blocked),
        (AccessItemStatus.Granted,    AccessItemStatus.Blocked),
        (AccessItemStatus.Blocked,    AccessItemStatus.InProgress),
        (AccessItemStatus.Blocked,    AccessItemStatus.Granted),
    ];

    // -------------------------------------------------------------------------
    // GET: all access items for an engagement
    // -------------------------------------------------------------------------
    public async Task<List<AccessItemDto>> GetAccessItemsAsync(int engagementId)
    {
        if (!await repository.EngagementExistsAsync(engagementId))
            throw new EngagementNotFoundException(engagementId);

        var items = await repository.GetAccessItemsByEngagementIdAsync(engagementId);
        return items.Select(MapToDto).ToList();
    }

    // -------------------------------------------------------------------------
    // POST: manually add a custom access item to an engagement
    // -------------------------------------------------------------------------
    public async Task<AccessItemDto> AddCustomAccessItemAsync(int engagementId, AddCustomAccessItemRequest request)
    {
        if (!await repository.EngagementExistsAsync(engagementId))
            throw new EngagementNotFoundException(engagementId);

        if (!await repository.UserExistsAsync(request.OwnerId))
            throw new UserNotFoundException(request.OwnerId);

        var item = new AccessItem
        {
            EngagementId = engagementId,
            Name = request.Name,
            IsClientSide = request.IsClientSide,
            OwnerId = request.OwnerId,
            TimeZoneNote = request.TimeZoneNote,
            ExpectedTurnaroundDays = request.ExpectedTurnaroundDays,
            Status = AccessItemStatus.InProgress,
            // Custom items have no definition link
            AccessItemDefinitionId = null
        };

        await repository.AddAccessItemAsync(item);
        await repository.SaveChangesAsync();

        // Reload with Owner navigation populated for the response
        var created = await repository.GetAccessItemByIdAsync(item.Id)
            ?? throw new AccessItemNotFoundException(item.Id);

        return MapToDto(created);
    }

    // -------------------------------------------------------------------------
    // PATCH: update the status of an access item (state machine enforced)
    // -------------------------------------------------------------------------
    public async Task<AccessItemDto> UpdateAccessItemStatusAsync(int accessItemId, UpdateAccessItemStatusRequest request)
    {
        var item = await repository.GetAccessItemByIdAsync(accessItemId)
            ?? throw new AccessItemNotFoundException(accessItemId);

        // Guard: same-status transition is never allowed
        if (item.Status == request.Status)
            throw new InvalidStatusTransitionException(item.Status, request.Status);

        // Guard: check against the allowed transition table
        if (!AllowedTransitions.Contains((item.Status, request.Status)))
            throw new InvalidStatusTransitionException(item.Status, request.Status);

        // Guard: BlockReason is mandatory when moving to Blocked
        if (request.Status == AccessItemStatus.Blocked &&
            string.IsNullOrWhiteSpace(request.BlockReason))
            throw new BlockReasonRequiredException();

        item.Status = request.Status;

        switch (request.Status)
        {
            case AccessItemStatus.Granted:
                item.GrantedDate = DateOnly.FromDateTime(DateTime.UtcNow);
                item.BlockReason = null;
                break;

            case AccessItemStatus.Blocked:
                item.BlockReason = request.BlockReason;
                break;

            case AccessItemStatus.InProgress:
                item.BlockReason = null;
                break;
        }

        await repository.SaveChangesAsync();

        await riskService.SyncAutoCreatedRisksAsync(item.EngagementId);

        if (request.Status == AccessItemStatus.Blocked)
        {
            await notificationService.CreateFromTemplateAsync(
                "AccessItemBlocked",
                item.EngagementId,
                RoleEnum.TechLead,
                JsonSerializer.Serialize(new AccessItemBlockedMapper.AccessItemBlockedData(AccessItemId: item.AccessItemId, TriggerUserId: currentUserService.GetUserId())),
                DateTime.UtcNow);
        }

        return MapToDto(item);
    }

    // -------------------------------------------------------------------------
    // PATCH: reassign the owner of an access item
    // -------------------------------------------------------------------------
    public async Task<AccessItemDto> UpdateAccessItemOwnerAsync(int accessItemId, UpdateAccessItemOwnerRequest request)
    {
        var item = await repository.GetAccessItemByIdAsync(accessItemId)
            ?? throw new AccessItemNotFoundException(accessItemId);

        if (!await repository.UserExistsAsync(request.OwnerId))
            throw new UserNotFoundException(request.OwnerId);

        item.OwnerId = request.OwnerId;
        await repository.SaveChangesAsync();

        // Reload to get updated Owner navigation property
        var updated = await repository.GetAccessItemByIdAsync(accessItemId)
            ?? throw new AccessItemNotFoundException(accessItemId);

        return MapToDto(updated);
    }

    // -------------------------------------------------------------------------
    // POST: evaluate the 2-day billing risk rule (User Story 3 — test endpoint)
    // -------------------------------------------------------------------------
    public async Task EvaluateClientSideRisksAsync(int engagementId)
    {
        var engagement = await repository.GetEngagementAsync(engagementId)
            ?? throw new EngagementNotFoundException(engagementId);

        if (engagement.BillingStartDate is null)
            return;

        await Task.CompletedTask;
    }

    // -------------------------------------------------------------------------
    // Helpers
    // -------------------------------------------------------------------------

    private static AccessItemDto MapToDto(AccessItem item)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        int? daysUntilBillingDate = null;
        if (item.Engagement?.BillingStartDate.HasValue == true)
        {
            daysUntilBillingDate = item.Engagement.BillingStartDate.Value.DayNumber - today.DayNumber;
        }

        bool isOverdueWithClient = false;
        if (item.IsClientSide && item.Status != AccessItemStatus.Granted && item.ExpectedTurnaroundDays.HasValue)
        {
            var createdDate = DateOnly.FromDateTime(item.CreatedAt);
            int workingDaysElapsed = CalculateWorkingDays(createdDate, today);
            isOverdueWithClient = workingDaysElapsed > item.ExpectedTurnaroundDays.Value;
        }

        return new AccessItemDto
        {
            Id = item.Id,
            Name = item.Name,
            IsClientSide = item.IsClientSide,
            OwnerId = item.OwnerId,
            OwnerFullName = item.Owner?.FullName ?? string.Empty,
            Status = item.Status.ToString(),
            TimeZoneNote = item.TimeZoneNote,
            ExpectedTurnaroundDays = item.ExpectedTurnaroundDays,
            DaysUntilBillingDate = daysUntilBillingDate,
            IsOverdueWithClient = isOverdueWithClient,
            GrantedDate = item.GrantedDate,
            BlockReason = item.BlockReason
        };
    }

    private static int CalculateWorkingDays(DateOnly startDate, DateOnly endDate)
    {
        int workingDays = 0;
        var current = startDate;
        while (current < endDate)
        {
            if (current.DayOfWeek != DayOfWeek.Saturday && current.DayOfWeek != DayOfWeek.Sunday)
            {
                workingDays++;
            }
            current = current.AddDays(1);
        }
        return workingDays;
    }
}
