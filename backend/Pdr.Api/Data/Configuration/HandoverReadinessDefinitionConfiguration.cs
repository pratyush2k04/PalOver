using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pdr.Api.Models.HandoverReadiness;

namespace Pdr.Api.Data.Configuration;

public class HandoverReadinessDefinitionConfiguration : IEntityTypeConfiguration<HandoverReadinessDefinition>
{
    public void Configure(EntityTypeBuilder<HandoverReadinessDefinition> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Area)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.DisplayOrder)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .HasDefaultValue(true);

        builder.HasData(
            new HandoverReadinessDefinition { Id = 1, Area = "COMMERCIAL", Name = "Sign commercial contract", DisplayOrder = 1, IsActive = true },
            new HandoverReadinessDefinition { Id = 2, Area = "COMMERCIAL", Name = "Confirm engagement budget", DisplayOrder = 2, IsActive = true },
            new HandoverReadinessDefinition { Id = 3, Area = "COMMERCIAL", Name = "Confirm billing schedule", DisplayOrder = 3, IsActive = true },
            new HandoverReadinessDefinition { Id = 4, Area = "HANDOVER", Name = "Complete sales-to-delivery handover call", DisplayOrder = 4, IsActive = true },
            new HandoverReadinessDefinition { Id = 5, Area = "HANDOVER", Name = "Share CRM opportunity notes", DisplayOrder = 5, IsActive = true },
            new HandoverReadinessDefinition { Id = 6, Area = "HANDOVER", Name = "Transfer key client contacts", DisplayOrder = 6, IsActive = true }
        );
    }
}
