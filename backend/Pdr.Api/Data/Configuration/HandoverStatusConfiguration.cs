using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pdr.Api.Models.Handover;

namespace Pdr.Api.Data.Configuration;

public class HandoverStatusConfiguration : IEntityTypeConfiguration<HandoverStatus>
{
    private static readonly DateTime SeedTimestamp = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public void Configure(EntityTypeBuilder<HandoverStatus> builder)
    {
        builder.ToTable("handover_statuses");

        builder.HasKey(s => s.HandoverStatusId);
        builder.Property(s => s.HandoverStatusId).HasColumnName("handover_status_id").UseIdentityAlwaysColumn();
        
        builder.Property(s => s.StatusName).HasColumnName("status_name").HasMaxLength(50).IsRequired();

        builder.HasData(
            new HandoverStatus { HandoverStatusId = (int)HandoverStatusEnum.Draft, StatusName = HandoverStatusNames.Draft },
            new HandoverStatus { HandoverStatusId = (int)HandoverStatusEnum.Submitted, StatusName = HandoverStatusNames.Submitted }
        );
    }
}
