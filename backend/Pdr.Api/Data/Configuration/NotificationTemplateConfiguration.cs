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
                Id = 4,
                Key = NotificationConstants.TemplateKeys.ResourceRequestRaised,
                Title = "Resource Request Raised — {EngagementName}",
                Body = "{PeopleNeeded} {RoleName} requested for {EngagementName} by {RequiredByDate}.",
                DefaultPriority = NotificationPriority.High,
                LinkPattern = "/engagements/{EngagementId}/workspace/resources",
                CreatedAt = seedTimestamp,
                UpdatedAt = seedTimestamp,
                CreatedBy = "System",
                IsDeleted = false
            },
            new NotificationTemplate
            {
                Id = 15,
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
            },
            new NotificationTemplate
            {
                Id = 16,
                Key = NotificationConstants.TemplateKeys.BlockerEscalatedPM,
                Title = "Escalation: {EngagementName}",
                Body = "{ItemTitle} blocked {UnresolvedDays} days — now at PM level.",
                DefaultPriority = NotificationPriority.High,
                LinkPattern = "/engagements/{EngagementId}/workspace/risks",
                CreatedAt = seedTimestamp,
                UpdatedAt = seedTimestamp,
                CreatedBy = "System",
                IsDeleted = false
            },
            new NotificationTemplate
            {
                Id = 17,
                Key = NotificationConstants.TemplateKeys.BlockerEscalatedLeadership,
                Title = "Escalation: {EngagementName}",
                Body = "{ItemTitle} blocked {UnresolvedDays} days — now at Leadership level.",
                DefaultPriority = NotificationPriority.High,
                LinkPattern = "/engagements/{EngagementId}/workspace/risks",
                CreatedAt = seedTimestamp,
                UpdatedAt = seedTimestamp,
                CreatedBy = "System",
                IsDeleted = false
            },
            new NotificationTemplate
            {
                Id = 5,
                Key = "ResourceRequestConfirmed",
                Title = "Resource Request Confirmed — {EngagementName}",
                Body = "All named resources for {RoleName} on {EngagementName} are now confirmed.",
                DefaultPriority = NotificationPriority.High,
                LinkPattern = "/engagements/{EngagementId}/workspace/resources",
                CreatedAt = seedTimestamp,
                UpdatedAt = seedTimestamp,
                CreatedBy = "System",
                IsDeleted = false
            },
            new NotificationTemplate
            {
                Id = 6,
                Key = "ResourceRequestAllocated",
                Title = "Resource Request Allocated — {EngagementName}",
                Body = "Named resources have been allocated for your {RoleName} request on {EngagementName}.",
                DefaultPriority = NotificationPriority.Medium,
                LinkPattern = "/engagements/{EngagementId}/workspace/resources",
                CreatedAt = seedTimestamp,
                UpdatedAt = seedTimestamp,
                CreatedBy = "System",
                IsDeleted = false
            },
            new NotificationTemplate
            {
                Id = 11,
                Key = "ResourceRequestReopened",
                Title = "Resource Request Reopened — {EngagementName}",
                Body = "A confirmed resource was removed from your {RoleName} request on {EngagementName}; it is now reopened for allocation.",
                DefaultPriority = NotificationPriority.High,
                LinkPattern = "/engagements/{EngagementId}/workspace/resources",
                CreatedAt = seedTimestamp,
                UpdatedAt = seedTimestamp,
                CreatedBy = "System",
                IsDeleted = false
            },
            new NotificationTemplate
            {
                Id = 12,
                Key = NotificationConstants.TemplateKeys.ResourcingGapFlagRaised,
                Title = "Resources Not Confirmed — {EngagementName}",
                Body = "{UnconfirmedCount} resource request(s) for {EngagementName} remain unconfirmed with {DaysToBilling} day(s) until billing starts ({BillingDate}).",
                DefaultPriority = NotificationPriority.High,
                LinkPattern = "/engagements/{EngagementId}/workspace/resources",
                CreatedAt = seedTimestamp,
                UpdatedAt = seedTimestamp,
                CreatedBy = "System",
                IsDeleted = false
            },
            new NotificationTemplate
            {
                Id = 18,
                Key = NotificationConstants.TemplateKeys.CommitmentVarianceEscalated,
                Title = "Commitment Variance — {EngagementName}",
                Body = "Commitment '{CommitmentTitle}' timeline variance flagged: {VarianceDescription}.",
                DefaultPriority = NotificationPriority.High,
                LinkPattern = "/engagements/{EngagementId}/workspace/handover",
                CreatedAt = seedTimestamp,
                UpdatedAt = seedTimestamp,
                CreatedBy = "System",
                IsDeleted = false
            },
            new NotificationTemplate
            {
                Id = 23697,
                Key = NotificationConstants.TemplateKeys.HandoverDraftFirstSaved,
                Title = "Allocate leadership for engagement {EngagementName}",
                Body = "{ActorName} started the handover for {EngagementName}.",
                DefaultPriority = NotificationPriority.High,
                LinkPattern = "/engagement-allocation",
                CreatedAt = seedTimestamp,
                UpdatedAt = seedTimestamp,
                CreatedBy = "System",
                IsDeleted = false
            }
        );
    }
}
