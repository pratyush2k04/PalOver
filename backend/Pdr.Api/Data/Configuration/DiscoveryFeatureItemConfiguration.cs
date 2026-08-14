using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pdr.Api.Models.ArchitectureValidation;

namespace Pdr.Api.Data.Configuration;

public class DiscoveryFeatureItemConfiguration : IEntityTypeConfiguration<DiscoveryFeatureItem>
{
    public void Configure(EntityTypeBuilder<DiscoveryFeatureItem> builder)
    {
        builder.ToTable("discovery_feature_items");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.FeatureName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.EstimateWeeks)
            .HasPrecision(5, 2);

        builder.Property(x => x.TargetSprint)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
