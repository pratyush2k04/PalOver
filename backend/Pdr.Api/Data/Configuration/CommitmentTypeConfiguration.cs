using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pdr.Api.Models.Handover;

namespace Pdr.Api.Data.Configuration;

public class CommitmentTypeConfiguration : IEntityTypeConfiguration<CommitmentType>
{
    private static readonly DateTime SeedTimestamp = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public void Configure(EntityTypeBuilder<CommitmentType> builder)
    {
        builder.ToTable("commitment_types");

        builder.HasKey(t => t.CommitmentTypeId);
        builder.Property(t => t.CommitmentTypeId).HasColumnName("commitment_type_id").UseIdentityAlwaysColumn();
        
        builder.Property(t => t.Type).HasColumnName("type").HasMaxLength(100).IsRequired();

        builder.HasData(
            new CommitmentType { CommitmentTypeId = 1, Type = "Delivery Milestone" },
            new CommitmentType { CommitmentTypeId = 2, Type = "Resource Allocation" },
            new CommitmentType { CommitmentTypeId = 3, Type = "Custom Requirement" }
        );
    }
}
