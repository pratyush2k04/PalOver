using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pdr.Api.Data;
using Pdr.Api.Models;
using Pdr.Api.Repositories.Interfaces;

namespace Pdr.Api.Repositories;

public class ItemEngagementRepository(AppDbContext db) : IItemEngagementRepository
{
    public async Task<int?> GetEngagementIdAsync(ItemEntityType entityType, int itemId)
    {
        return entityType switch
        {
            ItemEntityType.AccessItem => await db.AccessItems
                .AsNoTracking()
                .Where(x => x.Id == itemId && !x.IsDeleted)
                .Select(x => (int?)x.EngagementId)
                .FirstOrDefaultAsync(),

            ItemEntityType.EnvironmentTooling => await db.EnvironmentToolingItems
                .AsNoTracking()
                .Where(x => x.Id == itemId && !x.IsDeleted)
                .Select(x => (int?)x.EngagementId)
                .FirstOrDefaultAsync(),

            ItemEntityType.ChangeRequest => await db.ChangeRequests
                .AsNoTracking()
                .Where(x => x.Id == itemId && !x.IsDeleted)
                .Select(x => (int?)x.EngagementId)
                .FirstOrDefaultAsync(),

            ItemEntityType.BrownfieldFinding => await db.BrownfieldFindings
                .AsNoTracking()
                .Where(x => x.Id == itemId && !x.IsDeleted)
                .Select(x => (int?)x.EngagementId)
                .FirstOrDefaultAsync(),

            ItemEntityType.DiscoveryFeature => await db.DiscoveryFeatureItems
                .AsNoTracking()
                .Where(x => x.Id == itemId && !x.IsDeleted && !x.ArchitectureValidation.IsDeleted)
                .Select(x => (int?)x.ArchitectureValidation.EngagementId)
                .FirstOrDefaultAsync(),

            ItemEntityType.Document => await db.Documents
                .AsNoTracking()
                .Where(x => x.Id == itemId && !x.IsDeleted)
                .Select(x => (int?)x.EngagementId)
                .FirstOrDefaultAsync(),

            ItemEntityType.HandoverReadinessItem => await db.HandoverReadinessItems
                .AsNoTracking()
                .Where(x => x.Id == itemId && !x.IsDeleted)
                .Select(x => (int?)x.EngagementId)
                .FirstOrDefaultAsync(),

            _ => null
        };
    }
}
