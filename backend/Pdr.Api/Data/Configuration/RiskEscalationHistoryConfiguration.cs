using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pdr.Api.Models.Risks;

namespace Pdr.Api.Data.Configuration;

public class RiskEscalationHistoryConfiguration : IEntityTypeConfiguration<RiskEscalationHistory>
{
    public void Configure(EntityTypeBuilder<RiskEscalationHistory> builder)
    {
        builder.ToTable("risk_escalation_histories");

        builder.HasKey(h => h.Id);
        builder.Property(h => h.Id).HasColumnName("id");

        builder.Property(h => h.RiskId).HasColumnName("risk_id").IsRequired();
        builder.Property(h => h.EscalatedAt).HasColumnName("escalated_at").IsRequired();
        builder.Property(h => h.EscalationLevel)
            .HasColumnName("escalation_level")
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();
        builder.Property(h => h.EscalatedToUserId).HasColumnName("escalated_to_user_id");
        builder.Property(h => h.EscalatedToRole).HasColumnName("escalated_to_role").HasMaxLength(100).IsRequired();
        builder.Property(h => h.Reason).HasColumnName("reason").HasMaxLength(1000).IsRequired();

        // BaseAuditableEntity Column Mappings
        builder.Property(h => h.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
        builder.Property(h => h.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("NOW()");
        builder.Property(h => h.CreatedBy).HasColumnName("created_by").HasMaxLength(255);
        builder.Property(h => h.UpdatedBy).HasColumnName("updated_by").HasMaxLength(255);
        builder.Property(h => h.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);

        builder.HasQueryFilter(h => !h.IsDeleted);

        builder.HasOne(h => h.Risk)
            .WithMany()
            .HasForeignKey(h => h.RiskId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(h => h.EscalatedToUser)
            .WithMany()
            .HasForeignKey(h => h.EscalatedToUserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
