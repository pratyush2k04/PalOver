using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pdr.Api.Models.LoginEntity;

namespace Pdr.Api.Data.Configuration;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("roles");

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

        // Seed System Roles with static values
        builder.HasData(
            new Role
            {
                Id = 1,
                Name = "Pre Sales",
                IsDeleted = false,
                CreatedBy = "System",
                CreatedAt = seededAt,
                UpdatedAt = seededAt
            },
            new Role
            {
                Id = 2,
                Name = "Project Management",
                IsDeleted = false,
                CreatedBy = "System",
                CreatedAt = seededAt,
                UpdatedAt = seededAt
            },
            new Role
            {
                Id = 3,
                Name = "Tech Lead",
                IsDeleted = false,
                CreatedBy = "System",
                CreatedAt = seededAt,
                UpdatedAt = seededAt
            },
            new Role
            {
                Id = 4,
                Name = "Leadership",
                IsDeleted = false,
                CreatedBy = "System",
                CreatedAt = seededAt,
                UpdatedAt = seededAt
            }
        );
    }
}
