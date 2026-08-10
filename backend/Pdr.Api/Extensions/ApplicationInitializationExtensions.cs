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
        }

        return host;
    }
}
