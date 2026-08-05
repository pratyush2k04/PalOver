using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pdr.Api.Models.Documents;

namespace Pdr.Api.Data.Configuration;

public class DocumentConfiguration : IEntityTypeConfiguration<Document>
{
    public void Configure(EntityTypeBuilder<Document> builder)
    {
        builder.ToTable("documents");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.Id)
               .HasColumnName("id")
               .UseIdentityAlwaysColumn();

        builder.Property(d => d.EngagementId)
               .HasColumnName("engagement_id")
               .IsRequired();

        builder.Property(d => d.CategoryId)
               .HasColumnName("category_id")
               .IsRequired();

        builder.Property(d => d.FileName)
               .HasColumnName("file_name")
               .HasMaxLength(255)
               .IsRequired();

        builder.Property(d => d.FilePath)
               .HasColumnName("file_path")
               .HasMaxLength(500)
               .IsRequired();

        builder.Property(d => d.FileSizeBytes)
               .HasColumnName("file_size_bytes")
               .IsRequired();

        builder.Property(d => d.ContentType)
               .HasColumnName("content_type")
               .HasMaxLength(100)
               .IsRequired();

        builder.Property(d => d.DeletionReason)
               .HasColumnName("deletion_reason")
               .HasMaxLength(500);

        builder.Property(d => d.Visibility)
               .HasColumnName("visibility")
               .HasMaxLength(50)
               .IsRequired()
               .HasDefaultValue("Team-wide");

        // BaseAuditableEntity Column Mappings
        builder.Property(d => d.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
        builder.Property(d => d.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("NOW()");
        builder.Property(d => d.CreatedBy).HasColumnName("created_by").HasMaxLength(255);
        builder.Property(d => d.UpdatedBy).HasColumnName("updated_by").HasMaxLength(255);
        builder.Property(d => d.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);

        // Global Query Filter for Soft Delete
        builder.HasQueryFilter(d => !d.IsDeleted);

        builder.HasOne(d => d.Category)
               .WithMany(c => c.Documents)
               .HasForeignKey(d => d.CategoryId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(d => d.Engagement)
               .WithMany()
               .HasForeignKey(d => d.EngagementId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
