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

        // FK to Document (evidence document — deleting the document just clears the link)
        builder.HasOne(e => e.EvidenceDocument)
               .WithMany()
               .HasForeignKey(e => e.EvidenceDocumentId)
               .OnDelete(DeleteBehavior.SetNull);

        builder.HasData(GetSeedItems());
    }

    private static object[] GetSeedItems()
    {
        var seedTimestamp = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        // Master items 1-20 belong to EngagementChecklistId=1 (Engagement 1)
        // Master items 1-20 are copied again for EngagementChecklistId=2 (Engagement 2)
        // (same 20 items, two engagements)
        var masterItems = new[]
        {
            (Title: "Sign commercial contract",                      CategoryId: 1, IsMandatory: true,  EvidenceRequired: true, DisplayOrder: 1,  MasterItemId: 1),
            (Title: "Confirm engagement budget",                     CategoryId: 1, IsMandatory: true,  EvidenceRequired: true, DisplayOrder: 2,  MasterItemId: 2),
            (Title: "Confirm billing schedule",                      CategoryId: 1, IsMandatory: true,  EvidenceRequired: true, DisplayOrder: 3,  MasterItemId: 3),
            (Title: "Complete sales-to-delivery handover call",      CategoryId: 2, IsMandatory: true,  EvidenceRequired: true, DisplayOrder: 4,  MasterItemId: 4),
            (Title: "Share CRM opportunity notes",                   CategoryId: 2, IsMandatory: true,  EvidenceRequired: true, DisplayOrder: 5,  MasterItemId: 5),
            (Title: "Transfer key client contacts",                  CategoryId: 2, IsMandatory: true,  EvidenceRequired: true, DisplayOrder: 6,  MasterItemId: 6),
            (Title: "Document engagement scope",                     CategoryId: 3, IsMandatory: true,  EvidenceRequired: true, DisplayOrder: 7,  MasterItemId: 7),
            (Title: "Confirm deliverables list",                     CategoryId: 3, IsMandatory: true,  EvidenceRequired: true, DisplayOrder: 8,  MasterItemId: 8),
            (Title: "Identify scope assumptions and exclusions",     CategoryId: 3, IsMandatory: false, EvidenceRequired: true, DisplayOrder: 9,  MasterItemId: 9),
            (Title: "Set up engagement governance cadence",          CategoryId: 4, IsMandatory: true,  EvidenceRequired: true, DisplayOrder: 10, MasterItemId: 10),
            (Title: "Assign engagement steering committee",          CategoryId: 4, IsMandatory: true,  EvidenceRequired: true, DisplayOrder: 11, MasterItemId: 11),
            (Title: "Define change control process",                 CategoryId: 4, IsMandatory: true,  EvidenceRequired: true, DisplayOrder: 12, MasterItemId: 12),
            (Title: "Confirm RAID log owner",                        CategoryId: 4, IsMandatory: false, EvidenceRequired: true, DisplayOrder: 13, MasterItemId: 13),
            (Title: "Allocate delivery team members",                CategoryId: 5, IsMandatory: true,  EvidenceRequired: true, DisplayOrder: 14, MasterItemId: 14),
            (Title: "Confirm resource availability calendar",        CategoryId: 5, IsMandatory: true,  EvidenceRequired: true, DisplayOrder: 15, MasterItemId: 15),
            (Title: "Identify backup/on-call resources",             CategoryId: 5, IsMandatory: false, EvidenceRequired: true, DisplayOrder: 16, MasterItemId: 16),
            (Title: "Provision environment access",                  CategoryId: 6, IsMandatory: true,  EvidenceRequired: true, DisplayOrder: 17, MasterItemId: 17),
            (Title: "Verify technical prerequisites",                CategoryId: 6, IsMandatory: true,  EvidenceRequired: true, DisplayOrder: 18, MasterItemId: 18),
            (Title: "Set up source control and CI/CD access",        CategoryId: 6, IsMandatory: true,  EvidenceRequired: true, DisplayOrder: 19, MasterItemId: 19),
            (Title: "Confirm security and compliance requirements",  CategoryId: 6, IsMandatory: false, EvidenceRequired: true, DisplayOrder: 20, MasterItemId: 20),
        };

        var seed = new List<object>();
        var engagementChecklistIds = new[] { 1, 2 };
        var nextId = 1;

        foreach (var checklistId in engagementChecklistIds)
        {
            foreach (var item in masterItems)
            {
                seed.Add(new
                {
                    Id = nextId++,
                    EngagementChecklistId = checklistId,
                    SourceMasterItemId = (int?)item.MasterItemId,
                    Title = item.Title,
                    Description = (string?)null,
                    CategoryId = item.CategoryId,
                    IsMandatory = item.IsMandatory,
                    EvidenceRequired = item.EvidenceRequired,
                    DisplayOrder = item.DisplayOrder,
                    IsApplicable = true,
                    Status = "Incomplete",
                    CreatedAt = seedTimestamp,
                    UpdatedAt = seedTimestamp,
                    CreatedBy = "System",
                    UpdatedBy = (string?)null,
                    IsDeleted = false
                });
            }
        }

        return seed.ToArray();
    }
}
