using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pdr.Api.Models.Kickoff;

namespace Pdr.Api.Data.Configuration;

public class KickoffActionItemConfiguration : IEntityTypeConfiguration<KickoffActionItem>
{
    public void Configure(EntityTypeBuilder<KickoffActionItem> builder)
    {
        builder.ToTable("kickoff_action_items");

        builder.HasKey(a => a.KickoffActionItemId);
        builder.Property(a => a.KickoffActionItemId).HasColumnName("kickoff_action_item_id").UseIdentityAlwaysColumn();

        builder.Property(a => a.KickoffRecordId).HasColumnName("kickoff_record_id").IsRequired();
        builder.Property(a => a.EngagementId).HasColumnName("engagement_id").IsRequired();
        builder.Property(a => a.Title).HasColumnName("title").HasMaxLength(255).IsRequired();
        builder.Property(a => a.OwnerName).HasColumnName("owner_name").HasMaxLength(200).IsRequired();
        builder.Property(a => a.AssignedToUserId).HasColumnName("assigned_to_user_id");
        builder.Property(a => a.DueDate).HasColumnName("due_date").IsRequired();

        builder.Property(a => a.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasMaxLength(50)
            .HasDefaultValue(KickoffActionItemStatus.Open)
            .HasSentinel(default(KickoffActionItemStatus))
            .IsRequired();

        builder.Property(a => a.IsNotified).HasColumnName("is_notified").HasDefaultValue(true);

        // BaseAuditableEntity Column Mappings
        builder.Property(a => a.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
        builder.Property(a => a.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("NOW()");
        builder.Property(a => a.CreatedBy).HasColumnName("created_by").HasMaxLength(255);
        builder.Property(a => a.UpdatedBy).HasColumnName("updated_by").HasMaxLength(255);
        builder.Property(a => a.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);

        // Global Query Filter for Soft Delete
        builder.HasQueryFilter(a => !a.IsDeleted);

        // Foreign Key Relationships
        builder.HasOne(a => a.KickoffRecord)
            .WithMany(k => k.ActionItems)
            .HasForeignKey(a => a.KickoffRecordId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(a => a.Engagement)
            .WithMany(e => e.KickoffActionItems)
            .HasForeignKey(a => a.EngagementId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(a => a.AssignedToUser)
            .WithMany()
            .HasForeignKey(a => a.AssignedToUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
