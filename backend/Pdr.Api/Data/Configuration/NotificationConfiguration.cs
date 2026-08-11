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
        builder.Property(n => n.Link).HasColumnName("link").HasColumnType("text");

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

        var seedTimestamp = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        builder.HasData(
            new Notification
            {
                Id = 1,
                TemplateId = 2, // PMAllocated
                ReceiverUserId = 1, // Rahul Verma — PreSales on Engagement 1
                Priority = NotificationPriority.Medium, // matches PMAllocated template's DefaultPriority
                ReadAt = null,
                ScheduledFor = seedTimestamp,
                Title = null,
                Body = null,
                Data = "{\"AssignedUserId\":2,\"EngagementId\":1}", // Asha Rao (User 2) is the seeded PM on Engagement 1
                Link = "/engagements/1/workspace/overview",
                CreatedAt = seedTimestamp,
                UpdatedAt = seedTimestamp,
                CreatedBy = "System",
                IsDeleted = false
            }
        );
    }
}
