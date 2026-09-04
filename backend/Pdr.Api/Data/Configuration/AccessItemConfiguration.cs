using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pdr.Api.Models.AccessTracker;


namespace Pdr.Api.Data.Configuration
{
    public class AccessItemConfiguration : IEntityTypeConfiguration<AccessItem>
    {
        public void Configure(EntityTypeBuilder<AccessItem> builder)
        {
            builder.HasKey(x => x.Id);

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
        }
    }
}
