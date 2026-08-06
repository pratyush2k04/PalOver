using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pdr.Api.Models.EnvironmentTooling;
using Microsoft.EntityFrameworkCore;

namespace Pdr.Api.Data.Configuration
{
    public class EnvironmentToolingItemConfiguration : IEntityTypeConfiguration<EnvironmentToolingItem>
    {
        public void Configure(EntityTypeBuilder<EnvironmentToolingItem> builder)
        {
            builder.HasKey(x => x.EnvironmentToolingItemId);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.NotApplicableReason)
                .HasMaxLength(500);

            builder.Property(x => x.BlockReason)
                .HasMaxLength(500);

            builder.HasOne(x => x.Engagement)
                .WithMany(x => x.EnvironmentToolingItems)
                .HasForeignKey(x => x.EngagementId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Owner)
                .WithMany(x => x.EnvironmentToolingItems)
                .HasForeignKey(x => x.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.EnvironmentToolingDefinition)
                .WithMany(x => x.EnvironmentToolingItems)
                .HasForeignKey(x => x.EnvironmentToolingDefinitionId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
