using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pdr.Api.Models.Audit;

namespace Pdr.Api.Data.Configuration;

public class AuditActionConfiguration : IEntityTypeConfiguration<AuditAction>
{
    public void Configure(EntityTypeBuilder<AuditAction> builder)
    {
        builder.ToTable("audit_actions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.Action)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Details)
            .IsRequired()
            .HasMaxLength(500);

        builder.HasIndex(x => x.Action)
            .IsUnique();

        builder.HasData(
            new AuditAction
            {
                Id = (int)AuditActionEnum.UserLoggedIn,
                Action = AuditActionEnum.UserLoggedIn.ToString(),
                Details = "User {userName} logged into the system"
            },
            new AuditAction
            {
                Id = (int)AuditActionEnum.UserCreated,
                Action = AuditActionEnum.UserCreated.ToString(),
                Details = "User {userName} was created by {actorName}"
            },
            new AuditAction
            {
                Id = (int)AuditActionEnum.UserUpdated,
                Action = AuditActionEnum.UserUpdated.ToString(),
                Details = "User {userName} was updated by {actorName}"
            },
            new AuditAction
            {
                Id = (int)AuditActionEnum.UserDeleted,
                Action = AuditActionEnum.UserDeleted.ToString(),
                Details = "User {userName} was deleted by {actorName}"
            },
            new AuditAction
            {
                Id = (int)AuditActionEnum.KickoffReminderScheduled,
                Action = AuditActionEnum.KickoffReminderScheduled.ToString(),
                Details = "Kickoff readiness reminder scheduled for engagement {engagementName} ({thresholdLabel} before kickoff)"
            },
            new AuditAction
            {
                Id = (int)AuditActionEnum.KickoffReminderFired,
                Action = AuditActionEnum.KickoffReminderFired.ToString(),
                Details = "A {thresholdLabel}-before-kickoff readiness reminder was sent for engagement {engagementName}"
            },
            new AuditAction
            {
                Id = (int)AuditActionEnum.KickoffReminderSuppressed,
                Action = AuditActionEnum.KickoffReminderSuppressed.ToString(),
                Details = "Kickoff reminders for engagement {engagementName} ({thresholdsLabel} before kickoff on {kickoffDate}) were suppressed after readiness reached 100%"
            },
            new AuditAction
            {
                Id = (int)AuditActionEnum.KickoffReminderReactivated,
                Action = AuditActionEnum.KickoffReminderReactivated.ToString(),
                Details = "Kickoff reminders for engagement {engagementName} ({thresholdsLabel} before kickoff on {kickoffDate}) were reactivated after readiness dropped to {readinessScore}%"
            },
            new AuditAction
            {
                Id = (int)AuditActionEnum.KickoffReminderRetiredForBilling,
                Action = AuditActionEnum.KickoffReminderRetiredForBilling.ToString(),
                Details = "Kickoff reminders for engagement {engagementName} were retired after billing was signed off"
            });
    }
}
