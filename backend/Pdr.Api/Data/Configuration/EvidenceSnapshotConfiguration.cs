using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pdr.Api.Models.Engagements;

namespace Pdr.Api.Data.Configuration;

public class EvidenceSnapshotConfiguration : IEntityTypeConfiguration<EvidenceSnapshot>
{
    public void Configure(EntityTypeBuilder<EvidenceSnapshot> builder)
    {
        builder.ToTable("evidence_snapshots");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).HasColumnName("id").UseIdentityAlwaysColumn();

        builder.Property(s => s.EngagementId).HasColumnName("engagement_id").IsRequired();
        builder.Property(s => s.SignedOffByUserId).HasColumnName("signed_off_by_user_id").IsRequired();
        builder.Property(s => s.SignedOffByUserName).HasColumnName("signed_off_by_user_name").HasMaxLength(200).IsRequired();
        builder.Property(s => s.SignedOffAt).HasColumnName("signed_off_at").IsRequired();
        builder.Property(s => s.BillingStartDate).HasColumnName("billing_start_date").IsRequired();

        // BaseAuditableEntity Column Mappings
        builder.Property(s => s.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
        builder.Property(s => s.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("NOW()");
        builder.Property(s => s.CreatedBy).HasColumnName("created_by").HasMaxLength(255);
        builder.Property(s => s.UpdatedBy).HasColumnName("updated_by").HasMaxLength(255);
        builder.Property(s => s.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);

        // Global Query Filter for Soft Delete
        builder.HasQueryFilter(s => !s.IsDeleted);

        // One evidence snapshot per engagement
        builder.HasIndex(s => s.EngagementId).IsUnique();

        // Foreign Key Relationships
        builder.HasOne(s => s.Engagement)
            .WithOne(e => e.EvidenceSnapshot)
            .HasForeignKey<EvidenceSnapshot>(s => s.EngagementId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(s => s.SignedOffByUser)
            .WithMany()
            .HasForeignKey(s => s.SignedOffByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
