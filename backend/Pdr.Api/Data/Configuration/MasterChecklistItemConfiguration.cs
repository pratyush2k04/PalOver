using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pdr.Api.Models.MasterChecklist;

namespace Pdr.Api.Data.Configuration;

public class MasterChecklistItemConfiguration : IEntityTypeConfiguration<MasterChecklistItem>
{
    private static readonly DateTime SeedTimestamp = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public void Configure(EntityTypeBuilder<MasterChecklistItem> builder)
    {
        builder.ToTable("master_checklist_items");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id").UseIdentityAlwaysColumn();
        builder.Property(e => e.VersionId).HasColumnName("version_id").IsRequired();
        builder.Property(e => e.Title).HasColumnName("title").HasMaxLength(255).IsRequired();
        builder.Property(e => e.Description).HasColumnName("description");
        builder.Property(e => e.CategoryId).HasColumnName("category_id").IsRequired();
        builder.Property(e => e.IsMandatory).HasColumnName("is_mandatory").HasDefaultValue(false);
        builder.Property(e => e.EvidenceRequired).HasColumnName("evidence_required").HasDefaultValue(false);
        builder.Property(e => e.DisplayOrder).HasColumnName("display_order").IsRequired();
        builder.Property(e => e.IsRetired).HasColumnName("is_retired").HasDefaultValue(false);

        // BaseAuditableEntity Column Mappings
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
        builder.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("NOW()");
        builder.Property(e => e.CreatedBy).HasColumnName("created_by").HasMaxLength(255);
        builder.Property(e => e.UpdatedBy).HasColumnName("updated_by").HasMaxLength(255);
        builder.Property(e => e.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);

        // Global Query Filter for Soft Delete
        builder.HasQueryFilter(e => !e.IsDeleted);

        builder.HasOne(d => d.Version)
            .WithMany(p => p.Items)
            .HasForeignKey(d => d.VersionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(d => d.Category)
            .WithMany()
            .HasForeignKey(d => d.CategoryId);

        builder.HasData(GetMasterChecklistItemSeedData());
    }

    private static object[] GetMasterChecklistItemSeedData()
    {
        var items = new (int IdOffset, string Title, string Description, int CategoryId, bool IsMandatory, bool EvidenceRequired)[]
        {
            (7, "Document engagement scope", "Capture in-scope and out-of-scope items in the kickoff scope document.", 3, true, true),
            (8, "Confirm deliverables list", "List and confirm all deliverables committed to the client.", 3, false, false),
            (9, "Identify scope assumptions and exclusions", "Record assumptions made during the sales cycle that affect scope.", 3, false, false),
            (10, "Set up engagement governance cadence", "Agree recurring status meetings and escalation paths.", 4, false, false),
            (11, "Assign engagement steering committee", "Identify decision-makers on both client and delivery sides.", 4, false, false),
            (12, "Define change control process", "Agree how scope changes will be raised, approved, and tracked.", 4, false, false),
            (13, "Confirm RAID log owner", "Assign an owner to maintain the Risks, Assumptions, Issues, and Dependencies log.", 4, false, false),
            (14, "Allocate delivery team members", "Confirm named resources are assigned and available for the engagement.", 5, false, false),
            (15, "Confirm resource availability calendar", "Check assigned resources have capacity for the engagement duration.", 5, false, false),
            (16, "Identify backup/on-call resources", "Name backup resources in case of absence during the engagement.", 5, false, false)
        };

        var seed = new List<object>();
        var versions = new[] { (VersionId: 1, BaseId: 0), (VersionId: 2, BaseId: 20) };
        foreach (var (versionId, baseId) in versions)
        {
            for (var i = 0; i < items.Length; i++)
            {
                var (idOffset, title, description, categoryId, isMandatory, evidenceRequired) = items[i];
                seed.Add(new
                {
                    Id = baseId + idOffset,
                    VersionId = versionId,
                    Title = title,
                    Description = description,
                    CategoryId = categoryId,
                    IsMandatory = isMandatory,
                    EvidenceRequired = evidenceRequired,
                    DisplayOrder = i + 1,
                    IsRetired = false,
                    CreatedAt = SeedTimestamp,
                    UpdatedAt = SeedTimestamp,
                    IsDeleted = false
                });
            }
        }

        return seed.ToArray();
    }
}
