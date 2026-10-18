using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pdr.Api.Models.Engagements;

namespace Pdr.Api.Data.Configuration;

public class DayOneSignOffRecordConfiguration : IEntityTypeConfiguration<DayOneSignOffRecord>
{
    public void Configure(EntityTypeBuilder<DayOneSignOffRecord> builder)
    {
        builder.ToTable("day_one_sign_off_records");

        builder.HasKey(d => d.Id);
        builder.Property(d => d.Id).HasColumnName("id").UseIdentityAlwaysColumn();

        builder.Property(d => d.EngagementId).HasColumnName("engagement_id").IsRequired();
        builder.Property(d => d.SignedOffByUserId).HasColumnName("signed_off_by_user_id").IsRequired();
        builder.Property(d => d.BillingStartDate).HasColumnName("billing_start_date").IsRequired();
        builder.Property(d => d.SignedOffAt).HasColumnName("signed_off_at").IsRequired();
        builder.Property(d => d.IsException).HasColumnName("is_exception").HasDefaultValue(false);
        builder.Property(d => d.ExceptionJustification).HasColumnName("exception_justification").HasMaxLength(500);

        // BaseAuditableEntity Column Mappings
        builder.Property(d => d.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
        builder.Property(d => d.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("NOW()");
        builder.Property(d => d.CreatedBy).HasColumnName("created_by").HasMaxLength(255);
        builder.Property(d => d.UpdatedBy).HasColumnName("updated_by").HasMaxLength(255);
        builder.Property(d => d.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);

        // Global Query Filter for Soft Delete
        builder.HasQueryFilter(d => !d.IsDeleted);

        // Foreign Key Relationships
        builder.HasOne(d => d.Engagement)
            .WithOne(e => e.DayOneSignOffRecord)
            .HasForeignKey<DayOneSignOffRecord>(d => d.EngagementId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(d => d.SignedOffByUser)
            .WithMany()
            .HasForeignKey(d => d.SignedOffByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
