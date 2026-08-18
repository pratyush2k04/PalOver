using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pdr.Api.Models.Notifications;

namespace Pdr.Api.Data.Configuration;

public class NotificationTemplateConfiguration : IEntityTypeConfiguration<NotificationTemplate>
{
    public void Configure(EntityTypeBuilder<NotificationTemplate> builder)
    {
        builder.ToTable("notification_templates");

        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).HasColumnName("id");

        builder.Property(t => t.Key).HasColumnName("key").HasMaxLength(100).IsRequired();
        builder.Property(t => t.Title).HasColumnName("title").HasMaxLength(200).IsRequired();
        builder.Property(t => t.Body).HasColumnName("body").HasMaxLength(1000).IsRequired();
        builder.Property(t => t.DefaultPriority).HasColumnName("default_priority").IsRequired();
        builder.Property(t => t.LinkPattern).HasColumnName("link_pattern").HasColumnType("text");

        builder.HasIndex(t => t.Key).IsUnique();

        // BaseAuditableEntity Column Mappings
        builder.Property(t => t.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
        builder.Property(t => t.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("NOW()");
        builder.Property(t => t.CreatedBy).HasColumnName("created_by").HasMaxLength(255);
        builder.Property(t => t.UpdatedBy).HasColumnName("updated_by").HasMaxLength(255);
        builder.Property(t => t.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);

        builder.HasQueryFilter(t => !t.IsDeleted);

        var seedTimestamp = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        builder.HasData(
            new NotificationTemplate
            {
                Id = 1,
                Key = "AccessItemBlocked",
                Title = "Access Item Blocked — {ItemName}",
                Body = "{ItemName} was blocked by {TriggerUserName}. Reason: {BlockReason}",
                DefaultPriority = NotificationPriority.High,
                LinkPattern = "/engagements/{EngagementId}/workspace/technical-readiness",
                CreatedAt = seedTimestamp,
                UpdatedAt = seedTimestamp,
                CreatedBy = "System",
                IsDeleted = false
            },
            new NotificationTemplate
            {
                Id = 2,
                Key = "PMAllocated",
                Title = "Project Manager Assigned to {EngagementName}",
                Body = "{PMName} has been allocated as Project Manager for {EngagementName}.",
                DefaultPriority = NotificationPriority.Medium,
                LinkPattern = "/engagements/{EngagementId}/workspace/overview",
                CreatedAt = seedTimestamp,
                UpdatedAt = seedTimestamp,
                CreatedBy = "System",
                IsDeleted = false
            },
            new NotificationTemplate
            {
                Id = 3,
                Key = "HandoverSubmitted",
                Title = "Handover Submitted — {EngagementName}",
                Body = "The handover for {EngagementName} has been submitted by {TriggerUserName} and is ready for your review.",
                DefaultPriority = NotificationPriority.High,
                LinkPattern = "/engagements/{EngagementId}/workspace/handover",
                CreatedAt = seedTimestamp,
                UpdatedAt = seedTimestamp,
                CreatedBy = "System",
                IsDeleted = false
            },
            new NotificationTemplate
            {
                Id = 11,
                Key = "KickoffReadinessReminder",
                Title = "{DaysBeforeKickoff} day(s) until kickoff — {EngagementName} readiness incomplete",
                Body = "{EngagementName} still has open mandatory items with kickoff in {DaysBeforeKickoff} day(s). Priority escalates as the date approaches.",
                DefaultPriority = NotificationPriority.Medium,
                LinkPattern = "/engagements/{EngagementId}/workspace/{TabKey}",
                CreatedAt = seedTimestamp,
                UpdatedAt = seedTimestamp,
                CreatedBy = "System",
                IsDeleted = false
            }
        );
    }
}
