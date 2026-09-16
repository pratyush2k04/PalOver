using Microsoft.EntityFrameworkCore;
using Pdr.Api.Data;
using Pdr.Api.DTOs.Engagements;
using Pdr.Api.Exceptions;
using Pdr.Api.Models;
using Pdr.Api.Models.AccessTracker;
using Pdr.Api.Models.EngagementAssignment;
using Pdr.Api.Models.EngagementChecklist;
using Pdr.Api.Models.Engagements;
using Pdr.Api.Models.Handover;
using Pdr.Api.Models.LoginEntity;
using Pdr.Api.Models.ResourceRequests;
using Pdr.Api.Models.Risks;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Repositories;

public class EngagementRepository(AppDbContext context) : IEngagementRepository
{
    public async Task<List<Engagement>> GetEngagementsForUserAsync(int userId, string sortBy)
    {
        var query = context.Engagements
            .AsNoTracking()
            .Include(e => e.EngagementType)
            .Include(e => e.CommercialModel)
            .Include(e => e.DayOneSignOffRecord)
            .Where(e => e.EngagementAssignments.Any(a => a.UserId == userId));

        query = sortBy switch
        {
            "client" => query.OrderBy(e => e.ClientName),
            _ => query.OrderBy(e => e.KickoffDate)
        };

        return await query.ToListAsync();
    }

    private IQueryable<Engagement> BuildMyEngagementsQuery(int userId, bool isSignedOff)
    {
        return context.Engagements
            .AsNoTracking()
            .Include(e => e.EngagementType)
            .Include(e => e.CommercialModel)
            .Include(e => e.DayOneSignOffRecord)
            .Where(e => e.EngagementAssignments.Any(a => a.UserId == userId)
                     && (isSignedOff ? e.DayOneSignOffRecord != null : e.DayOneSignOffRecord == null));
    }

    private static IQueryable<Engagement> ApplySort(IQueryable<Engagement> query, string sortBy) =>
        sortBy switch
        {
            "client" => query.OrderBy(e => e.ClientName),
            _ => query.OrderBy(e => e.KickoffDate)
        };

