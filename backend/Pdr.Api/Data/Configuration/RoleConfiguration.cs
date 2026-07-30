using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
//using Pdr.Api.Models.RoleBasedPermissions;
using Pdr.Api.Models.LoginEntity;

namespace Pdr.Api.Data.Configuration;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("roles");

        builder.HasKey(r => r.Id);

        // Fixed explicit primary key IDs 1 to 4 (no GENERATED ALWAYS AS IDENTITY)
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

        // Seed System Roles
        builder.HasData(
            new Role { Id = 1, Name = "Pre Sales", IsDeleted = false, CreatedBy = "System" },
            new Role { Id = 2, Name = "Project Management", IsDeleted = false, CreatedBy = "System" },
            new Role { Id = 3, Name = "Tech Lead", IsDeleted = false, CreatedBy = "System" },
            new Role { Id = 4, Name = "Leadership", IsDeleted = false, CreatedBy = "System" }
        );
    }
}
