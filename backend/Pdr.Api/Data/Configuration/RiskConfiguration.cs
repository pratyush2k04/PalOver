using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pdr.Api.Models;
using Pdr.Api.Models.Risks;

namespace Pdr.Api.Data.Configuration;

public class RiskConfiguration : IEntityTypeConfiguration<Risk>
{
    public void Configure(EntityTypeBuilder<Risk> builder)
    {
        builder.ToTable("risks");

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).HasColumnName("id").UseIdentityAlwaysColumn();

        builder.Property(r => r.EngagementId).HasColumnName("engagement_id").IsRequired();
        builder.Property(r => r.Title).HasColumnName("title").HasMaxLength(300).IsRequired();
        builder.Property(r => r.Description).HasColumnName("description").HasMaxLength(2000).IsRequired();

        builder.Property(r => r.Impact)
            .HasColumnName("impact")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(r => r.Likelihood)
            .HasColumnName("likelihood")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(r => r.OwnerId).HasColumnName("owner_id").IsRequired(false);

        builder.Property(r => r.Mitigation).HasColumnName("mitigation").HasMaxLength(2000);

        builder.Property(r => r.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(r => r.ResolutionNote).HasColumnName("resolution_note").HasMaxLength(2000);

        builder.Property(r => r.SourceType).HasColumnName("source_type").HasMaxLength(100);
        builder.Property(r => r.SourceItemId).HasColumnName("source_item_id");
        builder.Property(r => r.SourceTitle).HasColumnName("source_title").HasMaxLength(300);

        // Audit fields
        builder.Property(r => r.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
        builder.Property(r => r.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("NOW()");
        builder.Property(r => r.CreatedBy).HasColumnName("created_by").HasMaxLength(255);
        builder.Property(r => r.UpdatedBy).HasColumnName("updated_by").HasMaxLength(255);
        builder.Property(r => r.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);

        builder.HasQueryFilter(r => !r.IsDeleted);

        // Relationships
        builder.HasOne<Engagement>()
            .WithMany()
            .HasForeignKey(r => r.EngagementId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.Owner)
            .WithMany()
            .HasForeignKey(r => r.OwnerId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
