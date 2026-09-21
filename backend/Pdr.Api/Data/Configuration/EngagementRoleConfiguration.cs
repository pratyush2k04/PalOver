using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pdr.Api.Models.LoginEntity;

namespace Pdr.Api.Data.Configuration;

public class EngagementRoleConfiguration : IEntityTypeConfiguration<EngagementRole>
{
    public void Configure(EntityTypeBuilder<EngagementRole> builder)
    {
        builder.ToTable("engagement_roles");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
               .HasColumnName("id")
               .ValueGeneratedNever();

        builder.Property(r => r.Name)
               .HasColumnName("name")
               .HasMaxLength(50)
               .IsRequired();

        builder.HasIndex(r => r.Name)
               .IsUnique();

        // BaseAuditableEntity Column Mappings
        builder.Property(r => r.IsDeleted)
               .HasColumnName("is_deleted")
               .HasDefaultValue(false);

        builder.Property(r => r.CreatedAt)
               .HasColumnName("created_at")
               .HasDefaultValueSql("NOW()");

        builder.Property(r => r.UpdatedAt)
               .HasColumnName("updated_at")
               .HasDefaultValueSql("NOW()");

        builder.Property(r => r.CreatedBy)
               .HasColumnName("created_by")
               .HasMaxLength(100);

        builder.Property(r => r.UpdatedBy)
               .HasColumnName("updated_by")
               .HasMaxLength(100);

        // Fixed baseline date for seed data
        var seededAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        // Seed Engagement Roles (Pre Sales = 1, Leadership = 2, Project Manager = 3, Tech Lead = 4)
        builder.HasData(
            new EngagementRole
            {
                Id = (int)EngagementRoleEnum.PreSales,
                Name = EngagementRoleEnum.PreSales.ToRoleName(),
                IsDeleted = false,
                CreatedBy = "System",
                CreatedAt = seededAt,
                UpdatedAt = seededAt
            },
            new EngagementRole
            {
                Id = (int)EngagementRoleEnum.Leadership,
                Name = EngagementRoleEnum.Leadership.ToRoleName(),
                IsDeleted = false,
                CreatedBy = "System",
                CreatedAt = seededAt,
                UpdatedAt = seededAt
            },
            new EngagementRole
            {
                Id = (int)EngagementRoleEnum.ProjectManager,
                Name = EngagementRoleEnum.ProjectManager.ToRoleName(),
                IsDeleted = false,
                CreatedBy = "System",
                CreatedAt = seededAt,
                UpdatedAt = seededAt
            },
            new EngagementRole
            {
                Id = (int)EngagementRoleEnum.TechLead,
                Name = EngagementRoleEnum.TechLead.ToRoleName(),
                IsDeleted = false,
                CreatedBy = "System",
                CreatedAt = seededAt,
                UpdatedAt = seededAt
            }
        );
    }
}
