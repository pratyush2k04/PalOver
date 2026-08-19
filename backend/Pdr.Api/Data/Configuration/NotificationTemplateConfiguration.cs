using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pdr.Api.Constants;
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
                Id = 6,
                Key = NotificationConstants.RiskCommentMention.TemplateKey,
                Title = "Mentioned in Risk — {RiskTitle}",
                Body = "{AuthorName} mentioned you in a comment on risk '{RiskTitle}'",
                DefaultPriority = NotificationPriority.Medium,
                LinkPattern = "/engagements/{EngagementId}/workspace/risks",
                CreatedAt = seedTimestamp,
                UpdatedAt = seedTimestamp,
                CreatedBy = "System",
                IsDeleted = false
                },
            new NotificationTemplate
            {
                Id = 7,
                Key = NotificationConstants.TemplateKeys.LeadershipAssigned,
                Title = "Assigned to Engagement — {EngagementName}",
                Body = "You have been assigned as Engagement Leader for {EngagementName}.",
                DefaultPriority = NotificationPriority.Medium,
                LinkPattern = "/engagements/{EngagementId}/workspace/overview",
                CreatedAt = seedTimestamp,
                UpdatedAt = seedTimestamp,
                CreatedBy = "System",
                IsDeleted = false
            },
            new NotificationTemplate
            {
                Id = 8,
                Key = NotificationConstants.TemplateKeys.PMAssigned,
                Title = "Assigned as Project Manager — {EngagementName}",
                Body = "You have been assigned as Project Manager for {EngagementName}.",
                DefaultPriority = NotificationPriority.Medium,
                LinkPattern = "/engagements/{EngagementId}/workspace/overview",
                CreatedAt = seedTimestamp,
                UpdatedAt = seedTimestamp,
                CreatedBy = "System",
                IsDeleted = false
            },
            new NotificationTemplate
            {
                Id = 9,
                Key = NotificationConstants.TemplateKeys.TechLeadAssigned,
                Title = "Assigned as Technical Lead — {EngagementName}",
                Body = "You have been assigned as Technical Lead for {EngagementName}.",
                DefaultPriority = NotificationPriority.Medium,
                LinkPattern = "/engagements/{EngagementId}/workspace/overview",
                CreatedAt = seedTimestamp,
                UpdatedAt = seedTimestamp,
                CreatedBy = "System",
                IsDeleted = false
            },
            new NotificationTemplate
            {
                Id = 10,
                Key = NotificationConstants.TemplateKeys.DocumentUploaded,
                Title = "New Document Uploaded — {DocumentName}",
                Body = "{UploaderName} uploaded {DocumentName} to {EngagementName}.",
                DefaultPriority = NotificationPriority.Medium,
                LinkPattern = "/engagements/{EngagementId}/workspace/documents",
                CreatedAt = seedTimestamp,
                UpdatedAt = seedTimestamp,
                CreatedBy = "System",
                IsDeleted = false
            }
        );
    }
}
