using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pdr.Api.Models.Cadence;

namespace Pdr.Api.Data.Configuration;

public class CommunicationCadenceConfiguration : IEntityTypeConfiguration<CommunicationCadence>
{
    public void Configure(EntityTypeBuilder<CommunicationCadence> builder)
    {
        builder.ToTable("communication_cadences");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).HasColumnName("id").UseIdentityAlwaysColumn();

        builder.Property(c => c.EngagementId).HasColumnName("engagement_id").IsRequired();
        builder.Property(c => c.StakeholderRole).HasColumnName("stakeholder_role").HasMaxLength(200).IsRequired();
        builder.Property(c => c.Cadence).HasColumnName("cadence").HasConversion<int>().IsRequired();
        builder.Property(c => c.Participants).HasColumnName("participants").HasMaxLength(500);

        // BaseAuditableEntity Column Mappings
        builder.Property(c => c.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
        builder.Property(c => c.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("NOW()");
        builder.Property(c => c.CreatedBy).HasColumnName("created_by").HasMaxLength(255);
        builder.Property(c => c.UpdatedBy).HasColumnName("updated_by").HasMaxLength(255);
        builder.Property(c => c.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);

        // Global Query Filter for Soft Delete
        builder.HasQueryFilter(c => !c.IsDeleted);
    }
}
