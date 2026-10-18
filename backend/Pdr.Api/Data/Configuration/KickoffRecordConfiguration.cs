using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pdr.Api.Models.Kickoff;

namespace Pdr.Api.Data.Configuration;

public class KickoffRecordConfiguration : IEntityTypeConfiguration<KickoffRecord>
{
    public void Configure(EntityTypeBuilder<KickoffRecord> builder)
    {
        builder.ToTable("kickoff_records");

        builder.HasKey(k => k.KickoffRecordId);
        builder.Property(k => k.KickoffRecordId).HasColumnName("kickoff_record_id").UseIdentityAlwaysColumn();

        builder.Property(k => k.EngagementId).HasColumnName("engagement_id").IsRequired();
        builder.Property(k => k.Outcomes).HasColumnName("outcomes").IsRequired();

        // BaseAuditableEntity Column Mappings
        builder.Property(k => k.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
        builder.Property(k => k.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("NOW()");
        builder.Property(k => k.CreatedBy).HasColumnName("created_by").HasMaxLength(255);
        builder.Property(k => k.UpdatedBy).HasColumnName("updated_by").HasMaxLength(255);
        builder.Property(k => k.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);

        // Global Query Filter for Soft Delete
        builder.HasQueryFilter(k => !k.IsDeleted);

        // Foreign Key Relationship (1-to-1 with Engagement)
        builder.HasOne(k => k.Engagement)
            .WithOne(e => e.KickoffRecord)
            .HasForeignKey<KickoffRecord>(k => k.EngagementId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
