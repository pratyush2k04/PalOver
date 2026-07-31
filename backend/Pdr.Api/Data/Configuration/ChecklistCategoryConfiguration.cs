using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pdr.Api.Models.MasterChecklist;

namespace Pdr.Api.Data.Configuration
{
    public class ChecklistCategoryConfiguration : IEntityTypeConfiguration<ChecklistCategory>
    {
        public void Configure(EntityTypeBuilder<ChecklistCategory> builder)
        {
            builder.ToTable("checklist_categories");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Id)
                   .HasColumnName("id")
                   .UseIdentityAlwaysColumn();

            builder.Property(c => c.CategoryName)
                   .HasColumnName("category_name")
                   .HasMaxLength(100)
                   .IsRequired();

            // Seed Data
            builder.HasData(
                new ChecklistCategory { Id = 1, CategoryName = "Commercial" },
                new ChecklistCategory { Id = 2, CategoryName = "Handover" },
                new ChecklistCategory { Id = 3, CategoryName = "Scope" },
                new ChecklistCategory { Id = 4, CategoryName = "Governance" },
                new ChecklistCategory { Id = 5, CategoryName = "Resourcing" },
                new ChecklistCategory { Id = 6, CategoryName = "Technical Readiness" }
            );
        }
    }
}
