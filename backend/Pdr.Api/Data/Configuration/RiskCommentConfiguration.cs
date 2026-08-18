using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pdr.Api.Models.Risks;

namespace Pdr.Api.Data.Configuration;

public class RiskCommentConfiguration : IEntityTypeConfiguration<RiskComment>
{
    public void Configure(EntityTypeBuilder<RiskComment> builder)
    {
        builder.ToTable("risk_comments");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).HasColumnName("id").UseIdentityAlwaysColumn();

        builder.Property(c => c.RiskId).HasColumnName("risk_id").IsRequired();
        builder.Property(c => c.AuthorId).HasColumnName("author_id").IsRequired();
        builder.Property(c => c.Content).HasColumnName("content").HasMaxLength(1000).IsRequired();
        builder.Property(c => c.IsEdited).HasColumnName("is_edited").HasDefaultValue(false);

        // Audit fields
        builder.Property(c => c.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
        builder.Property(c => c.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("NOW()");
        builder.Property(c => c.CreatedBy).HasColumnName("created_by").HasMaxLength(255);
        builder.Property(c => c.UpdatedBy).HasColumnName("updated_by").HasMaxLength(255);
        builder.Property(c => c.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);

        builder.HasQueryFilter(c => !c.IsDeleted);

        // Relationships
        builder.HasOne<Risk>()
            .WithMany()
            .HasForeignKey(c => c.RiskId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(c => c.Author)
            .WithMany()
            .HasForeignKey(c => c.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
