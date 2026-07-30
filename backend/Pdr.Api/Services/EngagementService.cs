using Microsoft.EntityFrameworkCore;
using Pdr.Api.Data;
using Pdr.Api.DTOs.Engagements;
using Pdr.Api.Models;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Services;

public class EngagementService(AppDbContext db) : IEngagementService
{
    private const string DefaultEngagementStatus = "Kickoff Pending";

    public async Task<MarkSowSignedResponse> MarkSowSignedAsync(MarkSowSignedRequest request)
    {
        if (request.BillingStartDate is { } billingStartDate && billingStartDate < request.KickoffDate)
            throw new BillingBeforeKickoffException();

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
            existing.KickoffDate = request.KickoffDate;
            existing.BillingStartDate = request.BillingStartDate;
            existing.UpdatedBy = request.SignedByUserId;
            existing.UpdatedOn = DateTime.UtcNow;
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
                KickoffDate = request.KickoffDate,
                BillingStartDate = request.BillingStartDate,
                StatusId = defaultStatus.StatusId,
                CreatedBy = request.SignedByUserId,
                CreatedOn = DateTime.UtcNow,
            };
            db.Engagements.Add(engagement);
        }

        await db.SaveChangesAsync();
        return new MarkSowSignedResponse(engagement, warnings);
    }
}
