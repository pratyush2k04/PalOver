using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pdr.Api.Models.Audit;

namespace Pdr.Api.Data.Configuration;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("audit_logs");

        builder.HasKey(x => x.Id);

        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.AuditActionId);
        builder.HasIndex(x => x.EngagementId);
        builder.HasIndex(x => new { x.EngagementId, x.AuditActionId });

        builder.Property(x => x.PayloadJson)
            .IsRequired();

        builder.Property(x => x.StaticFallback)
            .HasColumnName("static_fallback")
            .IsRequired(false);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.AuditAction)
            .WithMany()
            .HasForeignKey(x => x.AuditActionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
