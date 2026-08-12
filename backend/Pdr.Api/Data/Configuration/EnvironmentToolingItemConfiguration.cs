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

            builder.HasQueryFilter(x => !x.IsDeleted);

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

            var seededAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            builder.HasData(
                new EnvironmentToolingItem { EnvironmentToolingItemId = 1, EngagementId = 1, EnvironmentToolingDefinitionId = 1, Name = "CI/CD", OwnerId = 1, Status = EnvironmentToolingStatus.NotReady, CreatedAt = seededAt, UpdatedAt = seededAt },
                new EnvironmentToolingItem { EnvironmentToolingItemId = 2, EngagementId = 1, EnvironmentToolingDefinitionId = 2, Name = "Dev", OwnerId = 1, Status = EnvironmentToolingStatus.NotReady, CreatedAt = seededAt, UpdatedAt = seededAt },
                new EnvironmentToolingItem { EnvironmentToolingItemId = 3, EngagementId = 1, EnvironmentToolingDefinitionId = 3, Name = "UAT Environment", OwnerId = 1, Status = EnvironmentToolingStatus.NotReady, CreatedAt = seededAt, UpdatedAt = seededAt },
                new EnvironmentToolingItem { EnvironmentToolingItemId = 4, EngagementId = 2, EnvironmentToolingDefinitionId = 1, Name = "CI/CD", OwnerId = 1, Status = EnvironmentToolingStatus.Ready, CreatedAt = seededAt, UpdatedAt = seededAt },
                new EnvironmentToolingItem { EnvironmentToolingItemId = 5, EngagementId = 2, EnvironmentToolingDefinitionId = 2, Name = "Dev", OwnerId = 1, Status = EnvironmentToolingStatus.Ready, CreatedAt = seededAt, UpdatedAt = seededAt },
                new EnvironmentToolingItem { EnvironmentToolingItemId = 6, EngagementId = 2, EnvironmentToolingDefinitionId = 3, Name = "UAT Environment", OwnerId = 1, Status = EnvironmentToolingStatus.NotReady, CreatedAt = seededAt, UpdatedAt = seededAt }
            );
        }
    }
}
