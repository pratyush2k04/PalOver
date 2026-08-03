using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pdr.Api.Models.MasterChecklist;
using Microsoft.EntityFrameworkCore;

namespace Pdr.Api.Data.Configuration
{
    public class MasterChecklistStatusConfiguration : IEntityTypeConfiguration<MasterChecklistStatus>
    {
        public void Configure(EntityTypeBuilder<MasterChecklistStatus> builder)
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
                new MasterChecklistStatus { Id = 1, StatusName = "Draft" },
                new MasterChecklistStatus { Id = 2, StatusName = "Published" }
            );
        }
    }
}
