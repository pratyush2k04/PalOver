using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pdr.Api.Models.Handover;

namespace Pdr.Api.Data.Configuration;

public class HandoverQueryFieldSectionConfiguration : IEntityTypeConfiguration<HandoverQueryFieldSection>
{
    private static readonly DateTime SeedTimestamp = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public void Configure(EntityTypeBuilder<HandoverQueryFieldSection> builder)
    {
        builder.ToTable("handover_query_field_sections");

        builder.HasKey(s => s.HandoverQueryFieldSectionId);
        builder.Property(s => s.HandoverQueryFieldSectionId).HasColumnName("handover_query_field_section_id").UseIdentityAlwaysColumn();

        builder.Property(s => s.Name).HasColumnName("name").HasMaxLength(50).IsRequired();

        builder.HasData(
            new HandoverQueryFieldSection { HandoverQueryFieldSectionId = (int)HandoverQueryFieldSectionEnum.ClientBackground, Name = HandoverQueryFieldSectionNames.ClientBackground },
            new HandoverQueryFieldSection { HandoverQueryFieldSectionId = (int)HandoverQueryFieldSectionEnum.BusinessProblem, Name = HandoverQueryFieldSectionNames.BusinessProblem },
            new HandoverQueryFieldSection { HandoverQueryFieldSectionId = (int)HandoverQueryFieldSectionEnum.ClientSentiment, Name = HandoverQueryFieldSectionNames.ClientSentiment },
            new HandoverQueryFieldSection { HandoverQueryFieldSectionId = (int)HandoverQueryFieldSectionEnum.DealOrigin, Name = HandoverQueryFieldSectionNames.DealOrigin },
            new HandoverQueryFieldSection { HandoverQueryFieldSectionId = (int)HandoverQueryFieldSectionEnum.Commitment, Name = HandoverQueryFieldSectionNames.Commitment }
        );
    }
}
