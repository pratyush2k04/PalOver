using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pdr.Api.Models;
using Pdr.Api.Models.Engagements;
using Pdr.Api.Models.EngagementChecklist;

namespace Pdr.Api.Data.Configuration;

public class EngagementConfiguration : IEntityTypeConfiguration<Engagement>
{
    private static readonly DateTime SeedTimestamp = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public void Configure(EntityTypeBuilder<Engagement> builder)
    {
        builder.ToTable("engagements");

        builder.HasKey(e => e.EngagementId);
        builder.Property(e => e.EngagementId).HasColumnName("engagement_id");

        builder.Property(e => e.EngagementName).HasColumnName("engagement_name").HasMaxLength(200).IsRequired();
        builder.Property(e => e.ClientName).HasColumnName("client_name").HasMaxLength(200).IsRequired();
        builder.Property(e => e.EngagementDescription).HasColumnName("engagement_description");
        builder.Property(e => e.EngagementTypeId).HasColumnName("engagement_type_id").IsRequired();
        builder.Property(e => e.CommercialModelId).HasColumnName("commercial_model_id").IsRequired();
        builder.Property(e => e.KickoffDate).HasColumnName("kickoff_date").IsRequired();
        builder.Property(e => e.BillingStartDate).HasColumnName("billing_start_date");
        builder.Property(e => e.StatusId).HasColumnName("status_id");
        builder.Property(e => e.IsKickoffReady).HasColumnName("is_kickoff_ready").HasDefaultValue(false);

        // BaseAuditableEntity Column Mappings
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
        builder.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("NOW()");
        builder.Property(e => e.CreatedBy).HasColumnName("created_by").HasMaxLength(255);
        builder.Property(e => e.UpdatedBy).HasColumnName("updated_by").HasMaxLength(255);
        builder.Property(e => e.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);

        // Global Query Filter for Soft Delete
        builder.HasQueryFilter(e => !e.IsDeleted);

        builder.HasOne(e => e.EngagementType)
               .WithMany()
               .HasForeignKey(e => e.EngagementTypeId);

        builder.HasOne(e => e.CommercialModel)
               .WithMany()
               .HasForeignKey(e => e.CommercialModelId);

        builder.HasOne(e => e.Status)
               .WithMany(s => s.Engagements)
               .HasForeignKey(e => e.StatusId);

        // 1-to-1 relationship with EngagementChecklist (FK is on the EngagementChecklist side)
        builder.HasOne(e => e.Checklist)
               .WithOne(c => c.Engagement)
               .HasForeignKey<EngagementChecklist>(c => c.EngagementId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}

