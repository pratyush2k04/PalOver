using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pdr.Api.Models.EnvironmentTooling;

namespace Pdr.Api.Data.Configuration
{
    public class EnvironmentToolingDefinitionConfiguration : IEntityTypeConfiguration<EnvironmentToolingDefinition>
    {
        public void Configure(EntityTypeBuilder<EnvironmentToolingDefinition> builder)
        {
            builder.HasKey(x => x.EnvironmentToolingDefinitionId);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.DisplayOrder)
                .IsRequired();

            builder.Property(x => x.IsMandatory)
                .IsRequired();

            builder.Property(x => x.IsActive)
                .HasDefaultValue(true);

            builder.HasData(
                new EnvironmentToolingDefinition { EnvironmentToolingDefinitionId = 1, Name = "CI/CD", IsMandatory = true, DisplayOrder = 1, IsActive = true },
                new EnvironmentToolingDefinition { EnvironmentToolingDefinitionId = 2, Name = "Dev", IsMandatory = true, DisplayOrder = 2, IsActive = true },
                new EnvironmentToolingDefinition { EnvironmentToolingDefinitionId = 3, Name = "UAT Environment", IsMandatory = false, DisplayOrder = 3, IsActive = true }
            );
        }
    }
}
