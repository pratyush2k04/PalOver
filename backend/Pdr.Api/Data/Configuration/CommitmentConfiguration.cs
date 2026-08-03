using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pdr.Api.Models.Handover;

namespace Pdr.Api.Data.Configuration;

public class CommitmentConfiguration : IEntityTypeConfiguration<Commitment>
{
    public void Configure(EntityTypeBuilder<Commitment> builder)
    {
        builder.ToTable("commitments");

        builder.HasKey(c => c.CommitmentId);
        builder.Property(c => c.CommitmentId).HasColumnName("commitment_id").UseIdentityAlwaysColumn();
        
        builder.Property(c => c.HandoverId).HasColumnName("handover_id").IsRequired();
        builder.Property(c => c.Description).HasColumnName("description").HasMaxLength(2000).IsRequired();
        builder.Property(c => c.CommitmentTypeId).HasColumnName("commitment_type_id").IsRequired();
        builder.Property(c => c.Quantity).HasColumnName("quantity");

        // BaseAuditableEntity Column Mappings
        builder.Property(c => c.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
        builder.Property(c => c.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("NOW()");
        builder.Property(c => c.CreatedBy).HasColumnName("created_by").HasMaxLength(255);
        builder.Property(c => c.UpdatedBy).HasColumnName("updated_by").HasMaxLength(255);
        builder.Property(c => c.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
