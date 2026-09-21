using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pdr.Api.Models.LoginEntity;

namespace Pdr.Api.Data.Configuration;

public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.ToTable("user_roles");

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

        // Seed System Roles (Pre Sales = 1, Leadership = 2, Delivery = 3)
        builder.HasData(
            new UserRole
            {
                Id = (int)UserRoleEnum.PreSales,
                Name = UserRoleEnum.PreSales.ToRoleName(),
                IsDeleted = false,
                CreatedBy = "System",
                CreatedAt = seededAt,
                UpdatedAt = seededAt
            },
            new UserRole
            {
                Id = (int)UserRoleEnum.Leadership,
                Name = UserRoleEnum.Leadership.ToRoleName(),
                IsDeleted = false,
                CreatedBy = "System",
                CreatedAt = seededAt,
                UpdatedAt = seededAt
            },
            new UserRole
            {
                Id = (int)UserRoleEnum.Delivery,
                Name = UserRoleEnum.Delivery.ToRoleName(),
                IsDeleted = false,
                CreatedBy = "System",
                CreatedAt = seededAt,
                UpdatedAt = seededAt
            }
        );
    }
}
