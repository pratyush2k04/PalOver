using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pdr.Api.Models;
using Pdr.Api.Models.Engagements;
using Pdr.Api.Models.LoginEntity;
using Pdr.Api.Models.ResourceRequests;

namespace Pdr.Api.Data.Configuration;

public class ResourceRequestConfiguration : IEntityTypeConfiguration<ResourceRequest>
{
    public void Configure(EntityTypeBuilder<ResourceRequest> builder)
    {
        // EF Core's Fluent API has no minimum-value/minimum-length builder for either numeric or
        // string properties (HasMaxLength exists, but there's no HasMinLength) — both constraints
        // are enforced with DB check constraints instead.
        builder.ToTable("resource_requests", t =>
        {
            t.HasCheckConstraint("ck_resource_requests_people_needed_positive", "people_needed > 0");
            t.HasCheckConstraint("ck_resource_requests_job_description_min_length", "char_length(job_description) >= 15");
        });

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).HasColumnName("id");

        builder.Property(r => r.EngagementId).HasColumnName("engagement_id").IsRequired();
        builder.Property(r => r.StandardRole).HasColumnName("standard_role").IsRequired();
        builder.Property(r => r.PeopleNeeded).HasColumnName("people_needed").IsRequired();
        builder.Property(r => r.RequiredByDate).HasColumnName("required_by_date").IsRequired();
        builder.Property(r => r.JobDescription).HasColumnName("job_description").HasMaxLength(500).IsRequired();
        builder.Property(r => r.Status).HasColumnName("status").IsRequired();
        builder.Property(r => r.RequestedById).HasColumnName("requested_by_id").IsRequired();

        // BaseAuditableEntity Column Mappings
        builder.Property(r => r.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
        builder.Property(r => r.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("NOW()");
        builder.Property(r => r.CreatedBy).HasColumnName("created_by").HasMaxLength(255);
        builder.Property(r => r.UpdatedBy).HasColumnName("updated_by").HasMaxLength(255);
        builder.Property(r => r.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);

        // Matches the Resources tab and Portfolio "resourcing gap" query pattern (per engagement, by status)
        builder.HasIndex(r => new { r.EngagementId, r.Status });

        // Global Query Filter for Soft Delete
        builder.HasQueryFilter(r => !r.IsDeleted);

        // Foreign Keys without navigation properties
        builder.HasOne<Engagement>()
            .WithMany()
            .HasForeignKey(r => r.EngagementId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(r => r.RequestedById)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
