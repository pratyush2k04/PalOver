using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pdr.Api.Models.Handover;
using Pdr.Api.Models.LoginEntity;

namespace Pdr.Api.Data.Configuration;

public class HandoverQueryConfiguration : IEntityTypeConfiguration<HandoverQuery>
{
    public void Configure(EntityTypeBuilder<HandoverQuery> builder)
    {
        builder.ToTable("handover_queries");

        builder.HasKey(q => q.QueryId);
        builder.Property(q => q.QueryId).HasColumnName("query_id").UseIdentityAlwaysColumn();

        builder.Property(q => q.HandoverId).HasColumnName("handover_id").IsRequired();
        builder.Property(q => q.FieldSectionId).HasColumnName("field_section_id").IsRequired();
        builder.Property(q => q.ReferenceId).HasColumnName("reference_id");
        builder.Property(q => q.IsResolved).HasColumnName("is_resolved").HasDefaultValue(false);
        builder.Property(q => q.ResolvedByUserId).HasColumnName("resolved_by_user_id");
        builder.Property(q => q.ResolvedAt).HasColumnName("resolved_at");

        // BaseAuditableEntity Column Mappings
        builder.Property(q => q.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
        builder.Property(q => q.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("NOW()");
        builder.Property(q => q.CreatedBy).HasColumnName("created_by").HasMaxLength(255);
        builder.Property(q => q.UpdatedBy).HasColumnName("updated_by").HasMaxLength(255);
        builder.Property(q => q.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);

        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.HasOne<HandoverInfo>()
            .WithMany()
            .HasForeignKey(q => q.HandoverId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(q => q.FieldSection)
            .WithMany()
            .HasForeignKey(q => q.FieldSectionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(q => q.ResolvedByUser)
            .WithMany()
            .HasForeignKey(q => q.ResolvedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(q => new { q.HandoverId, q.IsResolved });
    }
}
