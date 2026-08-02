using Microsoft.EntityFrameworkCore;
using Pdr.Api.Data;
using Pdr.Api.DTOs.Engagements;
using Pdr.Api.Exceptions;
using Pdr.Api.Models;
using Pdr.Api.Repositories.Interfaces;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Services;

public class EngagementService(AppDbContext db, IEngagementRepository engagementRepository, ICurrentUserService currentUserService) : IEngagementService
{
    private static readonly string DefaultEngagementStatus = EngagementStatusEnum.KickoffPending.ToString();

    public async Task<MarkSowSignedResponse> MarkSowSignedAsync(MarkSowSignedRequest request)
    {
        if (request.BillingStartDate is { } billingStartDate && billingStartDate < request.KickoffDate)
            throw new BillingBeforeKickoffException();

        if (!await db.EngagementTypes.AnyAsync(t => t.Id == request.EngagementTypeId))
            throw new InvalidEngagementTypeException(request.EngagementTypeId);

        if (!await db.CommercialModels.AnyAsync(m => m.Id == request.CommercialModelId))
            throw new InvalidCommercialModelException(request.CommercialModelId);

        var warnings = new List<string>();
        var clientName = request.ClientName;
        if (string.IsNullOrWhiteSpace(clientName))
        {
            clientName = string.Empty;
            warnings.Add("Client name needed");
        }

        if (request.KickoffDate < DateOnly.FromDateTime(DateTime.UtcNow))
            warnings.Add("Kickoff date is in the past.");

        var existing = await db.Engagements.FirstOrDefaultAsync(e =>
            e.ClientName == clientName &&
            e.EngagementTypeId == request.EngagementTypeId &&
            e.EngagementDescription == request.EngagementDescription);

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
                e.ClientName,
                e.KickoffDate,
                e.KickoffDate.DayNumber - today.DayNumber,
                ReadinessScore: null))
            .ToList();

        return new PmEngagementListResponse(items);
    }
}