    public async Task<(List<Engagement> Items, int TotalCount)> GetEngagementsForUserPageAsync(int userId, bool isSignedOff, string sortBy, int page, int pageSize)
    {
        var query = BuildMyEngagementsQuery(userId, isSignedOff);

        var totalCount = await query.CountAsync();

        var items = await ApplySort(query, sortBy)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<List<Engagement>> GetEngagementsForUserFilteredAsync(int userId, bool isSignedOff, string sortBy)
    {
        var query = BuildMyEngagementsQuery(userId, isSignedOff);
        return await ApplySort(query, sortBy).ToListAsync();
    }

    public async Task<int> GetEngagementsForUserStatusCountAsync(int userId, bool isSignedOff) =>
        await BuildMyEngagementsQuery(userId, isSignedOff).CountAsync();

    public async Task<List<int>> GetEngagementIdsForUserAsync(int userId) =>
        await context.Engagements
            .AsNoTracking()
            .Where(e => e.EngagementAssignments.Any(a => a.UserId == userId))
            .Select(e => e.EngagementId)
            .ToListAsync();

    public async Task<Engagement?> GetByIdAsync(int engagementId) =>
        await context.Engagements
            .Include(e => e.EngagementType)
            .FirstOrDefaultAsync(e => e.EngagementId == engagementId);

    public async Task SaveChangesAsync() =>
        await context.SaveChangesAsync();

    //for assigning
    public async Task<List<EngagementAssignment>> GetAssignmentsAsync(int engagementId) =>
        await context.EngagementAssignments
            .AsNoTracking()
            .Include(a => a.User).ThenInclude(u => u.Role)
            .Include(a => a.EngagementRole)
            .Where(a => a.EngagementId == engagementId)
            .ToListAsync();

    public async Task<List<EngagementAssignment>> GetLeadershipAssignmentsAsync(int engagementId) =>
        await context.EngagementAssignments
            .AsNoTracking()
            .Include(a => a.User).ThenInclude(u => u.Role)
            .Include(a => a.EngagementRole)
            .Where(a => a.EngagementId == engagementId && (a.EngagementRoleId == (int)EngagementRoleEnum.Leadership || a.User.RoleId == (int)RoleEnum.Leadership))
            .ToListAsync();

    public async Task<EngagementAssignment?> GetAssignmentByRoleIdAsync(int engagementId, int roleId) =>
        await context.EngagementAssignments
            .Include(a => a.User).ThenInclude(u => u.Role)
            .Include(a => a.EngagementRole)
            .FirstOrDefaultAsync(a => a.EngagementId == engagementId && (a.EngagementRoleId == roleId || a.User.RoleId == roleId));

    public async Task<EngagementAssignment?> GetAssignmentByEngagementRoleIdAsync(int engagementId, int engagementRoleId) =>
        await context.EngagementAssignments
            .Include(a => a.User).ThenInclude(u => u.Role)
            .Include(a => a.EngagementRole)
            .FirstOrDefaultAsync(a => a.EngagementId == engagementId && a.EngagementRoleId == engagementRoleId);

    public async Task<EngagementAssignment?> GetAssignmentByUserAsync(int engagementId, int userId) =>
        await context.EngagementAssignments
            .Include(a => a.User).ThenInclude(u => u.Role)
            .Include(a => a.EngagementRole)
            .FirstOrDefaultAsync(a => a.EngagementId == engagementId && a.UserId == userId);

    public async Task RemoveAssignmentAsync(EngagementAssignment assignment)
    {
        var tracked = await context.EngagementAssignments.FindAsync(assignment.UserId, assignment.EngagementId, assignment.EngagementRoleId);
        if (tracked != null)
        {
            context.EngagementAssignments.Remove(tracked);
            await context.SaveChangesAsync();
        }
    }

    public async Task AddAssignmentAsync(int engagementId, int userId, int engagementRoleId = (int)EngagementRoleEnum.PreSales)
    {
        context.EngagementAssignments.Add(new EngagementAssignment
        {
            EngagementId = engagementId,
            UserId = userId,
            EngagementRoleId = engagementRoleId
        });
        await context.SaveChangesAsync();
    }

    public async Task<EngagementOpenItemsSummaryDto> GetOpenItemsSummaryAsync(int engagementId)
    {
        var checklistOpenCount = await context.Set<EngagementChecklistItem>()
            .AsNoTracking()
            .Where(i => i.EngagementChecklist.EngagementId == engagementId
                        && i.IsMandatory
                        && i.IsApplicable
                        && i.Status == "Incomplete")
            .CountAsync();

        var accessOpenCount = await context.Set<AccessItem>()
            .AsNoTracking()
            .Where(a => a.EngagementId == engagementId
                        && a.Status != AccessItemStatus.Granted)
            .CountAsync();

        var openRisksCount = await context.Set<Risk>()
            .AsNoTracking()
            .Where(r => r.EngagementId == engagementId
                        && r.Status != RiskStatus.Closed)
            .CountAsync();

        var resourcingOpenCount = await context.Set<ResourceRequest>()
            .AsNoTracking()
            .Where(r => r.EngagementId == engagementId
                        && r.Status != ResourceRequestStatus.Confirmed)
            .CountAsync();

        return new EngagementOpenItemsSummaryDto
        {
            ChecklistOpenCount = checklistOpenCount,
            AccessOpenCount = accessOpenCount,
            OpenRisksCount = openRisksCount,
            ResourcingOpenCount = resourcingOpenCount
        };
    }

    public async Task<Dictionary<int, Engagement>> GetByIdsAsync(IEnumerable<int> engagementIds) =>
        await context.Engagements
            .Where(e => engagementIds.Contains(e.EngagementId) && !e.IsDeleted)
            .ToDictionaryAsync(e => e.EngagementId);

    public async Task<IEnumerable<EngagementTypeDto>> GetEngagementTypesAsync() =>
        await context.EngagementTypes
            .Select(t => new EngagementTypeDto(t.Id, t.Name))
            .ToListAsync();

    public async Task<IEnumerable<CommercialModelDto>> GetCommercialModelsAsync() =>
        await context.CommercialModels
            .Select(m => new CommercialModelDto(m.Id, m.Name))
            .ToListAsync();

    public async Task<bool> EngagementTypeExistsAsync(int id) =>
        await context.EngagementTypes.AnyAsync(t => t.Id == id);

    public async Task<bool> CommercialModelExistsAsync(int id) =>
        await context.CommercialModels.AnyAsync(m => m.Id == id);

    public async Task<bool> EngagementExistsAsync(int id) =>
        await context.Engagements.AnyAsync(e => e.EngagementId == id);

    public async Task<Engagement?> GetMatchingEngagementAsync(string name, string client, int typeId) =>
        await context.Engagements.FirstOrDefaultAsync(e =>
            e.EngagementName == name &&
            e.ClientName == client &&
            e.EngagementTypeId == typeId);

    public async Task<int> GetStatusIdByNameAsync(string statusName) =>
        (await context.EngagementStatuses.FirstAsync(s => s.StatusName == statusName)).StatusId;

    public async Task AddEngagementAsync(Engagement engagement)
    {
        context.Engagements.Add(engagement);
        await context.SaveChangesAsync();
    }

    public async Task<EngagementInfoDto?> GetEngagementInfoAsync(int id) =>
        await context.Engagements
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
                e.HandoverInfo == null ? null : e.HandoverInfo.ReviewedByUser!.FullName,
                e.DayOneSignOffRecord != null
                    ? new DayOneSignOffDto(
                        e.DayOneSignOffRecord.SignedOffByUserId,
                        e.DayOneSignOffRecord.SignedOffByUser.FullName,
                        e.DayOneSignOffRecord.BillingStartDate,
                        e.DayOneSignOffRecord.SignedOffAt,
                        e.DayOneSignOffRecord.IsException,
                        e.DayOneSignOffRecord.ExceptionJustification)
                    : null))
            .FirstOrDefaultAsync();

    public async Task<IEnumerable<EngagementInfoDto>> GetAllEngagementInfosAsync() =>
        await context.Engagements
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
                e.HandoverInfo == null ? null : e.HandoverInfo.ReviewedByUser!.FullName,
                e.DayOneSignOffRecord != null
                    ? new DayOneSignOffDto(
                        e.DayOneSignOffRecord.SignedOffByUserId,
                        e.DayOneSignOffRecord.SignedOffByUser.FullName,
                        e.DayOneSignOffRecord.BillingStartDate,
                        e.DayOneSignOffRecord.SignedOffAt,
                        e.DayOneSignOffRecord.IsException,
                        e.DayOneSignOffRecord.ExceptionJustification)
                    : null))
            .ToListAsync();

    public async Task AssignTechLeadOwnerItemsAsync(int engagementId, int userId)
    {
        var accessItems = await context.AccessItems.Where(a => a.EngagementId == engagementId).ToListAsync();
        foreach (var item in accessItems)
        {
            item.OwnerId = userId;
        }

        var toolingItems = await context.EnvironmentToolingItems.Where(t => t.EngagementId == engagementId).ToListAsync();
        foreach (var item in toolingItems)
        {
            item.OwnerId = userId;
        }

        await context.SaveChangesAsync();
    }

    public async Task InitializeTechnicalReadinessItemsAsync(int engagementId, int ownerId)
    {
        var activeAccessDefs = await context.AccessItemDefinitions
            .Where(d => d.IsActive)
            .OrderBy(d => d.DisplayOrder)
            .ToListAsync();

        foreach (var def in activeAccessDefs)
        {
            context.AccessItems.Add(new Pdr.Api.Models.AccessTracker.AccessItem
            {
                EngagementId = engagementId,
                AccessItemDefinitionId = def.Id,
                Name = def.Name,
                IsClientSide = def.IsClientSide,
                OwnerId = ownerId,
                Status = Pdr.Api.Models.AccessTracker.AccessItemStatus.InProgress
            });
        }

        var activeToolingDefs = await context.EnvironmentToolingDefinitions
            .Where(d => d.IsActive)
            .OrderBy(d => d.DisplayOrder)
            .ToListAsync();

        foreach (var def in activeToolingDefs)
        {
            context.EnvironmentToolingItems.Add(new Pdr.Api.Models.EnvironmentTooling.EnvironmentToolingItem
            {
                EngagementId = engagementId,
                EnvironmentToolingDefinitionId = def.Id,
                Name = def.Name,
                OwnerId = ownerId,
                Status = Pdr.Api.Models.EnvironmentTooling.EnvironmentToolingStatus.InProgress
            });
        }

        context.ArchitectureValidations.Add(new Pdr.Api.Models.ArchitectureValidation.ArchitectureValidation
        {
            EngagementId = engagementId,
            Status = Pdr.Api.Models.ArchitectureValidation.ArchitectureStatus.StillValid
        });

        await context.SaveChangesAsync();
    }

    public async Task<EvidenceSnapshotDto> GetEvidenceSnapshotAsync(int engagementId)
    {
        var snapshot = await context.EvidenceSnapshots
            .AsNoTracking()
            .Include(s => s.Items)
            .FirstOrDefaultAsync(s => s.EngagementId == engagementId)
            ?? throw new EvidenceSnapshotNotFoundException(engagementId);

        return new EvidenceSnapshotDto(
            snapshot.SignedOffAt,
            snapshot.SignedOffByUserName,
            snapshot.BillingStartDate,
            snapshot.Items
                .OrderBy(i => i.DisplayOrder)
                .Select(i => new EvidenceSnapshotItemDto(
                    i.Title,
                    i.CategoryName,
                    i.StatusAtSignOff,
                    i.EvidenceDocumentId,
                    i.EvidenceDocumentFileName,
                    i.EvidenceSubmittedAt,
                    i.EvidenceSubmittedByName,
                    i.OwnerId,
                    i.OwnerName))
                .ToList());
    }

    public async Task<bool> IsUserAssignedAsync(int engagementId, int userId) =>
        await context.EngagementAssignments
            .AsNoTracking()
            .AnyAsync(a => a.EngagementId == engagementId && a.UserId == userId);

    public async Task<List<EngagementReportInfoDto>> GetEngagementReportInfoAsync(List<int> engagementIds) =>
        await context.Engagements
            .AsNoTracking()
            .Where(e => engagementIds.Contains(e.EngagementId))
            .Select(e => new EngagementReportInfoDto(e.EngagementId, e.EngagementTypeId, e.KickoffDate))
            .ToListAsync();

    public async Task<List<EngagementAssignment>> GetAssignmentsForEngagementsAsync(List<int> engagementIds) =>
        await context.EngagementAssignments
            .AsNoTracking()
            .Include(a => a.User)
            .Include(a => a.EngagementRole)
            .Where(a => engagementIds.Contains(a.EngagementId))
            .ToListAsync();

    public async Task<bool> IsEngagementSignedOffAsync(int engagementId) =>
        await context.DayOneSignOffRecords
            .AsNoTracking()
            .AnyAsync(r => r.EngagementId == engagementId);

    public async Task EnsureNotSignedOffAsync(int engagementId)
    {
        if (await IsEngagementSignedOffAsync(engagementId))
        {
            throw new EngagementSignedOffException();
        }
    }

    public async Task<bool> HasAnyAssignmentAsync(int engagementId) =>
        await context.EngagementAssignments
            .AsNoTracking()
            .AnyAsync(a => a.EngagementId == engagementId);

    public async Task<Engagement?> GetForOnePagerAsync(int engagementId) =>
        await context.Engagements
            .AsNoTracking()
            .Include(e => e.CommercialModel)
            .Include(e => e.DayOneSignOffRecord)
            .FirstOrDefaultAsync(e => e.EngagementId == engagementId);
}
