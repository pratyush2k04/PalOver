using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pdr.Api.Models.Engagements;

namespace Pdr.Api.Data.Configuration;

public class EvidenceSnapshotItemConfiguration : IEntityTypeConfiguration<EvidenceSnapshotItem>
{
    public void Configure(EntityTypeBuilder<EvidenceSnapshotItem> builder)
    {
        builder.ToTable("evidence_snapshot_items");

        builder.HasKey(i => i.Id);
        builder.Property(i => i.Id).HasColumnName("id").UseIdentityAlwaysColumn();

        builder.Property(i => i.EvidenceSnapshotId).HasColumnName("evidence_snapshot_id").IsRequired();
        builder.Property(i => i.SourceChecklistItemId).HasColumnName("source_checklist_item_id");
        builder.Property(i => i.Title).HasColumnName("title").HasMaxLength(255).IsRequired();
        builder.Property(i => i.CategoryName).HasColumnName("category_name").HasMaxLength(200).IsRequired();
        builder.Property(i => i.DisplayOrder).HasColumnName("display_order").IsRequired();

        builder.Property(i => i.StatusAtSignOff)
            .HasColumnName("status_at_sign_off")
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(i => i.EvidenceDocumentId).HasColumnName("evidence_document_id");
        builder.Property(i => i.EvidenceDocumentFileName).HasColumnName("evidence_document_file_name").HasMaxLength(255);
        builder.Property(i => i.EvidenceSubmittedAt).HasColumnName("evidence_submitted_at");
        builder.Property(i => i.EvidenceSubmittedByName).HasColumnName("evidence_submitted_by_name").HasMaxLength(200);
        builder.Property(i => i.OwnerId).HasColumnName("owner_id");
        builder.Property(i => i.OwnerName).HasColumnName("owner_name").HasMaxLength(200);

        // BaseAuditableEntity Column Mappings
        builder.Property(i => i.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
        builder.Property(i => i.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("NOW()");
        builder.Property(i => i.CreatedBy).HasColumnName("created_by").HasMaxLength(255);
        builder.Property(i => i.UpdatedBy).HasColumnName("updated_by").HasMaxLength(255);
        builder.Property(i => i.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);

        // Global Query Filter for Soft Delete
        builder.HasQueryFilter(i => !i.IsDeleted);

        // Foreign Key Relationships
        builder.HasOne(i => i.EvidenceSnapshot)
            .WithMany(s => s.Items)
            .HasForeignKey(i => i.EvidenceSnapshotId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
