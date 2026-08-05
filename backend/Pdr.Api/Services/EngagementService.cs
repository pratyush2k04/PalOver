using Microsoft.EntityFrameworkCore;
using Pdr.Api.Data;
using Pdr.Api.DTOs.Engagements;
using Pdr.Api.Exceptions;
using Pdr.Api.Models;
using Pdr.Api.Models.EngagementAssignment;
using Pdr.Api.Repositories.Interfaces;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Services;

public class EngagementService(
    AppDbContext db,
    IEngagementRepository engagementRepository,
    ICurrentUserService currentUserService,
    IEngagementChecklistService engagementChecklistService) : IEngagementService
{
    private static readonly string DefaultEngagementStatus = EngagementStatusEnum.KickoffPending.ToString();
    private const string ProjectManagerRole = "Project Management";
    private const string TechLeadRole = "Tech Lead";

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
            warnings.Add("Kickoff date is in the past.");

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
            warnings.Add("This engagement already exists - updated instead of duplicated.");
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

        // Apply master checklist snapshot only for new engagements
        if (existing is null)
        {
            await engagementChecklistService.ApplyMasterChecklistToEngagementAsync(engagement.EngagementId);
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
                e.Status.StatusName))
            .FirstOrDefaultAsync();
    

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

    public async Task<PmEngagementListResponse> GetMyEngagementsAsync(string sortBy)
    {
        var userId = currentUserService.GetUserId();
        var engagements = await engagementRepository.GetEngagementsForPmAsync(userId, sortBy);
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var items = engagements
            .Select(e => new PmEngagementListItemDto(
                e.EngagementId,
                e.EngagementName,
                e.ClientName,
                e.EngagementType.Name,
                e.CommercialModel.Name,
                e.KickoffDate,
                e.KickoffDate.DayNumber - today.DayNumber,
                ReadinessScore: null))
            .ToList();

        return new PmEngagementListResponse(items);
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
        await AssignOwnerAsync(engagementId, userId, ProjectManagerRole);

    public async Task<EngagementOwnersDto> AssignTechLeadAsync(int engagementId, int userId) =>
        await AssignOwnerAsync(engagementId, userId, TechLeadRole);

    private async Task<EngagementOwnersDto> AssignOwnerAsync(int engagementId, int userId, string requiredRole)
    {
        if (!await db.Engagements.AnyAsync(e => e.EngagementId == engagementId))
            throw new EngagementNotFoundException(engagementId);

        var user = await db.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == userId)
            ?? throw new UserNotFoundException(userId);

        if (user.Role.Name != requiredRole)
            throw new InvalidOwnerRoleException(userId, requiredRole);

        var existingAssignment = await engagementRepository.GetAssignmentByRoleAsync(engagementId, requiredRole);
        if (existingAssignment is not null)
        {
            if (existingAssignment.UserId == userId) return await GetEngagementOwnersAsync(engagementId);
            await engagementRepository.RemoveAssignmentAsync(existingAssignment);
        }

        await engagementRepository.AddAssignmentAsync(engagementId, userId);
        return await GetEngagementOwnersAsync(engagementId);
    }

    private static EngagementOwnersDto BuildOwnersDto(List<EngagementAssignment> assignments)
    {
        var pm = assignments.FirstOrDefault(a => a.User.Role.Name == ProjectManagerRole);
        var techLead = assignments.FirstOrDefault(a => a.User.Role.Name == TechLeadRole);

        return new EngagementOwnersDto(
            pm?.UserId,
            pm?.User.FullName,
            techLead?.UserId,
            techLead?.User.FullName);
    }
}
