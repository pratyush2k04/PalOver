using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pdr.Api.Models.Engagements;

namespace Pdr.Api.Data.Configuration;

public class EngagementTypeConfiguration : IEntityTypeConfiguration<EngagementType>
{
    public void Configure(EntityTypeBuilder<EngagementType> builder)
    {
        builder.ToTable("engagement_types");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).HasColumnName("id");
        builder.Property(t => t.Name).HasColumnName("name").HasMaxLength(100).IsRequired();

        var typeSeeds = Enum.GetValues<EngagementTypeEnum>()
            .Select((t, index) => new EngagementType { Id = index + 1, Name = t.ToString() })
            .ToArray();
        builder.HasData(typeSeeds);
    }
}
