using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pdr.Api.Models.Documents;

namespace Pdr.Api.Data.Configuration;

public class DocumentCategoryConfiguration : IEntityTypeConfiguration<DocumentCategory>
{
    public void Configure(EntityTypeBuilder<DocumentCategory> builder)
    {
        builder.ToTable("document_categories");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
               .HasColumnName("id");

        builder.Property(c => c.CategoryName)
               .HasColumnName("category_name")
               .HasMaxLength(50)
               .IsRequired();

        // Seed Data
        builder.HasData(
            new DocumentCategory { Id = 1, CategoryName = "RFP" },
            new DocumentCategory { Id = 2, CategoryName = "Proposal" },
            new DocumentCategory { Id = 3, CategoryName = "Signed SOW" },
            new DocumentCategory { Id = 4, CategoryName = "Email Thread" },
            new DocumentCategory { Id = 5, CategoryName = "Design or Approach" },
            new DocumentCategory { Id = 6, CategoryName = "Other" }
        );
    }
}
