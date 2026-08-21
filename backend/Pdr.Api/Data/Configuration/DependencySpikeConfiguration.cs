using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pdr.Api.Models.DependencySpikes;

namespace Pdr.Api.Data.Configuration;

public class DependencySpikeConfiguration : IEntityTypeConfiguration<DependencySpike>
{
    public void Configure(EntityTypeBuilder<DependencySpike> builder)
    {
        builder.ToTable("dependency_spikes");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.DependencyName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Objective)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(x => x.Status)
            .IsRequired();

        builder.Property(x => x.OutcomeFindings)
            .HasMaxLength(2000);

        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.HasOne(x => x.Engagement)
            .WithMany()
            .HasForeignKey(x => x.EngagementId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Owner)
            .WithMany()
            .HasForeignKey(x => x.OwnerId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(x => x.EngagementId);
    }
}
