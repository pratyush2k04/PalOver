using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pdr.Api.Models.Handover;

namespace Pdr.Api.Data.Configuration;

public class CommitmentConfiguration : IEntityTypeConfiguration<Commitment>
{
    public void Configure(EntityTypeBuilder<Commitment> builder)
    {
        builder.ToTable("commitments");

        builder.HasKey(c => c.CommitmentId);
        builder.Property(c => c.CommitmentId).HasColumnName("commitment_id").UseIdentityAlwaysColumn();
        
        builder.Property(c => c.HandoverId).HasColumnName("handover_id").IsRequired();
        builder.Property(c => c.Description).HasColumnName("description").HasMaxLength(2000).IsRequired();
        builder.Property(c => c.CommitmentTypeId).HasColumnName("commitment_type_id").IsRequired();
        builder.Property(c => c.Title).HasColumnName("title").HasMaxLength(100).IsRequired();
        builder.Property(c => c.Timeline).HasColumnName("timeline").IsRequired();
        builder.Property(c => c.TimelineUnit).HasColumnName("timeline_unit").HasMaxLength(20).IsRequired();

        // BaseAuditableEntity Column Mappings
        builder.Property(c => c.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
        builder.Property(c => c.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("NOW()");
        builder.Property(c => c.CreatedBy).HasColumnName("created_by").HasMaxLength(255);
        builder.Property(c => c.UpdatedBy).HasColumnName("updated_by").HasMaxLength(255);
        builder.Property(c => c.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);

        builder.HasQueryFilter(x => !x.IsDeleted);

        var seedTimestamp = new System.DateTime(2026, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
        builder.HasData(
            new Commitment
            {
                CommitmentId = 1,
                HandoverId = 1,
                CommitmentTypeId = 1, // Delivery Milestone
                Description = "Delivery of MVP banking module within 3 months of kickoff.",
                Title = "MVP Banking Module Delivery",
                Timeline = 3,
                TimelineUnit = "Months",
                CreatedAt = seedTimestamp,
                UpdatedAt = seedTimestamp,
                CreatedBy = "System",
                IsDeleted = false
            }
        );
    }
}
