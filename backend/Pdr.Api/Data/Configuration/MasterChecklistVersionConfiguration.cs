using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pdr.Api.Models.MasterChecklist;

namespace Pdr.Api.Data.Configuration;

public class MasterChecklistVersionConfiguration : IEntityTypeConfiguration<MasterChecklistVersion>
{
    private static readonly DateTime SeedTimestamp = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public void Configure(EntityTypeBuilder<MasterChecklistVersion> builder)
    {
        builder.ToTable("master_checklist_versions");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id").UseIdentityAlwaysColumn();
        builder.Property(e => e.VersionNumber).HasColumnName("version_number").IsRequired();
        builder.Property(e => e.StatusId).HasColumnName("status_id").IsRequired();

        // BaseAuditableEntity Column Mappings
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
        builder.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("NOW()");
        builder.Property(e => e.CreatedBy).HasColumnName("created_by").HasMaxLength(255);
        builder.Property(e => e.UpdatedBy).HasColumnName("updated_by").HasMaxLength(255);
        builder.Property(e => e.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);

        // Global Query Filter for Soft Delete
        builder.HasQueryFilter(e => !e.IsDeleted);

        builder.HasOne(d => d.Status)
            .WithMany()
            .HasForeignKey(d => d.StatusId);

        builder.HasData(
            new
            {
                Id = 1,
                VersionNumber = 0,
                StatusId = 2,
                CreatedAt = SeedTimestamp,
                UpdatedAt = SeedTimestamp,
                IsDeleted = false
            },
            new
            {
                Id = 2,
                VersionNumber = 1,
                StatusId = 1,
                CreatedAt = SeedTimestamp,
                UpdatedAt = SeedTimestamp,
                IsDeleted = false
            });
    }
}
