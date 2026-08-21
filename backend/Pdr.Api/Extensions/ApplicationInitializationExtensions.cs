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

            var existingTypes = db.EngagementTypes.Select(t => t.Name).ToList();
            var missingTypes = Enum.GetValues<EngagementTypeEnum>()
                .Select(t => t.ToString())
                .Where(t => !existingTypes.Contains(t))
                .Select(t => new EngagementType { Name = t })
                .ToList();

            if (missingTypes.Any())
            {
                db.EngagementTypes.AddRange(missingTypes);
                db.SaveChanges();
            }

            var existingStatuses = db.EngagementStatuses.Select(s => s.StatusName).ToList();
            var missingStatuses = Enum.GetValues<EngagementStatusEnum>()
                .Select(s => s.ToString())
                .Where(s => !existingStatuses.Contains(s))
                .Select(s => new EngagementStatus { StatusName = s })
                .ToList();

            if (missingStatuses.Any())
            {
                db.EngagementStatuses.AddRange(missingStatuses);
                db.SaveChanges();
            }
        }

        return host;
    }
}
