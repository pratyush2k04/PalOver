using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pdr.Api.Models.AccessTracker;


namespace Pdr.Api.Data.Configuration.AccessTracker
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

            builder.HasOne(x => x.Engagement)
                .WithMany(x => x.AccessItems)
                .HasForeignKey(x => x.EngagementId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Owner)
                .WithMany(x => x.AccessItems)
                .HasForeignKey(x => x.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.// backend\Pdr.Api\Models\AccessTracker\AccessItem.cs
public class AccessItem : BaseAuditableEntity
{
  public int AccessItemId { get; set; }
  public string Name { get; set; }
  public int EngagementId { get; set; }
  public Engagement Engagement { get; set; }
  public int OwnerId { get; set; }
  public User Owner { get; set; }

  // Added to match AccessItemConfiguration
  public int? AccessItemDefinitionId { get; set; }
  public AccessItemDefinition? AccessItemDefinition { get; set; }

  public AccessItemStatus Status { get; set; }
  public bool IsClientSide { get; set; }
  public string? TimeZoneNote { get; set; }
  public DateOnly? GrantedDate { get; set; }
  public string? BlockReason { get; set; }
}AccessItemDefinition)
                .WithMany(x => x.AccessItems)
                .HasForeignKey(x => x.AccessItemDefinitionId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
