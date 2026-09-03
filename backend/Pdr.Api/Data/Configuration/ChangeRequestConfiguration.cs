using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pdr.Api.Models;
using Pdr.Api.Models.Engagements;
using Pdr.Api.Models.ChangeRequest;
using Pdr.Api.Models.LoginEntity;

namespace Pdr.Api.Data.Configuration;

public class ChangeRequestConfiguration : IEntityTypeConfiguration<Models.ChangeRequest.ChangeRequest>
{
    public void Configure(EntityTypeBuilder<Models.ChangeRequest.ChangeRequest> builder)
    {
        builder.ToTable("change_requests");

        builder.HasKey(cr => cr.Id);
        builder.Property(cr => cr.Id).HasColumnName("id");

        builder.Property(cr => cr.EngagementId).HasColumnName("engagement_id").IsRequired();
        builder.Property(cr => cr.Description).HasColumnName("description").HasMaxLength(500).IsRequired();
        builder.Property(cr => cr.Impact).HasColumnName("impact").HasMaxLength(500).IsRequired();
        builder.Property(cr => cr.Status).HasColumnName("status").IsRequired();
        builder.Property(cr => cr.RejectionReason).HasColumnName("rejection_reason").HasMaxLength(500);

        builder.Property(cr => cr.ReviewedById).HasColumnName("reviewed_by_id");
        builder.Property(cr => cr.ReviewedAt).HasColumnName("reviewed_at");

        // BaseAuditableEntity Column Mappings
        builder.Property(cr => cr.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);
        builder.Property(cr => cr.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
        builder.Property(cr => cr.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("NOW()");
        builder.Property(cr => cr.CreatedBy).HasColumnName("created_by").HasMaxLength(100);
        builder.Property(cr => cr.UpdatedBy).HasColumnName("updated_by").HasMaxLength(100);

        // Foreign Keys without navigation properties
        builder.HasOne<Engagement>()
            .WithMany()
            .HasForeignKey(cr => cr.EngagementId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(cr => cr.ReviewedById)
            .OnDelete(DeleteBehavior.Restrict);

        // Global Query Filter (Soft Delete)
        builder.HasQueryFilter(cr => !cr.IsDeleted);

        // Seed Sample Data
        var seededAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        builder.HasData(
            new Models.ChangeRequest.ChangeRequest
            {
                Id = 1,
                EngagementId = 1,
                Description = "Add SMS notifications to the rewards flow",
                Impact = "+1.5 weeks, +1 developer-week",
                Status = ChangeRequestStatus.UnderReview,
                CreatedAt = seededAt,
                UpdatedAt = seededAt,
                IsDeleted = false,
                CreatedBy = "2", // Asha Rao (PM user ID 2)
                UpdatedBy = "2"
            }
        );
    }
}
