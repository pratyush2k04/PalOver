using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pdr.Api.Models.Engagements;

namespace Pdr.Api.Data.Configuration;

public class EngagementStatusConfiguration : IEntityTypeConfiguration<EngagementStatus>
{
    public void Configure(EntityTypeBuilder<EngagementStatus> builder)
    {
        builder.ToTable("engagement_statuses");
        builder.HasKey(s => s.StatusId);
        builder.Property(s => s.StatusId).HasColumnName("status_id");
        builder.Property(s => s.StatusName).HasColumnName("status_name").HasMaxLength(50).IsRequired();

        var statusSeeds = Enum.GetValues<EngagementStatusEnum>()
            .Select((s, index) => new EngagementStatus { StatusId = index + 1, StatusName = s.ToString() })
            .ToArray();
        builder.HasData(statusSeeds);
    }
}
