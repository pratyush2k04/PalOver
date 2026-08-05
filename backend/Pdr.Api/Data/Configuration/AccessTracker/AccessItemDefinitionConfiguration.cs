using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pdr.Api.Models.AccessTracker;

namespace Pdr.Api.Data.Configuration.AccessTracker
{
    public class AccessItemDefinitionConfiguration : IEntityTypeConfiguration<AccessItemDefinition>
    {
        public void Configure(EntityTypeBuilder<AccessItemDefinition> builder)
        {
            builder.HasKey(x => x.AccessItemDefinitionId);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.DisplayOrder)
                .IsRequired();

            builder.Property(x => x.IsClientSide)
                .IsRequired();

            builder.Property(x => x.IsActive)
                .HasDefaultValue(true);

            builder.HasData(
                new AccessItemDefinition { AccessItemDefinitionId = 1, Name = "VPN Access", IsClientSide = false, DisplayOrder = 1, IsActive = true },
                new AccessItemDefinition { AccessItemDefinitionId = 2, Name = "Jira", IsClientSide = true, DisplayOrder = 2, IsActive = true },
                new AccessItemDefinition { AccessItemDefinitionId = 3, Name = "SandBox", IsClientSide = true, DisplayOrder = 3, IsActive = true }
            );
        }
    }
}
