using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pdr.Api.Models.LoginEntity;

namespace Pdr.Api.Data.Configuration;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    private static readonly DateTime SeedTimestamp = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id).HasColumnName("id");
        builder.Property(u => u.FullName).HasColumnName("full_name").HasMaxLength(255).IsRequired();
        builder.Property(u => u.Email).HasColumnName("email").HasMaxLength(255).IsRequired();

        // Performance Indexes
        builder.HasIndex(u => u.Email, "idx_users_email").IsUnique();
        builder.HasIndex(u => u.RoleId, "idx_users_role_id");

        builder.Property(u => u.Password).HasColumnName("password").HasMaxLength(255).IsRequired();
        builder.Property(u => u.RoleId).HasColumnName("role_id").IsRequired();
        builder.Property(u => u.IsAdmin).HasColumnName("is_admin").IsRequired();
        builder.Property(u => u.FailedAttemptCount).HasColumnName("failed_attempt_count").HasDefaultValue(0);
        builder.Property(u => u.LockoutEnd).HasColumnName("lockout_end");

        // BaseAuditableEntity Column Mappings
        builder.Property(u => u.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);
        builder.Property(u => u.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
        builder.Property(u => u.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("NOW()");
        builder.Property(u => u.CreatedBy).HasColumnName("created_by").HasMaxLength(100);
        builder.Property(u => u.UpdatedBy).HasColumnName("updated_by").HasMaxLength(100);
        builder.Property(u => u.LastLoginAt).HasColumnName("last_login_at");

        // Global Query Filter for Soft Delete
        builder.HasQueryFilter(u => !u.IsDeleted);

        builder.HasOne(u => u.Role)
            .WithMany(r => r.Users)
            .HasForeignKey(u => u.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

        // Seed System Users with static values
        builder.HasData(
            new User
            {
                Id = 1,
                FullName = "Rahul Verma",
                Email = "presales@company.com",
                Password = "Password@123",
                RoleId = (int)UserRoleEnum.PreSales,
                IsAdmin = false,
                IsDeleted = false,
                CreatedBy = "System",
                CreatedAt = SeedTimestamp,
                UpdatedAt = SeedTimestamp
            },
            new User
            {
                Id = 2,
                FullName = "Asha Rao",
                Email = "pm@company.com",
                Password = "Password@123",
                RoleId = (int)UserRoleEnum.Delivery,
                IsAdmin = false,
                IsDeleted = false,
                CreatedBy = "System",
                CreatedAt = SeedTimestamp,
                UpdatedAt = SeedTimestamp
            },
            new User
            {
                Id = 3,
                FullName = "Vikram Singh",
                Email = "techlead@company.com",
                Password = "Password@123",
                RoleId = (int)UserRoleEnum.Delivery,
                IsAdmin = false,
                IsDeleted = false,
                CreatedBy = "System",
                CreatedAt = SeedTimestamp,
                UpdatedAt = SeedTimestamp
            },
            new User
            {
                Id = 4,
                FullName = "Neha Sharma",
                Email = "leadership@company.com",
                Password = "Password@123",
                RoleId = (int)UserRoleEnum.Leadership,
                IsAdmin = false,
                IsDeleted = false,
                CreatedBy = "System",
                CreatedAt = SeedTimestamp,
                UpdatedAt = SeedTimestamp
            },
            new User
            {
                Id = 5,
                FullName = "Admin User",
                Email = "admin@company.com",
                Password = "Password@123",
                RoleId = (int)UserRoleEnum.Leadership,
                IsAdmin = true,
                IsDeleted = false,
                CreatedBy = "System",
                CreatedAt = SeedTimestamp,
                UpdatedAt = SeedTimestamp
            }
        );
    }
}
