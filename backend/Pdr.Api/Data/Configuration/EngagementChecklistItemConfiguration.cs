using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pdr.Api.Models.EngagementChecklist;

namespace Pdr.Api.Data.Configuration;

public class EngagementChecklistItemConfiguration : IEntityTypeConfiguration<EngagementChecklistItem>
{
    public void Configure(EntityTypeBuilder<EngagementChecklistItem> builder)
    {
        builder.ToTable("engagement_checklist_items");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id").UseIdentityAlwaysColumn();

        builder.Property(e => e.EngagementChecklistId).HasColumnName("engagement_checklist_id").IsRequired();
        builder.Property(e => e.SourceMasterItemId).HasColumnName("source_master_item_id");
        builder.Property(e => e.Title).HasColumnName("title").HasMaxLength(255).IsRequired();
        builder.Property(e => e.Description).HasColumnName("description").HasMaxLength(500);
        builder.Property(e => e.CategoryId).HasColumnName("category_id").IsRequired();
        builder.Property(e => e.IsMandatory).HasColumnName("is_mandatory").HasDefaultValue(true).IsRequired();
        builder.Property(e => e.DisplayOrder).HasColumnName("display_order").IsRequired();
        builder.Property(e => e.IsApplicable).HasColumnName("is_applicable").HasDefaultValue(true).IsRequired();
        builder.Property(e => e.Status).HasColumnName("status").HasMaxLength(50).IsRequired();

        // Applicability exemption columns
        builder.Property(e => e.NotApplicableReason).HasColumnName("not_applicable_reason").HasMaxLength(1000);
        builder.Property(e => e.ExemptionStatus).HasColumnName("exemption_status").HasMaxLength(50).HasDefaultValue("None").IsRequired();
        builder.Property(e => e.ExemptionRequestedById).HasColumnName("exemption_requested_by_id");
        builder.Property(e => e.ExemptionRequestedAt).HasColumnName("exemption_requested_at");
        builder.Property(e => e.ExemptionReviewedById).HasColumnName("exemption_reviewed_by_id");
        builder.Property(e => e.ExemptionReviewedAt).HasColumnName("exemption_reviewed_at");
        builder.Property(e => e.ExemptionRejectionReason).HasColumnName("exemption_rejection_reason").HasMaxLength(1000);

        // Owner assignment
        builder.Property(e => e.OwnerId).HasColumnName("owner_id");

        // Blocking
        builder.Property(e => e.BlockReason).HasColumnName("block_reason").HasMaxLength(1000);

        // Evidence tracking (FTR-026 / US-045)
        builder.Property(e => e.EvidenceRequired).HasColumnName("evidence_required").HasDefaultValue(false).IsRequired();
        builder.Property(e => e.EvidenceDocumentId).HasColumnName("evidence_document_id");
        builder.Property(e => e.EvidenceSubmittedAt).HasColumnName("evidence_submitted_at");
        builder.Property(e => e.EvidenceSubmittedById).HasColumnName("evidence_submitted_by_id");

        // BaseAuditableEntity Column Mappings
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
        builder.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("NOW()");
        builder.Property(e => e.CreatedBy).HasColumnName("created_by").HasMaxLength(255);
        builder.Property(e => e.UpdatedBy).HasColumnName("updated_by").HasMaxLength(255);
        builder.Property(e => e.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);

        // Global Query Filter for Soft Delete
        builder.HasQueryFilter(e => !e.IsDeleted);

        // FK to MasterChecklistCategory (no cascade — category is a lookup)
        builder.HasOne(e => e.Category)
               .WithMany()
               .HasForeignKey(e => e.CategoryId)
               .OnDelete(DeleteBehavior.Restrict);

        // FK to User (owner — no cascade, a deleted user shouldn't take checklist items with it)
        builder.HasOne(e => e.Owner)
               .WithMany()
               .HasForeignKey(e => e.OwnerId)
               .OnDelete(DeleteBehavior.Restrict);

        // FK to User (evidence submitter — no cascade)
        builder.HasOne(e => e.EvidenceSubmittedBy)
               .WithMany()
               .HasForeignKey(e => e.EvidenceSubmittedById)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
