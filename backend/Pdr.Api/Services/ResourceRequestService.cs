using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Pdr.Api.Constants;
using Pdr.Api.DTOs.ResourceRequests;
using Pdr.Api.Exceptions;
using Pdr.Api.Models.LoginEntity;
using Pdr.Api.Models.ResourceRequests;
using Pdr.Api.Repositories.Interfaces;
using Pdr.Api.Services.Interfaces;
using Pdr.Api.Services.Notifications.Mappers;

namespace Pdr.Api.Services;

public class ResourceRequestService(
    IResourceRequestRepository resourceRequestRepository,
    ICurrentUserService currentUserService,
    INotificationService notificationService) : IResourceRequestService
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

        return MapToDto(created);
    }

    public async Task<List<ResourceRequestDto>> GetForEngagementAsync(int engagementId)
    {
        var engagement = await resourceRequestRepository.GetEngagementAsync(engagementId)
            ?? throw new EngagementNotFoundException(engagementId);

        var requests = await resourceRequestRepository.GetByEngagementIdAsync(engagementId);
        return requests.Select(MapToDto).ToList();
    }

    private async Task NotifyLeadershipAsync(int engagementId, int resourceRequestId)
    {
        var leadershipAssignments = await resourceRequestRepository.GetLeadershipAssignmentsAsync(engagementId);
        var data = JsonSerializer.Serialize(new ResourceRequestRaisedMapper.ResourceRequestRaisedData(resourceRequestId, engagementId));

        foreach (var assignment in leadershipAssignments)
        {
            await notificationService.CreateFromTemplateAsync(
                "ResourceRequestRaised",
                assignment.UserId,
                data,
                DateTime.UtcNow);
        }
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
            IsUrgent = CalculateIsUrgent(entity.RequiredByDate)
        };
    }
}
