using Microsoft.EntityFrameworkCore;
using Pdr.Api.Constants;
using Pdr.Api.Data;
using Pdr.Api.DTOs.Engagements;
using Pdr.Api.Exceptions;
using Pdr.Api.Models;
using Pdr.Api.Models.Engagements;
using Pdr.Api.Models.Handover;
using Pdr.Api.Models.LoginEntity;
using Pdr.Api.Models.EngagementAssignment;
using Pdr.Api.Models.Risks;
using Pdr.Api.Repositories.Interfaces;
using Pdr.Api.Services.Interfaces;
using Pdr.Api.Services.Notifications.Mappers;

namespace Pdr.Api.Services;

public class EngagementService(
    AppDbContext db,
    IEngagementRepository engagementRepository,
    ICurrentUserService currentUserService,
    IEngagementChecklistService engagementChecklistService,
    INotificationService notificationService,
    IKickoffReadinessService kickoffReadinessService) : IEngagementService
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

        if (!await db.EngagementTypes.AnyAsync(t => t.Id == request.EngagementTypeId))
            throw new InvalidEngagementTypeException(request.EngagementTypeId);

        if (!await db.CommercialModels.AnyAsync(m => m.Id == request.CommercialModelId))
            throw new InvalidCommercialModelException(request.CommercialModelId);

        var warnings = new List<string>();
        var clientName = request.ClientName;

        if (request.KickoffDate < DateOnly.FromDateTime(DateTime.UtcNow))
            warnings.Add(EngagementValidationConstants.Warnings.KickoffDateInPast);

        var existing = await db.Engagements.FirstOrDefaultAsync(e =>
            e.EngagementName == request.EngagementName &&
            e.ClientName == clientName &&
            e.EngagementTypeId == request.EngagementTypeId);

        Engagement engagement;
        if (existing is not null)
        {
            existing.CommercialModelId = request.CommercialModelId;
            existing.KickoffDate = request.KickoffDate;
            existing.BillingStartDate = request.BillingStartDate;
            warnings.Add(EngagementValidationConstants.Warnings.EngagementAlreadyExistsUpdated);
            engagement = existing;
        }
        else
        {
            var defaultStatus = await db.EngagementStatuses.FirstAsync(s => s.StatusName == DefaultEngagementStatus);

            engagement = new Engagement
            {
                EngagementName = request.EngagementName,
                ClientName = clientName,
                EngagementDescription = request.EngagementDescription,
                EngagementTypeId = request.EngagementTypeId,
                CommercialModelId = request.CommercialModelId,
                KickoffDate = request.KickoffDate,
                BillingStartDate = request.BillingStartDate,
                StatusId = defaultStatus.StatusId,
            };
            db.Engagements.Add(engagement);
        }

        await db.SaveChangesAsync();

        try
        {
            var currentUserId = currentUserService.GetUserId();
            if (currentUserId > 0)
            {
                var hasAssignment = await db.EngagementAssignments
                    .AnyAsync(ea => ea.EngagementId == engagement.EngagementId && ea.UserId == currentUserId);
                if (!hasAssignment)
                {
                    db.EngagementAssignments.Add(new EngagementAssignment
                    {
                        EngagementId = engagement.EngagementId,
                        UserId = currentUserId
                    });
                    await db.SaveChangesAsync();
                }
            }
        }
        catch (InvalidOperationException)
        {
            // Fallback for unauthenticated test calls
        }


        // Apply master checklist snapshot and technical readiness items for new engagements
        if (existing is null)
        {
            await engagementChecklistService.ApplyMasterChecklistToEngagementAsync(engagement.EngagementId);
            await InitializeTechnicalReadinessItemsAsync(engagement.EngagementId);
        }

        return new MarkSowSignedResponse(engagement, warnings);
    }

    public async Task<IEnumerable<EngagementTypeDto>> GetEngagementTypesAsync() =>
        await db.EngagementTypes
            .Select(t => new EngagementTypeDto(t.Id, t.Name))
            .ToListAsync();

    public async Task<IEnumerable<CommercialModelDto>> GetCommercialModelsAsync() =>
        await db.CommercialModels
            .Select(m => new CommercialModelDto(m.Id, m.Name))
            .ToListAsync();

    public async Task<EngagementInfoDto?> GetEngagementInfoAsync(int id) =>
        await db.Engagements
            .Where(e => e.EngagementId == id)
            .Select(e => new EngagementInfoDto(
                e.EngagementId,
                e.EngagementName,
                e.ClientName,
                e.EngagementDescription,
                e.EngagementType.Name,
                e.CommercialModel.Name,
                e.KickoffDate,
                e.BillingStartDate,
                e.Status.StatusName,
                e.IsKickoffReady,
                e.HandoverInfo == null
                    ? null
                    : e.HandoverInfo.StatusId == (int)HandoverStatusEnum.Submitted
                        ? HandoverStatusNames.Submitted
                        : HandoverStatusNames.Draft,
                e.HandoverInfo == null ? null : e.HandoverInfo.ReviewedAt,
                e.HandoverInfo == null ? null : e.HandoverInfo.ReviewedByUser!.FullName))
            .FirstOrDefaultAsync();

    public async Task<IEnumerable<EngagementInfoDto>> GetAllEngagementsAsync() =>
        await db.Engagements
            .OrderBy(e => e.EngagementName)
            .Select(e => new EngagementInfoDto(
                e.EngagementId,
                e.EngagementName,
                e.ClientName,
                e.EngagementDescription,
                e.EngagementType.Name,
                e.CommercialModel.Name,
                e.KickoffDate,
                e.BillingStartDate,
                e.Status.StatusName,
                e.IsKickoffReady,
                e.HandoverInfo == null
                    ? null
                    : e.HandoverInfo.StatusId == (int)HandoverStatusEnum.Submitted
                        ? HandoverStatusNames.Submitted
                        : HandoverStatusNames.Draft,
                e.HandoverInfo == null ? null : e.HandoverInfo.ReviewedAt,
                e.HandoverInfo == null ? null : e.HandoverInfo.ReviewedByUser!.FullName))
            .ToListAsync();
    

    public async Task<bool> EditEngagementAsync(int id, EditEngagementRequest request)
    {
        var engagement = await db.Engagements.FindAsync(id);
        if (engagement is null) return false;

        if (request.EngagementName is not null) engagement.EngagementName = request.EngagementName;
        if (request.ClientName is not null) engagement.ClientName = request.ClientName;
        if (request.EngagementDescription is not null) engagement.EngagementDescription = request.EngagementDescription;
        if (request.EngagementTypeId is not null)
        {
            if (!await db.EngagementTypes.AnyAsync(t => t.Id == request.EngagementTypeId.Value))
                throw new InvalidEngagementTypeException(request.EngagementTypeId.Value);

            engagement.EngagementTypeId = request.EngagementTypeId.Value;
        }
        if (request.CommercialModelId is not null)
        {
            if (!await db.CommercialModels.AnyAsync(m => m.Id == request.CommercialModelId.Value))
                throw new InvalidCommercialModelException(request.CommercialModelId.Value);

            engagement.CommercialModelId = request.CommercialModelId.Value;
        }
        if (request.KickoffDate is not null) engagement.KickoffDate = request.KickoffDate.Value;
        if (request.BillingStartDate is not null) engagement.BillingStartDate = request.BillingStartDate;

        var activeStatus = await db.EngagementStatuses.FirstAsync(s => s.StatusName == EngagementStatusEnum.Active.ToString());
        engagement.StatusId = activeStatus.StatusId;

        await db.SaveChangesAsync();
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
                ReadinessScore: null))
            .ToList();

        return new EngagementListResponse(items);
    }
    //assign

    public async Task<EngagementOwnersDto> GetEngagementOwnersAsync(int engagementId)
    {
        if (!await db.Engagements.AnyAsync(e => e.EngagementId == engagementId))
            throw new EngagementNotFoundException(engagementId);

        var assignments = await engagementRepository.GetAssignmentsAsync(engagementId);
        return BuildOwnersDto(assignments);
    }
    

    public async Task<EngagementOwnersDto> AssignProjectManagerAsync(int engagementId, int userId) =>
        await AssignOwnerAsync(engagementId, userId, RoleEnum.ProjectManagement);

    public async Task<EngagementOwnersDto> AssignTechLeadAsync(int engagementId, int userId) =>
        await AssignOwnerAsync(engagementId, userId, RoleEnum.TechLead);

    private async Task<EngagementOwnersDto> AssignOwnerAsync(int engagementId, int userId, RoleEnum requiredRole)
    {
        if (!await db.Engagements.AnyAsync(e => e.EngagementId == engagementId))
            throw new EngagementNotFoundException(engagementId);

        var user = await db.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == userId)
            ?? throw new UserNotFoundException(userId);

        if (user.RoleId != (int)requiredRole)
            throw new InvalidOwnerRoleException(userId, requiredRole.ToRoleName());

        var existingAssignment = await engagementRepository.GetAssignmentByRoleIdAsync(engagementId, (int)requiredRole);
        if (existingAssignment is not null)
        {
            if (existingAssignment.UserId == userId) return await GetEngagementOwnersAsync(engagementId);
            await engagementRepository.RemoveAssignmentAsync(existingAssignment);
        }

        await engagementRepository.AddAssignmentAsync(engagementId, userId);

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
        }
        else if (requiredRole == RoleEnum.TechLead)
        {
            await notificationService.CreateFromTemplateAsync(
                NotificationConstants.TemplateKeys.TechLeadAssigned,
                userId,
                System.Text.Json.JsonSerializer.Serialize(new TechLeadAssignedMapper.TechLeadAssignedPayload(EngagementId: engagementId)),
                DateTime.UtcNow);

            var accessItems = await db.AccessItems.Where(a => a.EngagementId == engagementId).ToListAsync();
            foreach (var item in accessItems)
            {
                item.OwnerId = userId;
            }

            var toolingItems = await db.EnvironmentToolingItems.Where(t => t.EngagementId == engagementId).ToListAsync();
            foreach (var item in toolingItems)
            {
                item.OwnerId = userId;
            }

            await db.SaveChangesAsync();
        }

        return await GetEngagementOwnersAsync(engagementId);
    }

    private async Task InitializeTechnicalReadinessItemsAsync(int engagementId)
    {
        var techLeadAssignment = await db.EngagementAssignments
            .Include(ea => ea.User)
            .FirstOrDefaultAsync(ea => ea.EngagementId == engagementId && ea.User.RoleId == (int)RoleEnum.TechLead);

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

        var activeAccessDefs = await db.AccessItemDefinitions
            .Where(d => d.IsActive)
            .OrderBy(d => d.DisplayOrder)
            .ToListAsync();

        foreach (var def in activeAccessDefs)
        {
            db.AccessItems.Add(new Pdr.Api.Models.AccessTracker.AccessItem
            {
                EngagementId = engagementId,
                AccessItemDefinitionId = def.Id,
                Name = def.Name,
                IsClientSide = def.IsClientSide,
                OwnerId = ownerId,
                Status = Pdr.Api.Models.AccessTracker.AccessItemStatus.InProgress
            });
        }

        var activeToolingDefs = await db.EnvironmentToolingDefinitions
            .Where(d => d.IsActive)
            .OrderBy(d => d.DisplayOrder)
            .ToListAsync();

        foreach (var def in activeToolingDefs)
        {
            db.EnvironmentToolingItems.Add(new Pdr.Api.Models.EnvironmentTooling.EnvironmentToolingItem
            {
                EngagementId = engagementId,
                EnvironmentToolingDefinitionId = def.Id,
                Name = def.Name,
                OwnerId = ownerId,
                Status = Pdr.Api.Models.EnvironmentTooling.EnvironmentToolingStatus.NotReady
            });
        }

        db.ArchitectureValidations.Add(new Pdr.Api.Models.ArchitectureValidation.ArchitectureValidation
        {
            EngagementId = engagementId,
            Status = Pdr.Api.Models.ArchitectureValidation.ArchitectureStatus.StillValid
        });

        await db.SaveChangesAsync();
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

    public async Task<List<LeadershipUserDto>> GetLeadershipUsersAsync(int engagementId)
    {
        if (!await db.Engagements.AnyAsync(e => e.EngagementId == engagementId))
            throw new EngagementNotFoundException(engagementId);

        var leadershipAssignments = await engagementRepository.GetLeadershipAssignmentsAsync(engagementId);
        return leadershipAssignments
            .Select(a => new LeadershipUserDto(a.UserId, a.User.FullName, a.User.Email))
            .ToList();
    }

    public async Task<List<LeadershipUserDto>> AssignLeadershipUserAsync(int engagementId, int userId)
    {
        if (!await db.Engagements.AnyAsync(e => e.EngagementId == engagementId))
            throw new EngagementNotFoundException(engagementId);

        var user = await db.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == userId)
            ?? throw new UserNotFoundException(userId);

        if (user.RoleId != (int)RoleEnum.Leadership)
            throw new InvalidOwnerRoleException(userId, RoleEnum.Leadership.ToRoleName());

        var existingAssignment = await engagementRepository.GetAssignmentByUserAsync(engagementId, userId);
        if (existingAssignment is not null)
            throw new LeadershipUserAlreadyAssignedException();

        await engagementRepository.AddAssignmentAsync(engagementId, userId);

        await notificationService.CreateFromTemplateAsync(
            NotificationConstants.TemplateKeys.LeadershipAssigned,
            userId,
            System.Text.Json.JsonSerializer.Serialize(new LeadershipAssignedMapper.LeadershipAssignedPayload(EngagementId: engagementId)),
            DateTime.UtcNow);

        return await GetLeadershipUsersAsync(engagementId);
    }

    public async Task<List<LeadershipUserDto>> AssignLeadershipUsersAsync(int engagementId, List<int> userIds)
    {
        if (!await db.Engagements.AnyAsync(e => e.EngagementId == engagementId))
            throw new EngagementNotFoundException(engagementId);

        foreach (var userId in userIds.Distinct())
        {
            await AssignLeadershipUserAsync(engagementId, userId);
        }

        return await GetLeadershipUsersAsync(engagementId);
    }

    public async Task<List<LeadershipUserDto>> RemoveLeadershipUserAsync(int engagementId, int userId)
    {
        if (!await db.Engagements.AnyAsync(e => e.EngagementId == engagementId))
            throw new EngagementNotFoundException(engagementId);

        var leadershipAssignments = await engagementRepository.GetLeadershipAssignmentsAsync(engagementId);
        var assignmentToRemove = leadershipAssignments.FirstOrDefault(a => a.UserId == userId);
        if (assignmentToRemove is null)
            throw new UserNotFoundException(userId);

        if (leadershipAssignments.Count <= 1)
            throw new LastLeadershipUserRequiredException();

        await engagementRepository.RemoveAssignmentAsync(assignmentToRemove);
        return await GetLeadershipUsersAsync(engagementId);
    }

    public async Task<List<LeadershipEngagementListItemDto>> GetLeadershipDashboardAsync()
    {
        var currentUserId = currentUserService.GetUserId();

        var activeEngagements = await db.Engagements
            .AsNoTracking()
            .Include(e => e.EngagementAssignments)
                .ThenInclude(ea => ea.User)
            .Where(e => e.EngagementAssignments.Any(ea => ea.UserId == currentUserId))
            .OrderBy(e => e.KickoffDate)
            .ToListAsync();

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var items = new List<LeadershipEngagementListItemDto>();

        foreach (var engagement in activeEngagements)
        {
            var pmUser = engagement.EngagementAssignments
                .FirstOrDefault(ea => ea.User.RoleId == (int)RoleEnum.ProjectManagement)?.User;

            var readiness = await kickoffReadinessService.GetReadinessAsync(engagement.EngagementId);
            var readinessScore = readiness.Score;

            var openBlockerCount = await db.Risks
                .AsNoTracking()
                .CountAsync(r => r.EngagementId == engagement.EngagementId &&
                                 (r.Status == RiskStatus.Open || r.Status == RiskStatus.Mitigating) &&
                                 !r.IsDeleted);

            var daysToKickoff = engagement.KickoffDate.DayNumber - today.DayNumber;

            int requiredThreshold;
            if (daysToKickoff > 14)
            {
                requiredThreshold = LeadershipDashboardConstants.Thresholds.TierGreaterThan14;
            }
            else if (daysToKickoff >= 8)
            {
                requiredThreshold = LeadershipDashboardConstants.Thresholds.Tier8To14;
            }
            else if (daysToKickoff >= 4)
            {
                requiredThreshold = LeadershipDashboardConstants.Thresholds.Tier4To7;
            }
            else if (daysToKickoff >= 1)
            {
                requiredThreshold = LeadershipDashboardConstants.Thresholds.Tier1To3;
            }
            else
            {
                requiredThreshold = LeadershipDashboardConstants.Thresholds.TierDay0OrPast;
            }

            string status;
            if (readinessScore == 100 && openBlockerCount == 0)
            {
                status = LeadershipDashboardConstants.Statuses.KickoffReady;
            }
            else if (openBlockerCount > 0 || readinessScore < requiredThreshold)
            {
                status = LeadershipDashboardConstants.Statuses.AtRisk;
            }
            else
            {
                status = LeadershipDashboardConstants.Statuses.InProgress;
            }

            items.Add(new LeadershipEngagementListItemDto(
                engagement.EngagementId,
                engagement.EngagementName,
                engagement.ClientName,
                pmUser?.FullName,
                readinessScore,
                openBlockerCount,
                daysToKickoff,
                status));
        }

        // Default sorted by soonest kickoff days (daysToKickoff ascending)
        return items.OrderBy(i => i.DaysToKickoff).ToList();
    }

    public async Task<EngagementOpenItemsSummaryDto?> GetOpenItemsSummaryAsync(int engagementId)
    {
        var exists = await db.Engagements.AnyAsync(e => e.EngagementId == engagementId);
        if (!exists) return null;

        var userId = currentUserService.GetUserId();
        var isAssigned = await db.EngagementAssignments
            .AsNoTracking()
            .AnyAsync(a => a.EngagementId == engagementId && a.UserId == userId);

        if (!isAssigned)
            throw new UserNotAssignedToEngagementException(engagementId);

        return await engagementRepository.GetOpenItemsSummaryAsync(engagementId);
    }
}

