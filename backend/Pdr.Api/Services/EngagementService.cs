using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pdr.Api.Constants;
using Pdr.Api.DTOs.Engagements;
using Pdr.Api.Exceptions;
using Pdr.Api.Helpers;
using Pdr.Api.Models.Audit;
using Pdr.Api.Models.Engagements;
using Pdr.Api.Models.EngagementAssignment;
using Pdr.Api.Models.Handover;
using Pdr.Api.Models.LoginEntity;
using Pdr.Api.Models.People;
using Pdr.Api.Repositories.Interfaces;
using Pdr.Api.Services.Interfaces;
using Pdr.Api.Services.Notifications.Mappers;

namespace Pdr.Api.Services;

public class EngagementService(
    IEngagementRepository engagementRepository,
    IUserRepository userRepository,
    ICurrentUserService currentUserService,
    IEngagementChecklistService engagementChecklistService,
    INotificationService notificationService,
    IKickoffReadinessService kickoffReadinessService,
    IPersonRepository personRepository,
    IAuditLogService auditLogService,
    ITechnicalSummaryService technicalSummaryService,
    IEngagementAccessService engagementAccessService,
    IHandoverRepository handoverRepository,
    ICommitmentRepository commitmentRepository,
    IKickoffReminderService kickoffReminderService,
    IEngagementChecklistRepository engagementChecklistRepository) : IEngagementService
{
    private static readonly string DefaultEngagementStatus = EngagementStatusEnum.KickoffPending.ToString();

    public async Task<MarkSowSignedResponse> MarkSowSignedAsync(MarkSowSignedRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.EngagementName))
            throw new EngagementNameRequiredException();

        if (string.IsNullOrWhiteSpace(request.ClientName))
            throw new ClientNameRequiredException();

        if (request.BillingStartDate < request.KickoffDate)
            throw new BillingBeforeKickoffException();

        if (!await engagementRepository.EngagementTypeExistsAsync(request.EngagementTypeId))
            throw new InvalidEngagementTypeException(request.EngagementTypeId);

        if (!await engagementRepository.CommercialModelExistsAsync(request.CommercialModelId))
            throw new InvalidCommercialModelException(request.CommercialModelId);

        var warnings = new List<string>();
        var clientName = request.ClientName;

        if (request.KickoffDate < DateOnly.FromDateTime(DateTime.UtcNow))
            warnings.Add(EngagementValidationConstants.Warnings.KickoffDateInPast);

        var existing = await engagementRepository.GetMatchingEngagementAsync(
            request.EngagementName,
            clientName,
            request.EngagementTypeId);

        Engagement engagement;
        if (existing is not null)
        {
            existing.CommercialModelId = request.CommercialModelId;
            existing.KickoffDate = request.KickoffDate;
            existing.BillingStartDate = request.BillingStartDate;
            warnings.Add(EngagementValidationConstants.Warnings.EngagementAlreadyExistsUpdated);
            engagement = existing;
            await engagementRepository.SaveChangesAsync();
        }
        else
        {
            var defaultStatusId = await engagementRepository.GetStatusIdByNameAsync(DefaultEngagementStatus);

            engagement = new Engagement
            {
                EngagementName = request.EngagementName,
                ClientName = clientName,
                EngagementDescription = request.EngagementDescription,
                EngagementTypeId = request.EngagementTypeId,
                CommercialModelId = request.CommercialModelId,
                KickoffDate = request.KickoffDate,
                BillingStartDate = request.BillingStartDate,
                StatusId = defaultStatusId,
            };
            await engagementRepository.AddEngagementAsync(engagement);
        }

        try
        {
            var currentUserId = currentUserService.GetUserId();
            if (currentUserId > 0)
            {
                var hasAssignment = await engagementRepository.IsUserAssignedAsync(engagement.EngagementId, currentUserId);
                if (!hasAssignment)
                {
                    await engagementRepository.AddAssignmentAsync(engagement.EngagementId, currentUserId);

                    var creator = await userRepository.GetByIdAsync(currentUserId);
                    if (creator is not null && creator.RoleId == (int)RoleEnum.PreSales)
                    {
                        await personRepository.AddAsync(new Person
                        {
                            EngagementId = engagement.EngagementId,
                            Name = creator.FullName,
                            Role = RoleEnum.PreSales.ToRoleName(),
                            PersonType = PersonType.Internal,
                            UserId = creator.Id,
                            SourceRoleId = (int)RoleEnum.PreSales,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        });
                        await personRepository.SaveChangesAsync();
                    }
                }
            }
        }
        catch (InvalidOperationException)
        {
            // Allowed for unauthenticated/system-driven SOW signing
        }

        // Runs after the PreSales assignment bookkeeping above, so a brand-new engagement's
        // signing PreSales user is already visible when ScheduleRemindersAsync checks role assignments.
        await kickoffReminderService.ScheduleRemindersAsync(engagement.EngagementId, engagement.KickoffDate);

        // Apply master checklist snapshot and technical readiness items for new engagements
        if (existing is null)
        {
            await engagementChecklistService.ApplyMasterChecklistToEngagementAsync(engagement.EngagementId);
            await InitializeTechnicalReadinessItemsAsync(engagement.EngagementId);
        }

        return new MarkSowSignedResponse(engagement, warnings);
    }

    public async Task<IEnumerable<EngagementTypeDto>> GetEngagementTypesAsync() =>
        await engagementRepository.GetEngagementTypesAsync();

    public async Task<IEnumerable<CommercialModelDto>> GetCommercialModelsAsync() =>
        await engagementRepository.GetCommercialModelsAsync();

    public async Task<EngagementInfoDto?> GetEngagementInfoAsync(int id)
    {
        await engagementAccessService.ValidateAccessAsync(id);
        return await engagementRepository.GetEngagementInfoAsync(id);
    }

    public async Task<IEnumerable<EngagementInfoDto>> GetAllEngagementsAsync() =>
        await engagementRepository.GetAllEngagementInfosAsync();

    public async Task<bool> EditEngagementAsync(int id, EditEngagementRequest request)
    {
        var engagement = await engagementRepository.GetByIdAsync(id);
        if (engagement is null) return false;

        if (request.EngagementName is not null) engagement.EngagementName = request.EngagementName;
        if (request.ClientName is not null) engagement.ClientName = request.ClientName;
        if (request.EngagementDescription is not null) engagement.EngagementDescription = request.EngagementDescription;
        if (request.EngagementTypeId is not null)
        {
            if (!await engagementRepository.EngagementTypeExistsAsync(request.EngagementTypeId.Value))
                throw new InvalidEngagementTypeException(request.EngagementTypeId.Value);

            engagement.EngagementTypeId = request.EngagementTypeId.Value;
        }
        if (request.CommercialModelId is not null)
        {
            if (!await engagementRepository.CommercialModelExistsAsync(request.CommercialModelId.Value))
                throw new InvalidCommercialModelException(request.CommercialModelId.Value);

            engagement.CommercialModelId = request.CommercialModelId.Value;
        }
        if (request.KickoffDate is not null) engagement.KickoffDate = request.KickoffDate.Value;
        if (request.BillingStartDate is not null) engagement.BillingStartDate = request.BillingStartDate;

        var activeStatusId = await engagementRepository.GetStatusIdByNameAsync(EngagementStatusEnum.Active.ToString());
        engagement.StatusId = activeStatusId;

        await engagementRepository.SaveChangesAsync();

        if (request.KickoffDate is not null)
            await kickoffReminderService.ScheduleRemindersAsync(id, request.KickoffDate.Value);

        return true;
    }

    public async Task<EngagementListResponse> GetMyEngagementsAsync(string sortBy)
    {
        var userId = currentUserService.GetUserId();
        var engagements = await engagementRepository.GetEngagementsForUserAsync(userId, sortBy);
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var items = engagements
            .Select(e => new EngagementListItemDto(
                e.EngagementId,
                e.EngagementName,
                e.ClientName,
                e.EngagementType.Name,
                e.CommercialModel.Name,
                e.KickoffDate,
                e.KickoffDate.DayNumber - today.DayNumber,
                ReadinessScore: null,
                IsSignedOff: e.DayOneSignOffRecord != null))
            .ToList();

        return new EngagementListResponse(items);
    }

    public async Task<EngagementOwnersDto> GetEngagementOwnersAsync(int engagementId)
    {
        if (!await engagementRepository.EngagementExistsAsync(engagementId))
            throw new EngagementNotFoundException(engagementId);

        var assignments = await engagementRepository.GetAssignmentsAsync(engagementId);
        return BuildOwnersDto(assignments);
    }

    public async Task<IEnumerable<Pdr.Api.DTOs.Users.UserListItemDto>> GetEngagementUsersAsync(int engagementId)
    {
        if (!await engagementRepository.EngagementExistsAsync(engagementId))
            throw new EngagementNotFoundException(engagementId);

        var assignments = await engagementRepository.GetAssignmentsAsync(engagementId);
        return assignments.Select(a => new Pdr.Api.DTOs.Users.UserListItemDto
        {
            Id = a.User.Id,
            FullName = a.User.FullName,
            Email = a.User.Email,
            RoleId = a.User.RoleId,
            RoleName = a.User.Role.Name
        }).ToList();
    }

    public async Task<EngagementOwnersDto> AssignProjectManagerAsync(int engagementId, int userId) =>
        await AssignOwnerAsync(engagementId, userId, RoleEnum.ProjectManagement);

    public async Task<EngagementOwnersDto> AssignTechLeadAsync(int engagementId, int userId) =>
        await AssignOwnerAsync(engagementId, userId, RoleEnum.TechLead);

    private async Task<EngagementOwnersDto> AssignOwnerAsync(int engagementId, int userId, RoleEnum requiredRole)
    {
        if (!await engagementRepository.EngagementExistsAsync(engagementId))
            throw new EngagementNotFoundException(engagementId);

        var user = await userRepository.GetByIdAsync(userId)
            ?? throw new UserNotFoundException(userId);

        if (user.RoleId != (int)requiredRole)
            throw new InvalidOwnerRoleException(userId, requiredRole.ToRoleName());

        var existingAssignment = await engagementRepository.GetAssignmentByRoleIdAsync(engagementId, (int)requiredRole);
        var isReassignment = existingAssignment is not null;
        if (existingAssignment is not null)
        {
            if (existingAssignment.UserId == userId) return await GetEngagementOwnersAsync(engagementId);
            await engagementRepository.RemoveAssignmentAsync(existingAssignment);
        }

        await engagementRepository.AddAssignmentAsync(engagementId, userId);
        await SyncOwnerPersonAsync(engagementId, user, requiredRole);

        var actorUserId = currentUserService.GetUserId();
        await auditLogService.LogAsync(
            isReassignment ? AuditActionEnum.PmAndTlReassigned : AuditActionEnum.PmAndTlAssigned,
            new Dictionary<string, object?>
            {
                ["EngagementId"] = engagementId,
                ["ActorUserId"] = actorUserId
            },
            engagementId: engagementId,
            actingUserId: actorUserId > 0 ? actorUserId : null);

        if (requiredRole == RoleEnum.ProjectManagement)
        {
            await notificationService.CreateFromTemplateAsync(
                NotificationConstants.TemplateKeys.PMAllocated,
                engagementId,
                RoleEnum.PreSales,
                System.Text.Json.JsonSerializer.Serialize(new PMAllocatedMapper.PMAllocatedData(AssignedUserId: userId, EngagementId: engagementId)),
                DateTime.UtcNow);

            await notificationService.CreateFromTemplateAsync(
                NotificationConstants.TemplateKeys.PMAssigned,
                userId,
                System.Text.Json.JsonSerializer.Serialize(new PMAssignedMapper.PMAssignedPayload(EngagementId: engagementId)),
                DateTime.UtcNow);

            var handover = await handoverRepository.GetByEngagementIdAsync(engagementId);
            if (handover is not null)
            {
                var pendingCommitmentsCount = await commitmentRepository.GetCountByHandoverIdAsync(handover.HandoverId);
                if (pendingCommitmentsCount > 0)
                {
                    await notificationService.CreateFromTemplateAsync(
                        NotificationConstants.TemplateKeys.PendingCommitmentsSummary,
                        userId,
                        System.Text.Json.JsonSerializer.Serialize(new PendingCommitmentsSummaryMapper.PendingCommitmentsSummaryPayload(engagementId, pendingCommitmentsCount)),
                        DateTime.UtcNow);
                }
            }

            var engagementForReminder = await engagementRepository.GetByIdAsync(engagementId);
            if (engagementForReminder is not null)
                await kickoffReminderService.ScheduleRemindersAsync(engagementId, engagementForReminder.KickoffDate);
        }
        else if (requiredRole == RoleEnum.TechLead)
        {
            await notificationService.CreateFromTemplateAsync(
                NotificationConstants.TemplateKeys.TechLeadAssigned,
                userId,
                System.Text.Json.JsonSerializer.Serialize(new TechLeadAssignedMapper.TechLeadAssignedPayload(EngagementId: engagementId)),
                DateTime.UtcNow);

            await engagementRepository.AssignTechLeadOwnerItemsAsync(engagementId, userId);

            var engagementForReminder = await engagementRepository.GetByIdAsync(engagementId);
            if (engagementForReminder is not null)
                await kickoffReminderService.ScheduleRemindersAsync(engagementId, engagementForReminder.KickoffDate);
        }

        return await GetEngagementOwnersAsync(engagementId);
    }

    private async Task SyncOwnerPersonAsync(int engagementId, User user, RoleEnum role)
    {
        var previousPerson = await personRepository.GetBySourceRoleAsync(engagementId, (int)role);
        if (previousPerson is not null)
        {
            personRepository.Delete(previousPerson);
        }

        await personRepository.AddAsync(new Person
        {
            EngagementId = engagementId,
            Name = user.FullName,
            Role = role.ToRoleName(),
            PersonType = PersonType.Internal,
            UserId = user.Id,
            SourceRoleId = (int)role,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });

        await personRepository.SaveChangesAsync();
    }

    private async Task InitializeTechnicalReadinessItemsAsync(int engagementId)
    {
        var techLeadAssignment = await engagementRepository.GetAssignmentByRoleIdAsync(engagementId, (int)RoleEnum.TechLead);

        int ownerId = techLeadAssignment?.UserId ?? 0;
        if (ownerId == 0)
        {
            try
            {
                ownerId = currentUserService.GetUserId();
            }
            catch (InvalidOperationException)
            {
                ownerId = 1;
            }
        }
        if (ownerId == 0) ownerId = 1;

        await engagementRepository.InitializeTechnicalReadinessItemsAsync(engagementId, ownerId);
    }

    private static EngagementOwnersDto BuildOwnersDto(List<EngagementAssignment> assignments)
    {
        var pm = assignments.FirstOrDefault(a => a.User.RoleId == (int)RoleEnum.ProjectManagement);
        var techLead = assignments.FirstOrDefault(a => a.User.RoleId == (int)RoleEnum.TechLead);
        var leadership = assignments
            .Where(a => a.User.RoleId == (int)RoleEnum.Leadership)
            .Select(a => new LeadershipUserDto(a.UserId, a.User.FullName, a.User.Email))
            .ToList();

        return new EngagementOwnersDto(
            pm?.UserId,
            pm?.User.FullName,
            techLead?.UserId,
            techLead?.User.FullName,
            leadership);
    }

    public async Task<HandoverDraftStatusDto> GetHandoverDraftStatusAsync(int engagementId)
    {
        if (!await engagementRepository.EngagementExistsAsync(engagementId))
            throw new EngagementNotFoundException(engagementId);

        var handover = await handoverRepository.GetByEngagementIdAsync(engagementId);
        if (handover == null || handover.StatusId != (int)HandoverStatusEnum.Submitted)
            return new HandoverDraftStatusDto(false, null, null, null, null);

        var status = await handoverRepository.GetStatusByIdAsync(handover.StatusId);

        return new HandoverDraftStatusDto(
            true,
            status?.StatusName ?? HandoverStatusNames.Draft,
            handover.ClientBackground,
            handover.BusinessProblem,
            handover.ClientSentiment);
    }

    public async Task<List<LeadershipUserDto>> GetLeadershipUsersAsync(int engagementId)
    {
        if (!await engagementRepository.EngagementExistsAsync(engagementId))
            throw new EngagementNotFoundException(engagementId);

        var leadershipAssignments = await engagementRepository.GetLeadershipAssignmentsAsync(engagementId);
        return leadershipAssignments
            .Select(a => new LeadershipUserDto(a.UserId, a.User.FullName, a.User.Email))
            .ToList();
    }

    public async Task<List<LeadershipUserDto>> AssignLeadershipUserAsync(int engagementId, int userId)
    {
        if (!await engagementRepository.EngagementExistsAsync(engagementId))
            throw new EngagementNotFoundException(engagementId);

        var handover = await handoverRepository.GetByEngagementIdAsync(engagementId);
        if (handover == null || handover.StatusId != (int)HandoverStatusEnum.Submitted)
            throw new HandoverIncompleteException(HandoverValidationConstants.NotFoundForLeadershipAllocation);

        var user = await userRepository.GetByIdAsync(userId)
            ?? throw new UserNotFoundException(userId);

        if (user.RoleId != (int)RoleEnum.Leadership)
            throw new InvalidOwnerRoleException(userId, RoleEnum.Leadership.ToRoleName());

        var existingAssignment = await engagementRepository.GetAssignmentByUserAsync(engagementId, userId);
        if (existingAssignment is not null)
            throw new LeadershipUserAlreadyAssignedException();

        await engagementRepository.AddAssignmentAsync(engagementId, userId);

        await personRepository.AddAsync(new Person
        {
            EngagementId = engagementId,
            Name = user.FullName,
            Role = RoleEnum.Leadership.ToRoleName(),
            PersonType = PersonType.Internal,
            UserId = user.Id,
            SourceRoleId = (int)RoleEnum.Leadership,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });
        await personRepository.SaveChangesAsync();
        await notificationService.CreateFromTemplateAsync(
            NotificationConstants.TemplateKeys.LeadershipAssigned,
            userId,
            System.Text.Json.JsonSerializer.Serialize(new LeadershipAssignedMapper.LeadershipAssignedPayload(EngagementId: engagementId)),
            DateTime.UtcNow);
        await notificationService.CreateFromTemplateAsync(
            NotificationConstants.TemplateKeys.LeadershipActionRequired,
            userId,
            System.Text.Json.JsonSerializer.Serialize(new LeadershipActionRequiredMapper.LeadershipActionRequiredPayload(EngagementId: engagementId)),
            DateTime.UtcNow);

        var actorUserId = currentUserService.GetUserId();
        await auditLogService.LogAsync(
            AuditActionEnum.LeadershipAllocated,
            new Dictionary<string, object?>
            {
                ["EngagementId"] = engagementId,
                ["ActorUserId"] = actorUserId
            },
            engagementId: engagementId,
            actingUserId: actorUserId > 0 ? actorUserId : null);

        return await GetLeadershipUsersAsync(engagementId);
    }

    public async Task<List<LeadershipUserDto>> AssignLeadershipUsersAsync(int engagementId, List<int> userIds)
    {
        if (!await engagementRepository.EngagementExistsAsync(engagementId))
            throw new EngagementNotFoundException(engagementId);

        foreach (var userId in userIds.Distinct())
        {
            await AssignLeadershipUserAsync(engagementId, userId);
        }

        return await GetLeadershipUsersAsync(engagementId);
    }

    public async Task<List<LeadershipUserDto>> RemoveLeadershipUserAsync(int engagementId, int userId)
    {
        if (!await engagementRepository.EngagementExistsAsync(engagementId))
            throw new EngagementNotFoundException(engagementId);

        var leadershipAssignments = await engagementRepository.GetLeadershipAssignmentsAsync(engagementId);
        var assignmentToRemove = leadershipAssignments.FirstOrDefault(a => a.UserId == userId);
        if (assignmentToRemove is null)
            throw new UserNotFoundException(userId);

        if (leadershipAssignments.Count <= 1)
            throw new LastLeadershipUserRequiredException();

        await engagementRepository.RemoveAssignmentAsync(assignmentToRemove);

        var person = await personRepository.GetByUserAndSourceRoleAsync(engagementId, userId, (int)RoleEnum.Leadership);
        if (person is not null)
        {
            personRepository.Delete(person);
            await personRepository.SaveChangesAsync();
        }

        return await GetLeadershipUsersAsync(engagementId);
    }

    public async Task<List<LeadershipUserDto>> UpdateLeadershipUsersAsync(int engagementId, List<int> userIds)
    {
        if (!await engagementRepository.EngagementExistsAsync(engagementId))
            throw new EngagementNotFoundException(engagementId);

        if (userIds == null || userIds.Count == 0)
            throw new LastLeadershipUserRequiredException();

        var currentAssignments = await engagementRepository.GetLeadershipAssignmentsAsync(engagementId);
        var currentUserIds = currentAssignments.Select(a => a.UserId).ToList();

        var toAdd = userIds.Except(currentUserIds).ToList();
        var toRemove = currentUserIds.Except(userIds).ToList();

        foreach (var userId in toAdd)
        {
            await AssignLeadershipUserAsync(engagementId, userId);
        }

        foreach (var userId in toRemove)
        {
            await RemoveLeadershipUserAsync(engagementId, userId);
        }

        return await GetLeadershipUsersAsync(engagementId);
    }

    public async Task<List<LeadershipEngagementListItemDto>> GetLeadershipDashboardAsync()
    {
        var currentUserId = currentUserService.GetUserId();

        var engagements = await engagementRepository.GetEngagementsForUserAsync(currentUserId, "kickoff");
        var activeEngagements = engagements.OrderBy(e => e.KickoffDate).ToList();

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var items = new List<LeadershipEngagementListItemDto>();

        foreach (var engagement in activeEngagements)
        {
            var assignments = await engagementRepository.GetAssignmentsAsync(engagement.EngagementId);
            var pmUser = assignments.FirstOrDefault(a => a.User.RoleId == (int)RoleEnum.ProjectManagement)?.User;

            var readiness = await kickoffReadinessService.GetReadinessAsync(engagement.EngagementId);
            var readinessScore = readiness.Score;

            var summary = await engagementRepository.GetOpenItemsSummaryAsync(engagement.EngagementId);
            var openBlockerCount = summary.OpenRisksCount;

            var daysToKickoff = engagement.KickoffDate.DayNumber - today.DayNumber;
            var requiredThreshold = LeadershipDashboardHelpers.GetRequiredThreshold(daysToKickoff);
            var status = LeadershipDashboardHelpers.DeriveStatus(readinessScore, openBlockerCount, requiredThreshold);

            int? daysToBilling = engagement.BillingStartDate.HasValue
                ? engagement.BillingStartDate.Value.DayNumber - today.DayNumber
                : null;

            int unconfirmedResourceCount = summary.ResourcingOpenCount;

            bool hasResourcingGap = LeadershipDashboardHelpers.EvaluateResourcingGap(daysToBilling, unconfirmedResourceCount);

            var fullEngagementInfo = await engagementRepository.GetEngagementInfoAsync(engagement.EngagementId);
            var isDayOneSignedOff = fullEngagementInfo?.DayOneSignOff != null;
            var dayOneSignOffIsException = fullEngagementInfo?.DayOneSignOff?.IsException ?? false;

            items.Add(new LeadershipEngagementListItemDto(
                engagement.EngagementId,
                engagement.EngagementName,
                engagement.ClientName,
                pmUser?.FullName,
                readinessScore,
                openBlockerCount,
                daysToKickoff,
                status,
                hasResourcingGap,
                unconfirmedResourceCount,
                daysToBilling,
                isDayOneSignedOff,
                dayOneSignOffIsException));
        }

        return items.OrderBy(i => i.DaysToKickoff).ToList();
    }

    public async Task<EngagementOpenItemsSummaryDto?> GetOpenItemsSummaryAsync(int engagementId)
    {
        var exists = await engagementRepository.EngagementExistsAsync(engagementId);
        if (!exists) return null;

        await engagementAccessService.ValidateAccessAsync(engagementId);

        return await engagementRepository.GetOpenItemsSummaryAsync(engagementId);
    }

    public async Task<DayOneSignOffDto> SignOffDayOneAsync(int engagementId, DayOneSignOffRequest request)
    {
        var engagement = await engagementRepository.GetByIdAsync(engagementId)
            ?? throw new EngagementNotFoundException(engagementId);

        if (request.BillingStartDate < engagement.KickoffDate)
            throw new BillingBeforeKickoffException();

        var technicalSummary = await technicalSummaryService.GetDayOneSummaryAsync(engagementId);
        if (!technicalSummary.IsAbleToStartDayOne)
        {
            if (!request.IsException)
                throw new DayOneTechnicalBlockersRemainingException();

            var isLeadership = currentUserService.IsInRole(RoleEnum.Leadership);
            if (!isLeadership)
                throw new DayOneLeadershipExceptionForbiddenException();

            if (string.IsNullOrWhiteSpace(request.ExceptionJustification) || request.ExceptionJustification.Trim().Length < 20)
                throw new DayOneExceptionJustificationTooShortException();
        }

        var currentUserId = currentUserService.GetUserId();
        var currentUser = await userRepository.GetByIdAsync(currentUserId)
            ?? throw new UserNotFoundException(currentUserId);

        var now = DateTime.UtcNow;
        engagement.BillingStartDate = request.BillingStartDate;

        engagement.DayOneSignOffRecord = new DayOneSignOffRecord
        {
            EngagementId = engagementId,
            SignedOffByUserId = currentUserId,
            BillingStartDate = request.BillingStartDate,
            SignedOffAt = now,
            IsException = request.IsException,
            ExceptionJustification = request.IsException ? request.ExceptionJustification?.Trim() : null
        };

        var checklist = await engagementChecklistRepository.GetByEngagementIdAsync(engagementId);
        if (checklist != null)
        {
            engagement.EvidenceSnapshot = new EvidenceSnapshot
            {
                EngagementId = engagementId,
                SignedOffByUserId = currentUserId,
                SignedOffByUserName = currentUser.FullName,
                SignedOffAt = now,
                BillingStartDate = request.BillingStartDate,
                Items = checklist.Items
                    .Where(i => i.EvidenceRequired)
                    .Select(i => new EvidenceSnapshotItem
                    {
                        SourceChecklistItemId = i.Id,
                        Title = i.Title,
                        CategoryName = i.Category?.CategoryName ?? string.Empty,
                        DisplayOrder = i.DisplayOrder,
                        StatusAtSignOff = !i.IsApplicable
                            ? EvidenceSnapshotItemStatus.NotApplicable
                            : i.Status == "Complete"
                                ? EvidenceSnapshotItemStatus.Complete
                                : EvidenceSnapshotItemStatus.Open,
                        EvidenceNote = i.EvidenceNote,
                        EvidenceLink = i.EvidenceLink,
                        EvidenceSubmittedAt = i.EvidenceSubmittedAt,
                        EvidenceSubmittedByName = i.EvidenceSubmittedBy?.FullName
                    }).ToList()
            };
        }

        await engagementRepository.SaveChangesAsync();

        return new DayOneSignOffDto(
            currentUserId,
            currentUser.FullName,
            request.BillingStartDate,
            now,
            request.IsException,
            request.IsException ? request.ExceptionJustification?.Trim() : null);
    }

    public async Task<EvidenceSnapshotDto> GetEvidenceSnapshotAsync(int engagementId)
    {
        await engagementAccessService.ValidateAccessAsync(engagementId);
        return await engagementRepository.GetEvidenceSnapshotAsync(engagementId);
    }
}
