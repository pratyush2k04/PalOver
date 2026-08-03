using Microsoft.EntityFrameworkCore;
using Pdr.Api.Data;
using Pdr.Api.Models;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Repositories;

public class EngagementRepository(AppDbContext context) : IEngagementRepository
{
    public async Task<List<Engagement>> GetEngagementsForPmAsync(int userId, string sortBy)
    {
        var query = context.Engagements
            .AsNoTracking()
            .Include(e => e.EngagementType)
            .Include(e => e.CommercialModel)
            .Where(e => e.EngagementAssignments.Any(a => a.UserId == userId));

        query = sortBy switch
        {
            "client" => query.OrderBy(e => e.ClientName),
            _ => query.OrderBy(e => e.KickoffDate)
        };

        return await query.ToListAsync();
    }
}
