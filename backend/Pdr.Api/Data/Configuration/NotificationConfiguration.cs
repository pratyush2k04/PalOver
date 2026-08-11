using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pdr.Api.Models.LoginEntity;
using Pdr.Api.Models.Notifications;

namespace Pdr.Api.Data.Configuration;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("notifications");

        builder.HasKey(n => n.Id);
        builder.Property(n => n.Id).HasColumnName("id");

        builder.Property(n => n.TemplateId).HasColumnName("template_id");
        builder.Property(n => n.ReceiverUserId).HasColumnName("receiver_user_id").IsRequired();
        builder.Property(n => n.Priority).HasColumnName("priority").IsRequired();
        builder.Property(n => n.ReadAt).HasColumnName("read_at");
        builder.Property(n => n.ScheduledFor).HasColumnName("scheduled_for").IsRequired();
        builder.Property(n => n.Title).HasColumnName("title").HasMaxLength(200);
        builder.Property(n => n.Body).HasColumnName("body").HasMaxLength(1000);
        builder.Property(n => n.Data).HasColumnName("data").HasColumnType("jsonb");

        // BaseAuditableEntity Column Mappings
        builder.Property(n => n.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
        builder.Property(n => n.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("NOW()");
        builder.Property(n => n.CreatedBy).HasColumnName("created_by").HasMaxLength(255);
        builder.Property(n => n.UpdatedBy).HasColumnName("updated_by").HasMaxLength(255);
        builder.Property(n => n.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);

        builder.HasQueryFilter(n => !n.IsDeleted);

        builder.HasIndex(n => new { n.ReceiverUserId, n.ReadAt });

        builder.HasOne(n => n.Template)
            .WithMany()
            .HasForeignKey(n => n.TemplateId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(n => n.Receiver)
            .WithMany()
            .HasForeignKey(n => n.ReceiverUserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
