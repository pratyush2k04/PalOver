using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pdr.Api.Models.MasterChecklist;
using Microsoft.EntityFrameworkCore;

namespace Pdr.Api.Data.Configuration
{
    public class ChecklistStatusConfiguration : IEntityTypeConfiguration<ChecklistStatus>
    {
        public void Configure(EntityTypeBuilder<ChecklistStatus> builder)
        {
            builder.ToTable("checklist_statuses");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.Id)
                   .HasColumnName("id")
                   .UseIdentityAlwaysColumn();

            builder.Property(s => s.StatusName)
                   .HasColumnName("status_name")
                   .HasMaxLength(50)
                   .IsRequired();

            // Seed Data
            builder.HasData(
                new ChecklistStatus { Id = 1, StatusName = "Draft" },
                new ChecklistStatus { Id = 2, StatusName = "Published" }
            );
        }
    }
}
