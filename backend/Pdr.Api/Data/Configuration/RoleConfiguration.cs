using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pdr.Api.Models.LoginEntity;

namespace Pdr.Api.Data.Configuration
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable("roles");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.Id)
                   .HasColumnName("id");

            builder.Property(r => r.Name)
                   .HasColumnName("name")
                   .HasMaxLength(50)
                   .IsRequired();

            // CRITICAL: Seed the Roles with matching Primary Keys (1 to 4)
            builder.HasData(
                new Role { Id = 1, Name = "Pre Sales" },
                new Role { Id = 2, Name = "Project Management" },
                new Role { Id = 3, Name = "Tech Lead" },
                new Role { Id = 4, Name = "Leadership" }
            );
        }
    }
}
