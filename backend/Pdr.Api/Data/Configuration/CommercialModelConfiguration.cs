using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pdr.Api.Models.Engagements;

namespace Pdr.Api.Data.Configuration;

public class CommercialModelConfiguration : IEntityTypeConfiguration<CommercialModel>
{
    public void Configure(EntityTypeBuilder<CommercialModel> builder)
    {
        builder.ToTable("commercial_models");
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id).HasColumnName("id");
        builder.Property(m => m.Name).HasColumnName("name").HasMaxLength(100).IsRequired();

        builder.HasData(
            new { Id = 1, Name = "Time and Material" },
            new { Id = 2, Name = "Fixed" },
            new { Id = 3, Name = "Capped T&M" }
        );
    }
}
