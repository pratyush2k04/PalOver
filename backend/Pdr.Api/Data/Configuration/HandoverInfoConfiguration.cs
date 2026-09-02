using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pdr.Api.Models;
using Pdr.Api.Models.Engagements;
using Pdr.Api.Models.Handover;
using Pdr.Api.Models.LoginEntity;

namespace Pdr.Api.Data.Configuration;

public class HandoverInfoConfiguration : IEntityTypeConfiguration<HandoverInfo>
{
    public void Configure(EntityTypeBuilder<HandoverInfo> builder)
    {
        builder.ToTable("handover_info");
        
        builder.HasKey(h => h.HandoverId);
        builder.Property(h => h.HandoverId).HasColumnName("handover_id").UseIdentityAlwaysColumn();
        
        builder.Property(h => h.EngagementId).HasColumnName("engagement_id").IsRequired();
        builder.Property(h => h.StatusId).HasColumnName("status_id").IsRequired();
        builder.Property(h => h.ClientBackground).HasColumnName("client_background").HasMaxLength(2000);
        builder.Property(h => h.BusinessProblem).HasColumnName("business_problem").HasMaxLength(2000).IsRequired();
        builder.Property(h => h.ClientSentiment).HasColumnName("client_sentiment").IsRequired();
        builder.Property(h => h.DealOrigin).HasColumnName("deal_origin").HasMaxLength(2000);
        builder.Property(h => h.SubmittedAt).HasColumnName("submitted_at");
        builder.Property(h => h.SubmittedByUserId).HasColumnName("submitted_by_user_id");
        builder.Property(h => h.ReviewedAt).HasColumnName("reviewed_at");
        builder.Property(h => h.ReviewedByUserId).HasColumnName("reviewed_by_user_id");

        // BaseAuditableEntity Column Mappings
        builder.Property(h => h.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
        builder.Property(h => h.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("NOW()");
        builder.Property(h => h.CreatedBy).HasColumnName("created_by").HasMaxLength(255);
        builder.Property(h => h.UpdatedBy).HasColumnName("updated_by").HasMaxLength(255);
        builder.Property(h => h.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);

        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.HasMany<Commitment>()
            .WithOne()
            .HasForeignKey(c => c.HandoverId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Engagement>()
            .WithOne(e => e.HandoverInfo)
            .HasForeignKey<HandoverInfo>(h => h.EngagementId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(h => h.SubmittedByUser)
            .WithMany()
            .HasForeignKey(h => h.SubmittedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(h => h.ReviewedByUser)
            .WithMany()
            .HasForeignKey(h => h.ReviewedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
