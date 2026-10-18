using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pdr.Api.Models.BrownField;

namespace Pdr.Api.Data.Configuration;

public class BrownfieldFindingConfiguration : IEntityTypeConfiguration<BrownfieldFinding>
{
    public void Configure(EntityTypeBuilder<BrownfieldFinding> builder)
    {
        builder.ToTable("brownfield_findings");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Description)
            .HasMaxLength(1000);

        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.HasOne(x => x.Engagement)
            .WithMany()
            .HasForeignKey(x => x.EngagementId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Document)
            .WithMany()
            .HasForeignKey(x => x.DocumentId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
