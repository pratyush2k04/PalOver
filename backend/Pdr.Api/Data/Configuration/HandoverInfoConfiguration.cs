using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pdr.Api.Models;
using Pdr.Api.Models.Engagements;
using Pdr.Api.Models.Handover;

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

        var seedTimestamp = new System.DateTime(2026, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
        builder.HasData(
            new HandoverInfo
            {
                HandoverId = 1,
                EngagementId = 1,
                StatusId = (int)HandoverStatusEnum.Draft,
                ClientBackground = "FinTech Solutions is a mid-sized financial institution expanding its digital footprint.",
                BusinessProblem = "They need a modern core banking platform to support new digital initiatives and mobile apps.",
                ClientSentiment = "Excited but cautious about delivery timelines due to past vendor failures.",
                DealOrigin = "Strategic Partner Referral",
                CreatedAt = seedTimestamp,
                UpdatedAt = seedTimestamp,
                CreatedBy = "System",
                IsDeleted = false
            }
        );
    }
}
