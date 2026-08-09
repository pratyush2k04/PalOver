using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pdr.Api.Models.Handover;

namespace Pdr.Api.Data.Configuration;

public class RealisticCommitmentConfiguration : IEntityTypeConfiguration<RealisticCommitment>
{
    public void Configure(EntityTypeBuilder<RealisticCommitment> builder)
    {
        builder.ToTable("realistic_commitments");

        builder.HasKey(rc => rc.CommitmentId);
        builder.Property(rc => rc.CommitmentId).HasColumnName("commitment_id").ValueGeneratedNever();

        builder.Property(rc => rc.Description).HasColumnName("description").HasMaxLength(2000).IsRequired();
        builder.Property(rc => rc.Timeline).HasColumnName("timeline").IsRequired();
        builder.Property(rc => rc.TimelineUnit).HasColumnName("timeline_unit").HasMaxLength(20).IsRequired();
        builder.Property(rc => rc.AcknowledgementMessage).HasColumnName("acknowledgement_message").HasMaxLength(2000);
        builder.Property(rc => rc.IsPossible).HasColumnName("is_possible").HasDefaultValue(true);

        // BaseAuditableEntity Column Mappings
        builder.Property(rc => rc.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
        builder.Property(rc => rc.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("NOW()");
        builder.Property(rc => rc.CreatedBy).HasColumnName("created_by").HasMaxLength(255);
        builder.Property(rc => rc.UpdatedBy).HasColumnName("updated_by").HasMaxLength(255);
        builder.Property(rc => rc.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);

        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.HasOne<Commitment>()
            .WithOne()
            .HasForeignKey<RealisticCommitment>(rc => rc.CommitmentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
