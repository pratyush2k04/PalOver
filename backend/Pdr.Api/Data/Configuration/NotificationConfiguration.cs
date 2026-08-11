using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pdr.Api.Models.LoginEntity;
using Pdr.Api.Models.Notifications;

namespace Pdr.Api.Data.Configuration;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("notifications", t =>
            t.HasCheckConstraint("ck_notifications_read_at_after_scheduled", "read_at IS NULL OR read_at >= scheduled_for"));

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
        var mockCreatedAt = new DateTime(2026, 8, 1, 0, 0, 0, DateTimeKind.Utc);

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
            },
            // Explicit (non-templated) mock notifications for User 1 (Rahul Verma) exercising every
            // Notifications endpoint and status combination. Not tied to real domain events.
            new Notification
            {
                Id = 2,
                TemplateId = null,
                ReceiverUserId = 1,
                Priority = NotificationPriority.Medium,
                ReadAt = null,
                ScheduledFor = new DateTime(2026, 8, 5, 0, 0, 0, DateTimeKind.Utc), // in the past → Status: Sent
                Title = "MOCK DB ENTRY notification — mark as read test",
                Body = "This is a MOCK DB ENTRY notification seeded for manual testing. Use PUT /api/Notifications/2/read to mark it read, then confirm GET /api/Notifications/unread-count decreases by one.",
                Data = null,
                Link = "/engagements/1/workspace/overview",
                CreatedAt = mockCreatedAt,
                UpdatedAt = mockCreatedAt,
                CreatedBy = "System",
                IsDeleted = false
            },
            new Notification
            {
                Id = 3,
                TemplateId = null,
                ReceiverUserId = 1,
                Priority = NotificationPriority.High,
                ReadAt = null,
                ScheduledFor = new DateTime(2026, 8, 6, 0, 0, 0, DateTimeKind.Utc), // in the past → Status: Sent
                Title = "MOCK DB ENTRY notification — Engagement 1 filter test",
                Body = "This is a MOCK DB ENTRY notification seeded for manual testing. Use GET /api/Notifications?engagementId=1 to confirm this entry is included in the filtered results.",
                Data = "{\"EngagementId\":1}",
                Link = "/engagements/1/workspace/handover",
                CreatedAt = mockCreatedAt,
                UpdatedAt = mockCreatedAt,
                CreatedBy = "System",
                IsDeleted = false
            },
            new Notification
            {
                Id = 4,
                TemplateId = null,
                ReceiverUserId = 1,
                Priority = NotificationPriority.Low,
                ReadAt = null,
                ScheduledFor = new DateTime(2026, 8, 7, 0, 0, 0, DateTimeKind.Utc), // in the past → Status: Sent
                Title = "MOCK DB ENTRY notification — Engagement 2, no link",
                Body = "This is a MOCK DB ENTRY notification seeded for manual testing. Use GET /api/Notifications?engagementId=2 to confirm this entry is included, and GET /api/Notifications?engagementId=1 to confirm it's excluded. Link is intentionally null to test handling of notifications with no redirect target.",
                Data = "{\"EngagementId\":2}",
                Link = null,
                CreatedAt = mockCreatedAt,
                UpdatedAt = mockCreatedAt,
                CreatedBy = "System",
                IsDeleted = false
            },
            new Notification
            {
                Id = 5,
                TemplateId = null,
                ReceiverUserId = 1,
                Priority = NotificationPriority.Medium,
                ReadAt = null,
                ScheduledFor = new DateTime(2026, 9, 1, 0, 0, 0, DateTimeKind.Utc), // in the future → Status: Pending
                Title = "MOCK DB ENTRY notification — Pending (future scheduled)",
                Body = "This is a MOCK DB ENTRY notification seeded for manual testing. ScheduledFor is in the future, so it should show Status: Pending and must be EXCLUDED from GET /api/Notifications/unread-count even though it is unread. Note: PUT /api/Notifications/read-all will still mark it read, since that endpoint doesn't check ScheduledFor.",
                Data = null,
                Link = "/engagements/2/workspace/overview",
                CreatedAt = mockCreatedAt,
                UpdatedAt = mockCreatedAt,
                CreatedBy = "System",
                IsDeleted = false
            },
            new Notification
            {
                Id = 6,
                TemplateId = null,
                ReceiverUserId = 1,
                Priority = NotificationPriority.Low,
                ReadAt = new DateTime(2026, 8, 4, 0, 0, 0, DateTimeKind.Utc), // already read → Status: Read
                ScheduledFor = new DateTime(2026, 8, 3, 0, 0, 0, DateTimeKind.Utc),
                Title = "MOCK DB ENTRY notification — already read, safe to delete",
                Body = "This is a MOCK DB ENTRY notification seeded for manual testing. It is already marked read and is intended as the target for DELETE /api/Notifications — send { \"ids\": [6] } to remove it and confirm it no longer appears in GET /api/Notifications.",
                Data = null,
                Link = null,
                CreatedAt = mockCreatedAt,
                UpdatedAt = mockCreatedAt,
                CreatedBy = "System",
                IsDeleted = false
            }
        );
    }
}
