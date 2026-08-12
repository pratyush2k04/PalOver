using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pdr.Api.Models.AccessTracker;


namespace Pdr.Api.Data.Configuration
{
    public class AccessItemConfiguration : IEntityTypeConfiguration<AccessItem>
    {
        public void Configure(EntityTypeBuilder<AccessItem> builder)
        {
            builder.HasKey(x => x.AccessItemId);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.TimeZoneNote)
                .HasMaxLength(100);

            builder.Property(x => x.BlockReason)
                .HasMaxLength(500);

            builder.HasQueryFilter(x => !x.IsDeleted);

            builder.HasOne(x => x.Engagement)
                .WithMany(x => x.AccessItems)
                .HasForeignKey(x => x.EngagementId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Owner)
                .WithMany(x => x.AccessItems)
                .HasForeignKey(x => x.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.AccessItemDefinition)
                .WithMany(x => x.AccessItems)
                .HasForeignKey(x => x.AccessItemDefinitionId)
                .OnDelete(DeleteBehavior.Restrict);

            var seededAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            builder.HasData(
                new AccessItem { AccessItemId = 1, EngagementId = 1, AccessItemDefinitionId = 1, Name = "VPN Access", IsClientSide = false, OwnerId = 1, Status = AccessItemStatus.InProgress, CreatedAt = seededAt, UpdatedAt = seededAt },
                new AccessItem { AccessItemId = 2, EngagementId = 1, AccessItemDefinitionId = 2, Name = "Jira", IsClientSide = true, OwnerId = 1, Status = AccessItemStatus.InProgress, CreatedAt = seededAt, UpdatedAt = seededAt },
                new AccessItem { AccessItemId = 3, EngagementId = 1, AccessItemDefinitionId = 3, Name = "SandBox", IsClientSide = true, OwnerId = 1, Status = AccessItemStatus.InProgress, CreatedAt = seededAt, UpdatedAt = seededAt },
                new AccessItem { AccessItemId = 4, EngagementId = 2, AccessItemDefinitionId = 1, Name = "VPN Access", IsClientSide = false, OwnerId = 1, Status = AccessItemStatus.Granted, GrantedDate = new DateOnly(2026, 6, 2), CreatedAt = seededAt, UpdatedAt = seededAt },
                new AccessItem { AccessItemId = 5, EngagementId = 2, AccessItemDefinitionId = 2, Name = "Jira", IsClientSide = true, OwnerId = 1, Status = AccessItemStatus.Granted, GrantedDate = new DateOnly(2026, 6, 2), CreatedAt = seededAt, UpdatedAt = seededAt },
                new AccessItem { AccessItemId = 6, EngagementId = 2, AccessItemDefinitionId = 3, Name = "SandBox", IsClientSide = true, OwnerId = 1, Status = AccessItemStatus.InProgress, CreatedAt = seededAt, UpdatedAt = seededAt }
            );
        }
    }
}
