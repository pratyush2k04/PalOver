using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pdr.Api.Models.ArchitectureValidation;

namespace Pdr.Api.Data.Configuration;

public class ArchitectureValidationConfiguration : IEntityTypeConfiguration<ArchitectureValidation>
{
    public void Configure(EntityTypeBuilder<ArchitectureValidation> builder)
    {
        builder.ToTable("architecture_validations");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Rationale)
            .HasMaxLength(500);

        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.HasOne(x => x.Engagement)
            .WithMany()
            .HasForeignKey(x => x.EngagementId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Features)
            .WithOne(f => f.ArchitectureValidation)
            .HasForeignKey(f => f.ArchitectureValidationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
