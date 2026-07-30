using Microsoft.EntityFrameworkCore;
using Pdr.Api.Data;
using Pdr.Api.DTOs.Workspace;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Services;

public class WorkspaceService(AppDbContext db) : IWorkspaceService
{
    public async Task<IEnumerable<EngagementTypeDto>> GetEngagementTypesAsync() =>
        await db.EngagementTypes
            .Select(t => new EngagementTypeDto(t.Id, t.Name))
            .ToListAsync();

    public async Task<EngagementInfoDto?> GetEngagementInfoAsync(long id) =>
        await db.Engagements
            .Where(e => e.EngagementId == id)
            .Select(e => new EngagementInfoDto(
                e.EngagementId,
                e.ClientName,
                e.EngagementDescription,
                e.EngagementType.Name,
                e.KickoffDate,
                e.BillingStartDate,
                e.Status.StatusName))
            .FirstOrDefaultAsync();

    public async Task<bool> EditEngagementAsync(long id, EditEngagementRequest request)
    {
        var engagement = await db.Engagements.FindAsync(id);
        if (engagement is null) return false;

        if (request.ClientName is not null) engagement.ClientName = request.ClientName;
        if (request.EngagementDescription is not null) engagement.EngagementDescription = request.EngagementDescription;
        if (request.EngagementTypeId is not null) engagement.EngagementTypeId = request.EngagementTypeId.Value;
        if (request.KickoffDate is not null) engagement.KickoffDate = request.KickoffDate.Value;
        if (request.BillingStartDate is not null) engagement.BillingStartDate = request.BillingStartDate;

        // Status is hardcoded to "Active" for now; revisit once status selection is designed (future sprint).
        var activeStatus = await db.EngagementStatuses.FirstAsync(s => s.StatusName == "Active");
        engagement.StatusId = activeStatus.StatusId;

        engagement.UpdatedOn = DateTime.UtcNow;

        await db.SaveChangesAsync();
        return true;
    }
}
