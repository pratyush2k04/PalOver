using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pdr.Api.Models.ResourceRequests;

namespace Pdr.Api.Data.Configuration;

public class ResourceAllocationConfiguration : IEntityTypeConfiguration<ResourceAllocation>
{
    public void Configure(EntityTypeBuilder<ResourceAllocation> builder)
    {
        builder.ToTable("resource_allocations");

        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).HasColumnName("id");

        builder.Property(a => a.ResourceRequestId).HasColumnName("resource_request_id").IsRequired();
        builder.Property(a => a.Name).HasColumnName("name").HasMaxLength(255).IsRequired();
        builder.Property(a => a.ConfirmedDate).HasColumnName("confirmed_date");
        builder.Property(a => a.RemovedAt).HasColumnName("removed_at");
        builder.Property(a => a.RemovalReason).HasColumnName("removal_reason").HasMaxLength(500);

        // BaseAuditableEntity Column Mappings
        builder.Property(a => a.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
        builder.Property(a => a.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("NOW()");
        builder.Property(a => a.CreatedBy).HasColumnName("created_by").HasMaxLength(255);
        builder.Property(a => a.UpdatedBy).HasColumnName("updated_by").HasMaxLength(255);
        builder.Property(a => a.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);

        builder.HasOne(a => a.ResourceRequest)
            .WithMany(r => r.Allocations)
            .HasForeignKey(a => a.ResourceRequestId)
            .OnDelete(DeleteBehavior.Cascade);

        // Mirrors ResourceRequestSkillConfiguration's cascading soft-delete filter. Deliberately NOT
        // filtering on RemovedAt here — "active vs. removed" is business state handled explicitly in
        // the service layer, kept queryable for the deferred audit-trail ticket.
        builder.HasQueryFilter(a => !a.ResourceRequest.IsDeleted);
    }
}
