using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pdr.Api.Models.Engagements.Dependencies;

namespace Pdr.Api.Data.Configuration;

public class DependencyConfiguration : IEntityTypeConfiguration<Dependency>
{
    public void Configure(EntityTypeBuilder<Dependency> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(2000);

        builder.HasOne(x => x.Engagement)
            .WithMany()
            .HasForeignKey(x => x.EngagementId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Owner)
            .WithMany()
            .HasForeignKey(x => x.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.LinkedRisk)
            .WithMany()
            .HasForeignKey(x => x.LinkedRiskId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
