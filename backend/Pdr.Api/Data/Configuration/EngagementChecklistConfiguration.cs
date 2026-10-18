using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pdr.Api.Models.EngagementChecklist;

namespace Pdr.Api.Data.Configuration;

public class EngagementChecklistConfiguration : IEntityTypeConfiguration<EngagementChecklist>
{
    public void Configure(EntityTypeBuilder<EngagementChecklist> builder)
    {
        builder.ToTable("engagement_checklists");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id").UseIdentityAlwaysColumn();

        builder.Property(e => e.EngagementId).HasColumnName("engagement_id").IsRequired();
        builder.Property(e => e.SourceMasterVersionId).HasColumnName("source_master_version_id").IsRequired();
        builder.Property(e => e.VersionNumber).HasColumnName("version_number").IsRequired();

        // BaseAuditableEntity Column Mappings
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
        builder.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("NOW()");
        builder.Property(e => e.CreatedBy).HasColumnName("created_by").HasMaxLength(255);
        builder.Property(e => e.UpdatedBy).HasColumnName("updated_by").HasMaxLength(255);
        builder.Property(e => e.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);

        // Global Query Filter for Soft Delete
        builder.HasQueryFilter(e => !e.IsDeleted);

        // Unique index — one checklist per engagement
        builder.HasIndex(e => e.EngagementId).IsUnique();

        builder.HasMany(e => e.Items)
               .WithOne(i => i.EngagementChecklist)
               .HasForeignKey(i => i.EngagementChecklistId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
