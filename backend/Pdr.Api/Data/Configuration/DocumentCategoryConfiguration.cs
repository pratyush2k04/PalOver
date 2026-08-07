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
            new DocumentCategory { Id = (int)DocumentCategoryEnum.Rfp, CategoryName = DocumentCategoryNames.Rfp },
            new DocumentCategory { Id = (int)DocumentCategoryEnum.Proposal, CategoryName = DocumentCategoryNames.Proposal },
            new DocumentCategory { Id = (int)DocumentCategoryEnum.SignedSow, CategoryName = DocumentCategoryNames.SignedSow },
            new DocumentCategory { Id = (int)DocumentCategoryEnum.EmailThread, CategoryName = DocumentCategoryNames.EmailThread },
            new DocumentCategory { Id = (int)DocumentCategoryEnum.DesignOrApproach, CategoryName = DocumentCategoryNames.DesignOrApproach },
            new DocumentCategory { Id = (int)DocumentCategoryEnum.Other, CategoryName = DocumentCategoryNames.Other },
            new DocumentCategory { Id = (int)DocumentCategoryEnum.Commercial, CategoryName = DocumentCategoryNames.Commercial }
        );
    }
}
