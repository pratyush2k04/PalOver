using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pdr.Api.Models.HandoverReadiness;

namespace Pdr.Api.Data.Configuration;

public class HandoverReadinessItemConfiguration : IEntityTypeConfiguration<HandoverReadinessItem>
{
    public void Configure(EntityTypeBuilder<HandoverReadinessItem> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Area)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.BlockReason)
            .HasMaxLength(500);

        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.HasOne(x => x.Engagement)
            .WithMany(x => x.HandoverReadinessItems)
            .HasForeignKey(x => x.EngagementId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Owner)
            .WithMany(x => x.HandoverReadinessItems)
            .HasForeignKey(x => x.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.HandoverReadinessDefinition)
            .WithMany(x => x.HandoverReadinessItems)
            .HasForeignKey(x => x.HandoverReadinessDefinitionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.EvidenceDocument)
            .WithMany()
            .HasForeignKey(x => x.EvidenceDocumentId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(x => x.EvidenceSubmittedBy)
            .WithMany()
            .HasForeignKey(x => x.EvidenceSubmittedById)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
