using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pdr.Api.Models.MasterChecklist;

namespace Pdr.Api.Data.Configuration
{
    public class MasterChecklistCategoryConfiguration : IEntityTypeConfiguration<MasterChecklistCategory>
    {
        public void Configure(EntityTypeBuilder<MasterChecklistCategory> builder)
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
                new MasterChecklistCategory { Id = 1, CategoryName = "Commercial" },
                new MasterChecklistCategory { Id = 4, CategoryName = "Governance" },
                new MasterChecklistCategory { Id = 5, CategoryName = "Resourcing" }
            );
        }
    }
}
