using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pdr.Api.Models.Documents;

namespace Pdr.Api.Data.Configuration;

public class DocumentVersionConfiguration : IEntityTypeConfiguration<DocumentVersion>
{
    public void Configure(EntityTypeBuilder<DocumentVersion> builder)
    {
        builder.ToTable("document_versions");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.Id)
               .HasColumnName("id")
               .UseIdentityAlwaysColumn();

        builder.Property(v => v.DocumentId)
               .HasColumnName("document_id")
               .IsRequired();

        builder.Property(v => v.VersionNumber)
               .HasColumnName("version_number")
               .IsRequired();

        builder.Property(v => v.FilePath)
               .HasColumnName("file_path")
               .HasMaxLength(500)
               .IsRequired();

        builder.Property(v => v.FileSizeKB)
               .HasColumnName("file_size_kb")
               .IsRequired();

        builder.Property(v => v.ContentType)
               .HasColumnName("content_type")
               .HasMaxLength(100)
               .IsRequired();

        builder.Property(v => v.ChangeNote)
               .HasColumnName("change_note")
               .HasMaxLength(200);

        builder.Property(v => v.IsCurrent)
               .HasColumnName("is_current")
               .HasDefaultValue(true);

        // BaseAuditableEntity Column Mappings
        builder.Property(v => v.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
        builder.Property(v => v.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("NOW()");
        builder.Property(v => v.CreatedBy).HasColumnName("created_by").HasMaxLength(255);
        builder.Property(v => v.UpdatedBy).HasColumnName("updated_by").HasMaxLength(255);
        builder.Property(v => v.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);

        // Global Query Filter for Soft Delete
        builder.HasQueryFilter(v => !v.IsDeleted);

        builder.HasIndex(v => new { v.DocumentId, v.IsCurrent });

        builder.HasOne(v => v.Document)
               .WithMany(d => d.Versions)
               .HasForeignKey(v => v.DocumentId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
