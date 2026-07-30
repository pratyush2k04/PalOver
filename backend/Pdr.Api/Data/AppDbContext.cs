using Microsoft.EntityFrameworkCore;
using Pdr.Api.Models;

namespace Pdr.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Engagement> Engagements => Set<Engagement>();
    public DbSet<EngagementStatus> EngagementStatuses => Set<EngagementStatus>();
    public DbSet<EngagementType> EngagementTypes => Set<EngagementType>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<EngagementStatus>(entity =>
        {
            entity.HasKey(s => s.StatusId);

            entity.Property(s => s.StatusName)
                .HasMaxLength(50)
                .IsRequired();

        });

        modelBuilder.Entity<EngagementType>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Name).HasMaxLength(100).IsRequired();
        });

        modelBuilder.Entity<Engagement>(entity =>
        {
            entity.HasKey(e => e.EngagementId);
            entity.Property(e => e.ClientName).HasMaxLength(200).IsRequired();

            entity.HasOne(e => e.EngagementType)
                  .WithMany()
                  .HasForeignKey(e => e.EngagementTypeId);

            entity.HasOne(e => e.Status)
                  .WithMany(s => s.Engagements)
                  .HasForeignKey(e => e.StatusId);
        });

    }
}
