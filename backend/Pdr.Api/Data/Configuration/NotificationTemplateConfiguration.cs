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
            },
            new NotificationTemplate
            {
                Id = 96837,
                Key = "KickoffReadinessReminder",
                Title = "{DaysBeforeKickoff} days until kickoff — {EngagementName} readiness incomplete",
                Body = "{EngagementName} still has open mandatory items with kickoff in {DaysBeforeKickoff} days.",
                DefaultPriority = NotificationPriority.Medium,
                LinkPattern = "/engagements/{EngagementId}/workspace/{TabKey}",
                CreatedAt = seedTimestamp,
                UpdatedAt = seedTimestamp,
                CreatedBy = "System",
                IsDeleted = false
            },
            new NotificationTemplate
            {
                Id = 19,
                Key = NotificationConstants.TemplateKeys.LeadershipActionRequired,
                Title = "Action Required: Assign PM & TL — {EngagementName}",
                Body = "You have been assigned as Engagement Leader for {EngagementName}. Please assign a Project Manager and Tech Lead to the engagement.",
                DefaultPriority = NotificationPriority.High,
                LinkPattern = "/engagements/{EngagementId}/workspace/overview",
                CreatedAt = seedTimestamp,
                UpdatedAt = seedTimestamp,
                CreatedBy = "System",
                IsDeleted = false
            },
            new NotificationTemplate
            {
                Id = 20,
                Key = NotificationConstants.TemplateKeys.PresalesCommitmentAdded,
                Title = "New Presales Commitment — {EngagementName}",
                Body = "Presales added commitment '{CommitmentTitle}' for {EngagementName}. Please review and add realistic commitments.",
                DefaultPriority = NotificationPriority.High,
                LinkPattern = "/engagements/{EngagementId}/workspace/handover",
                CreatedAt = seedTimestamp,
                UpdatedAt = seedTimestamp,
                CreatedBy = "System",
                IsDeleted = false
            },
            new NotificationTemplate
            {
                Id = 21,
                Key = NotificationConstants.TemplateKeys.PendingCommitmentsSummary,
                Title = "Action Required: {Count} commitments require your review",
                Body = "There are {Count} presales commitments on engagement {EngagementName} that need realistic estimates.",
                DefaultPriority = NotificationPriority.High,
                LinkPattern = "/engagements/{EngagementId}/workspace/handover",
                CreatedAt = seedTimestamp,
                UpdatedAt = seedTimestamp,
                CreatedBy = "System",
                IsDeleted = false
            },
            new NotificationTemplate
            {
                Id = 22,
                Key = NotificationConstants.TemplateKeys.TechnicalReadinessComplete,
                Title = "Technical Readiness 100% — {EngagementName}",
                Body = "Technical readiness has reached 100% for {EngagementName}. You can now proceed with Day 1 sign-off.",
                DefaultPriority = NotificationPriority.High,
                LinkPattern = "/engagements/{EngagementId}/workspace/overview",
                CreatedAt = seedTimestamp,
                UpdatedAt = seedTimestamp,
                CreatedBy = "System",
                IsDeleted = false
            },
            new NotificationTemplate
            {
                Id = 23,
                Key = NotificationConstants.TemplateKeys.TechnicalReadinessRegressed,
                Title = "Technical Readiness Dropped Below 100% — {EngagementName}",
                Body = "Technical readiness for {EngagementName} has dropped below 100% ({Reason}). Day 1 sign-off has been disabled until all technical blockers are resolved.",
                DefaultPriority = NotificationPriority.High,
                LinkPattern = "/engagements/{EngagementId}/workspace/technical-readiness",
                CreatedAt = seedTimestamp,
                UpdatedAt = seedTimestamp,
                CreatedBy = "System",
                IsDeleted = false
            },
            new NotificationTemplate
            {
                Id = 471928,
                Key = NotificationConstants.TemplateKeys.DocumentDeleted,
                Title = "Document removed — {DocumentName}",
                Body = "{ActorName} removed {DocumentName} from {EngagementName}.",
                DefaultPriority = NotificationPriority.Medium,
                LinkPattern = "/engagements/{EngagementId}/workspace/documents",
                CreatedAt = seedTimestamp,
                UpdatedAt = seedTimestamp,
                CreatedBy = "System",
                IsDeleted = false
            },
            new NotificationTemplate
            {
                Id = 583046,
                Key = NotificationConstants.TemplateKeys.DocumentVersionUploaded,
                Title = "New version uploaded — {DocumentName}",
                Body = "{UploaderName} uploaded version {Version} of {DocumentName} to {EngagementName}.",
                DefaultPriority = NotificationPriority.Medium,
                LinkPattern = "/engagements/{EngagementId}/workspace/documents",
                CreatedAt = seedTimestamp,
                UpdatedAt = seedTimestamp,
                CreatedBy = "System",
                IsDeleted = false
            },
            new NotificationTemplate
            {
                Id = 692157,
                Key = NotificationConstants.TemplateKeys.DocumentVisibilityChanged,
                Title = "Document visibility changed — {DocumentName}",
                Body = "{ActorName} changed {DocumentName}'s visibility to {Visibility} in {EngagementName}.",
                DefaultPriority = NotificationPriority.Low,
                LinkPattern = "/engagements/{EngagementId}/workspace/documents",
                CreatedAt = seedTimestamp,
                UpdatedAt = seedTimestamp,
                CreatedBy = "System",
                IsDeleted = false
            },
            new NotificationTemplate
            {
                Id = 856017,
                Key = "HandoverQueryRaised",
                Title = "Handover Query Raised — {EngagementName}",
                Body = "{TriggerUserName} raised a query on {FieldSection} for {EngagementName}.",
                DefaultPriority = NotificationPriority.High,
                LinkPattern = "/engagements/{EngagementId}/workspace/handover",
                CreatedAt = seedTimestamp,
                UpdatedAt = seedTimestamp,
                CreatedBy = "System",
                IsDeleted = false
            },
            new NotificationTemplate
            {
                Id = 918273,
                Key = NotificationConstants.TemplateKeys.KickoffActionItemAssigned,
                Title = "Action Item Assigned — {EngagementName}",
                Body = "You have been assigned the kickoff action \"{ActionTitle}\" for {EngagementName}.",
                DefaultPriority = NotificationPriority.Low,
                LinkPattern = "/engagements/{EngagementId}/workspace/kickoff",
                CreatedAt = seedTimestamp,
                UpdatedAt = seedTimestamp,
                CreatedBy = "System",
                IsDeleted = false
            },
            new NotificationTemplate
            {
                Id = 629481,
                Key = NotificationConstants.TemplateKeys.AssumptionNeedsValidation,
                Title = "Assumption Needs Validation — {EngagementName}",
                Body = "Assumption \"{AssumptionDescription}\" needs to be validated for {EngagementName}.",
                DefaultPriority = NotificationPriority.Medium,
                LinkPattern = "/engagements/{EngagementId}/workspace/assumptions-and-decisions",
                CreatedAt = seedTimestamp,
                UpdatedAt = seedTimestamp,
                CreatedBy = "System",
                IsDeleted = false
            },
            new NotificationTemplate
            {
                Id = 742619,
                Key = NotificationConstants.TemplateKeys.DependencyAssignedToOwner,
                Title = "Dependency Assigned — {EngagementName}",
                Body = "You have been assigned the dependency \"{DependencyDescription}\" for {EngagementName}.",
                DefaultPriority = NotificationPriority.Medium,
                LinkPattern = "/engagements/{EngagementId}/workspace/assumptions-and-decisions",
                CreatedAt = seedTimestamp,
                UpdatedAt = seedTimestamp,
                CreatedBy = "System",
                IsDeleted = false
            },
            new NotificationTemplate
            {
                Id = 853720,
                Key = NotificationConstants.TemplateKeys.DependencyAssignedExternal,
                Title = "External Dependency Added — {EngagementName}",
                Body = "Dependency \"{DependencyDescription}\" was assigned to external owner {OwnerName} for {EngagementName}.",
                DefaultPriority = NotificationPriority.Medium,
                LinkPattern = "/engagements/{EngagementId}/workspace/assumptions-and-decisions",
                CreatedAt = seedTimestamp,
                UpdatedAt = seedTimestamp,
                CreatedBy = "System",
                IsDeleted = false
            }
        );
    }
}
