using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pdr.Api.Data;
using Pdr.Api.Models;
using Pdr.Api.Models.Engagements;

namespace Pdr.Api.Extensions;

public static class ApplicationInitializationExtensions
{
    public static IHost InitializeDatabase(this IHost host)
    {
        using (var scope = host.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.Migrate();

            if (!db.EngagementTypes.Any())
            {
                db.EngagementTypes.AddRange(
                    Enum.GetValues<EngagementTypeEnum>()
                        .Select(t => new EngagementType { Name = t.ToString() }));
                db.SaveChanges();
            }

            if (!db.EngagementStatuses.Any())
            {
                db.EngagementStatuses.AddRange(
                    Enum.GetValues<EngagementStatusEnum>()
                        .Select(s => new EngagementStatus { StatusName = s.ToString() }));
                db.SaveChanges();
            }

            var engagements = db.Engagements
                .Include(e => e.AccessItems)
                .Include(e => e.EnvironmentToolingItems)
                .ToList();

            var accessDefs = db.AccessItemDefinitions
                .Where(d => d.IsActive)
                .OrderBy(d => d.DisplayOrder)
                .ToList();

            var toolingDefs = db.EnvironmentToolingDefinitions
                .Where(d => d.IsActive)
                .OrderBy(d => d.DisplayOrder)
                .ToList();

            bool changesMade = false;
            foreach (var engagement in engagements)
            {
                // Only seed AccessItems if an engagement has no access items at all
                if (!engagement.AccessItems.Any() && accessDefs.Any())
                {
                    foreach (var def in accessDefs)
                    {
                        db.AccessItems.Add(new Models.AccessTracker.AccessItem
                        {
                            EngagementId = engagement.EngagementId,
                            AccessItemDefinitionId = def.AccessItemDefinitionId,
                            Name = def.Name,
                            IsClientSide = def.IsClientSide,
                            OwnerId = 1,
                            Status = Models.AccessTracker.AccessItemStatus.InProgress
                        });
                    }
                    changesMade = true;
                }

                // Only seed EnvironmentToolingItems if an engagement has no environment tooling items at all
                if (!engagement.EnvironmentToolingItems.Any() && toolingDefs.Any())
                {
                    foreach (var def in toolingDefs)
                    {
                        db.EnvironmentToolingItems.Add(new Models.EnvironmentTooling.EnvironmentToolingItem
                        {
                            EngagementId = engagement.EngagementId,
                            EnvironmentToolingDefinitionId = def.EnvironmentToolingDefinitionId,
                            Name = def.Name,
                            OwnerId = 1,
                            Status = Models.EnvironmentTooling.EnvironmentToolingStatus.NotReady
                        });
                    }
                    changesMade = true;
                }
            }

            if (changesMade)
            {
                db.SaveChanges();
            }
        }

        return host;
    }
}
