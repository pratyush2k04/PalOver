using Pdr.Api.DTOs.AccessTracker;
using Pdr.Api.Exceptions;
using Pdr.Api.Models.AccessTracker;
using Pdr.Api.Repositories.Interfaces;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Services;

public class AccessTrackerService(
    IAccessTrackerRepository repository,
    IRiskService riskService) : IAccessTrackerService
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
            Status = AccessItemStatus.InProgress,
            // Custom items have no definition link
            AccessItemDefinitionId = null
        };

        await repository.AddAccessItemAsync(item);
        await repository.SaveChangesAsync();

        // Reload with Owner navigation populated for the response
        var created = await repository.GetAccessItemByIdAsync(item.AccessItemId)
            ?? throw new AccessItemNotFoundException(item.AccessItemId);

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

        var previousStatus = item.Status;
        item.Status = request.Status;

        switch (request.Status)
        {
            case AccessItemStatus.Granted:
                item.GrantedDate = DateOnly.FromDateTime(DateTime.UtcNow);
                item.BlockReason = null;
                // If unblocking to Granted, resolve the linked risk
                if (previousStatus == AccessItemStatus.Blocked)
                    await riskService.ResolveRiskAsync(item.EngagementId, BuildRiskTitle(item.Name));
                break;

            case AccessItemStatus.Blocked:
                item.BlockReason = request.BlockReason;
                await riskService.CreateRiskAsync(
                    item.EngagementId,
                    BuildRiskTitle(item.Name),
                    $"Access item '{item.Name}' is blocked. Reason: {request.BlockReason}");
                break;

            case AccessItemStatus.InProgress:
                // Unblocking back to InProgress — clear reason and resolve linked risk
                item.BlockReason = null;
                await riskService.ResolveRiskAsync(item.EngagementId, BuildRiskTitle(item.Name));
                break;
        }

        await repository.SaveChangesAsync();
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
    //
    // Deadline logic (as per spec):
    //   deadline = (BillingStartDate > KickoffDate + 2 days)
    //              ? BillingStartDate
    //              : KickoffDate
    //
    // If daysRemaining to deadline <= 2 AND item is client-side AND not Granted
    // → trigger CreateClientSideRiskAsync
    // -------------------------------------------------------------------------
    public async Task EvaluateClientSideRisksAsync(int engagementId)
    {
        var engagement = await repository.GetEngagementAsync(engagementId)
            ?? throw new EngagementNotFoundException(engagementId);

        // BillingStartDate is a required field — treat null as a data integrity skip
        if (engagement.BillingStartDate is null)
            return;

        var billingStartDate = engagement.BillingStartDate.Value;
        var kickoffDate = engagement.KickoffDate;
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        // Determine the effective deadline
        var deadline = billingStartDate > kickoffDate.AddDays(2)
            ? billingStartDate
            : kickoffDate;

        var daysRemaining = deadline.DayNumber - today.DayNumber;

        // Only evaluate if within the 2-day window
        if (daysRemaining > 2)
            return;

        var items = await repository.GetAccessItemsByEngagementIdAsync(engagementId);

        foreach (var item in items.Where(i => i.IsClientSide && i.Status != AccessItemStatus.Granted))
        {
            await riskService.CreateClientSideRiskAsync(engagementId, item.Name);
        }
    }

    // -------------------------------------------------------------------------
    // Helpers
    // -------------------------------------------------------------------------

    private static AccessItemDto MapToDto(AccessItem item) => new()
    {
        AccessItemId = item.AccessItemId,
        Name = item.Name,
        IsClientSide = item.IsClientSide,
        OwnerId = item.OwnerId,
        OwnerFullName = item.Owner?.FullName ?? string.Empty,
        Status = item.Status.ToString(),
        TimeZoneNote = item.TimeZoneNote,
        GrantedDate = item.GrantedDate,
        BlockReason = item.BlockReason
    };

    private static string BuildRiskTitle(string accessItemName) =>
        $"Access Blocked: {accessItemName}";
}
