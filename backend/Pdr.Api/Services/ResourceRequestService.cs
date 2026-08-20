using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Pdr.Api.Constants;
using Pdr.Api.DTOs.ResourceRequests;
using Pdr.Api.Exceptions;
using Pdr.Api.Models.Audit;
using Pdr.Api.Models.LoginEntity;
using Pdr.Api.Models.ResourceRequests;
using Pdr.Api.Repositories.Interfaces;
using Pdr.Api.Services.Interfaces;
using Pdr.Api.Services.Notifications.Mappers;

namespace Pdr.Api.Services;

public class ResourceRequestService(
    IResourceRequestRepository resourceRequestRepository,
    ICurrentUserService currentUserService,
    INotificationService notificationService,
    IAuditLogService auditLogService) : IResourceRequestService
{
    public async Task<ResourceRequestDto> CreateResourceRequestAsync(int engagementId, CreateResourceRequestDto dto)
    {
        var engagement = await resourceRequestRepository.GetEngagementAsync(engagementId)
            ?? throw new EngagementNotFoundException(engagementId);

        var currentUserId = currentUserService.GetUserId();
        var assignment = await resourceRequestRepository.GetUserEngagementAssignmentAsync(engagementId, currentUserId);
        if (assignment == null || assignment.User.RoleId != (int)RoleEnum.ProjectManagement)
        {
            throw new ResourceRequestAccessDeniedException();
        }

        if (engagement.BillingStartDate == null)
        {
            throw new EngagementBillingDateNotSetException();
        }

        if (dto.RequiredByDate > engagement.BillingStartDate.Value)
        {
            throw new ResourceRequestAfterBillingDateException();
        }

        var gapBefore = await GetResourcingGapAsync(engagementId);

        var entity = new ResourceRequest
        {
            EngagementId = engagementId,
            StandardRole = dto.StandardRole,
            PeopleNeeded = dto.PeopleNeeded,
            RequiredByDate = dto.RequiredByDate,
            JobDescription = dto.JobDescription.Trim(),
            Status = ResourceRequestStatus.Raised,
            RequestedById = currentUserId,
            RequestSkills = dto.Skills.Select(s => new ResourceRequestSkill
            {
                Skill = s
            }).ToList()
        };

        var created = await resourceRequestRepository.AddAsync(entity);

        await NotifyLeadershipAsync(engagementId, created.Id);

        var gapAfter = await GetResourcingGapAsync(engagementId);
        await CheckAndHandleGapTransitionAsync(engagementId, gapBefore, gapAfter);

        return MapToDto(created);
    }

    public async Task<List<ResourceRequestDto>> GetForEngagementAsync(int engagementId)
    {
        var engagement = await resourceRequestRepository.GetEngagementAsync(engagementId)
            ?? throw new EngagementNotFoundException(engagementId);

        var requests = await resourceRequestRepository.GetByEngagementIdAsync(engagementId);
        return requests.Select(MapToDto).ToList();
    }

    public async Task<ResourceRequestDto> AllocateResourcesAsync(int engagementId, int id, AllocateResourcesDto dto)
    {
        var request = await GetOwnedRequestAsync(engagementId, id);

        await EnsureLeadershipOrAdminAsync(engagementId);

        if (request.Status == ResourceRequestStatus.Confirmed)
        {
            throw new ResourceRequestInvalidStateTransitionException(
                "Cannot allocate resources for a request that is already Confirmed.");
        }

        if (dto.Names.Count != request.PeopleNeeded)
        {
            throw new ResourceAllocationCountMismatchException();
        }

        var activeAllocations = request.Allocations
            .Where(a => a.RemovedAt == null)
            .OrderBy(a => a.Id)
            .ToList();

        for (var position = 0; position < dto.Names.Count; position++)
        {
            var name = dto.Names[position].Trim();

            if (position < activeAllocations.Count)
            {
                var existing = activeAllocations[position];
                if (existing.ConfirmedDate == null)
                {
                    existing.Name = name;
                }
                // Confirmed positions are server-authoritative: the submitted name is ignored.
            }
            else
            {
                request.Allocations.Add(new ResourceAllocation { Name = name });
            }
        }

        if (request.Status == ResourceRequestStatus.Raised)
        {
            request.Status = ResourceRequestStatus.Allocated;
            await NotifyPmOfAllocationAsync(request);
        }

        await resourceRequestRepository.SaveChangesAsync();
        return MapToDto(request);
    }

    public async Task<ResourceRequestDto> ConfirmAllocationAsync(int engagementId, int id, int allocationId, ConfirmAllocationDto dto)
    {
        var request = await GetOwnedRequestAsync(engagementId, id);

        await EnsureLeadershipOrAdminAsync(engagementId);

        var allocation = request.Allocations.FirstOrDefault(a => a.Id == allocationId && a.RemovedAt == null)
            ?? throw new ResourceAllocationNotFoundException(allocationId);

        if (allocation.ConfirmedDate != null)
        {
            throw new ResourceAllocationAlreadyConfirmedException();
        }

        var gapBefore = await GetResourcingGapAsync(engagementId);

        allocation.ConfirmedDate = dto.ConfirmedDate;

        var activeAllocations = request.Allocations.Where(a => a.RemovedAt == null).ToList();
        var allConfirmed = activeAllocations.Count == request.PeopleNeeded
            && activeAllocations.All(a => a.ConfirmedDate != null);

        if (allConfirmed)
        {
            request.Status = ResourceRequestStatus.Confirmed;
            await NotifyPmOfConfirmationAsync(request);
        }

        await resourceRequestRepository.SaveChangesAsync();

        var gapAfter = await GetResourcingGapAsync(engagementId);
        await CheckAndHandleGapTransitionAsync(engagementId, gapBefore, gapAfter);

        return MapToDto(request);
    }

    public async Task<ResourceRequestDto> RemoveAllocationAsync(int engagementId, int id, int allocationId, RemoveAllocationDto dto)
    {
        var request = await GetOwnedRequestAsync(engagementId, id);

        await EnsureLeadershipOrAdminAsync(engagementId);

        var allocation = request.Allocations.FirstOrDefault(a => a.Id == allocationId && a.RemovedAt == null)
            ?? throw new ResourceAllocationNotFoundException(allocationId);

        if (allocation.ConfirmedDate == null)
        {
            throw new ResourceAllocationNotConfirmedException();
        }

        var gapBefore = await GetResourcingGapAsync(engagementId);

        allocation.RemovedAt = DateTime.UtcNow;
        allocation.RemovalReason = dto.Reason.Trim();

        if (request.Status == ResourceRequestStatus.Confirmed)
        {
            request.Status = ResourceRequestStatus.Allocated;
            await NotifyPmOfReopeningAsync(request);
        }

        await resourceRequestRepository.SaveChangesAsync();

        var gapAfter = await GetResourcingGapAsync(engagementId);
        await CheckAndHandleGapTransitionAsync(engagementId, gapBefore, gapAfter);

        return MapToDto(request);
    }

    private async Task<ResourceRequest> GetOwnedRequestAsync(int engagementId, int id)
    {
        var request = await resourceRequestRepository.GetByIdAsync(id)
            ?? throw new ResourceRequestNotFoundException(id);

        if (request.EngagementId != engagementId)
        {
            throw new ResourceRequestNotFoundException(id);
        }

        return request;
    }

    private async Task EnsureLeadershipOrAdminAsync(int engagementId)
    {
        if (currentUserService.IsAdmin())
        {
            return;
        }

        var assignment = await resourceRequestRepository.GetUserEngagementAssignmentAsync(engagementId, currentUserService.GetUserId());
        if (assignment == null || assignment.User.RoleId != (int)RoleEnum.Leadership)
        {
            throw new ResourceAllocationAccessDeniedException();
        }
    }

    private async Task NotifyPmOfAllocationAsync(ResourceRequest request)
    {
        var data = JsonSerializer.Serialize(
            new ResourceRequestAllocatedMapper.ResourceRequestAllocatedData(request.Id, request.EngagementId));

        await notificationService.CreateFromTemplateAsync(
            "ResourceRequestAllocated",
            request.RequestedById,
            data,
            DateTime.UtcNow);
    }

    private async Task NotifyPmOfConfirmationAsync(ResourceRequest request)
    {
        var data = JsonSerializer.Serialize(
            new ResourceRequestConfirmedMapper.ResourceRequestConfirmedData(request.Id, request.EngagementId));

        await notificationService.CreateFromTemplateAsync(
            "ResourceRequestConfirmed",
            request.RequestedById,
            data,
            DateTime.UtcNow);
    }

    private async Task NotifyPmOfReopeningAsync(ResourceRequest request)
    {
        var data = JsonSerializer.Serialize(
            new ResourceRequestReopenedMapper.ResourceRequestReopenedData(request.Id, request.EngagementId));

        await notificationService.CreateFromTemplateAsync(
            "ResourceRequestReopened",
            request.RequestedById,
            data,
            DateTime.UtcNow);
    }

    private async Task NotifyLeadershipAsync(int engagementId, int resourceRequestId)
    {
        var leadershipAssignments = await resourceRequestRepository.GetLeadershipAssignmentsAsync(engagementId);
        if (leadershipAssignments.Count == 0)
        {
            return;
        }

        var data = JsonSerializer.Serialize(new ResourceRequestRaisedMapper.ResourceRequestRaisedData(resourceRequestId, engagementId));

        await notificationService.CreateFromTemplateAsync(
            "ResourceRequestRaised",
            leadershipAssignments.Select(a => a.UserId),
            data,
            DateTime.UtcNow);
    }

    private static bool CalculateIsUrgent(DateOnly requiredByDate)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        if (requiredByDate <= today)
        {
            return true;
        }

        var workingDays = 0;
        var current = today.AddDays(1);
        while (current <= requiredByDate)
        {
            if (current.DayOfWeek != DayOfWeek.Saturday && current.DayOfWeek != DayOfWeek.Sunday)
            {
                workingDays++;
            }
            current = current.AddDays(1);
        }

        return workingDays <= ResourceRequestConstants.UrgentThresholdWorkingDays;
    }

    private static ResourceRequestDto MapToDto(ResourceRequest entity)
    {
        var roleName = ResourceRequestConstants.StandardRoleDisplayNames.TryGetValue(entity.StandardRole, out var rName)
            ? rName
            : entity.StandardRole.ToString();

        var statusName = ResourceRequestConstants.StatusDisplayNames.TryGetValue(entity.Status, out var sName)
            ? sName
            : entity.Status.ToString();

        var skillNames = entity.RequestSkills
            .Select(rs => ResourceRequestConstants.SkillDisplayNames.TryGetValue(rs.Skill, out var skName) ? skName : rs.Skill.ToString())
            .ToList();

        var allocations = entity.Allocations
            .Where(a => a.RemovedAt == null)
            .OrderBy(a => a.Id)
            .Select(a => new ResourceAllocationDto
            {
                Id = a.Id,
                Name = a.Name,
                AllocatedDate = DateOnly.FromDateTime(a.CreatedAt),
                IsConfirmed = a.ConfirmedDate != null,
                ConfirmedDate = a.ConfirmedDate
            })
            .ToList();

        return new ResourceRequestDto
        {
            Id = entity.Id,
            EngagementId = entity.EngagementId,
            StandardRole = roleName,
            Skills = skillNames,
            PeopleNeeded = entity.PeopleNeeded,
            RequiredByDate = entity.RequiredByDate,
            JobDescription = entity.JobDescription,
            Status = statusName,
            RequestedById = entity.RequestedById,
            IsUrgent = CalculateIsUrgent(entity.RequiredByDate),
            Allocations = allocations
        };
    }

    public async Task<ResourcingGapDto> GetResourcingGapAsync(int engagementId)
    {
        var engagement = await resourceRequestRepository.GetEngagementAsync(engagementId)
            ?? throw new EngagementNotFoundException(engagementId);

        var requests = await resourceRequestRepository.GetByEngagementIdAsync(engagementId);
        var unconfirmedCount = requests.Count(r => r.Status != ResourceRequestStatus.Confirmed);

        if (engagement.BillingStartDate == null)
        {
            return new ResourcingGapDto(false, unconfirmedCount, null);
        }

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var daysToBilling = engagement.BillingStartDate.Value.DayNumber - today.DayNumber;
        var hasGap = daysToBilling <= ResourceRequestConstants.BillingGapThresholdDays && unconfirmedCount > 0;

        return new ResourcingGapDto(hasGap, unconfirmedCount, daysToBilling);
    }

    private async Task CheckAndHandleGapTransitionAsync(int engagementId, ResourcingGapDto before, ResourcingGapDto after)
    {
        if (!before.HasGap && after.HasGap)
        {
            await auditLogService.LogAsync(
                AuditActionEnum.ResourcingGapFlagRaised,
                new Dictionary<string, object?>
                {
                    ["EngagementId"] = engagementId,
                    ["UnconfirmedCount"] = after.UnconfirmedCount,
                    ["DaysToBilling"] = after.DaysToBilling ?? 0
                });

            var assignments = await resourceRequestRepository.GetPmAndLeadershipAssignmentsAsync(engagementId);
            var notifData = JsonSerializer.Serialize(
                new ResourcingGapFlaggedNotificationMapper.ResourcingGapFlaggedNotificationData(
                    engagementId,
                    after.UnconfirmedCount,
                    after.DaysToBilling ?? 0));

            foreach (var assignment in assignments)
            {
                await notificationService.CreateFromTemplateAsync(
                    NotificationConstants.TemplateKeys.ResourcingGapFlagRaised,
                    assignment.UserId,
                    notifData,
                    DateTime.UtcNow);
            }
        }
        else if (before.HasGap && !after.HasGap)
        {
            await auditLogService.LogAsync(
                AuditActionEnum.ResourcingGapFlagCleared,
                new Dictionary<string, object?>
                {
                    ["EngagementId"] = engagementId
                });
        }
    }
}
