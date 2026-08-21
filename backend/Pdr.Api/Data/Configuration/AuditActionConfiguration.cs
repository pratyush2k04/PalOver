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
                Id = (int)AuditActionEnum.DocumentVersioned,
                Action = AuditActionEnum.DocumentVersioned.ToString(),
                Details = "Document {documentName} was versioned to {version} by {actorName}"
            },
            new AuditAction
            {
                Id = (int)AuditActionEnum.DocumentUploaded,
                Action = AuditActionEnum.DocumentUploaded.ToString(),
                Details = "Document {documentName} uploaded under category {categoryName} by {actorName}"
            },
            new AuditAction
            {
                Id = (int)AuditActionEnum.UnauthorizedAccess,
                Action = AuditActionEnum.UnauthorizedAccess.ToString(),
                Details = "Unauthorized access attempt for email {email}"
            },
            new AuditAction
            {
                Id = (int)AuditActionEnum.AccountLockout,
                Action = AuditActionEnum.AccountLockout.ToString(),
                Details = "Account locked out for user {userName}"
            },
            new AuditAction
            {
                Id = (int)AuditActionEnum.ClientContactDetailsChanged,
                Action = AuditActionEnum.ClientContactDetailsChanged.ToString(),
                Details = "Contact details changed for client {clientName} by {actorName}"
            },
            new AuditAction
            {
                Id = (int)AuditActionEnum.ClientPersonaChanged,
                Action = AuditActionEnum.ClientPersonaChanged.ToString(),
                Details = "Client persona changed for {clientName} by {actorName}"
            },
            new AuditAction
            {
                Id = (int)AuditActionEnum.LeadershipAllocated,
                Action = AuditActionEnum.LeadershipAllocated.ToString(),
                Details = "Leadership allocated for engagement {engagementName} by {actorName}"
            },
            new AuditAction
            {
                Id = (int)AuditActionEnum.UserRoleAllocated,
                Action = AuditActionEnum.UserRoleAllocated.ToString(),
                Details = "Role {roleName} allocated to user {targetUserName} by {actorName}"
            },
            new AuditAction
            {
                Id = (int)AuditActionEnum.PmAndTlAssigned,
                Action = AuditActionEnum.PmAndTlAssigned.ToString(),
                Details = "PM and TL assigned for engagement {engagementName} by {actorName}"
            },
            new AuditAction
            {
                Id = (int)AuditActionEnum.PmAndTlReassigned,
                Action = AuditActionEnum.PmAndTlReassigned.ToString(),
                Details = "PM and TL reassigned for engagement {engagementName} by {actorName}"
            },
            new AuditAction
            {
                Id = (int)AuditActionEnum.DayOneSummaryGenerated,
                Action = AuditActionEnum.DayOneSummaryGenerated.ToString(),
                Details = "Day-One Summary generated for engagement {engagementName} by {actorName}"
            },
            new AuditAction
            {
                Id = (int)AuditActionEnum.MasterChecklistPublished,
                Action = AuditActionEnum.MasterChecklistPublished.ToString(),
                Details = "Master checklist version {version} published by {actorName}"
            },
            new AuditAction
            {
                Id = (int)AuditActionEnum.BrownfieldItemAdded,
                Action = AuditActionEnum.BrownfieldItemAdded.ToString(),
                Details = "Brownfield finding {findingName} added to engagement {engagementName} by {actorName}"
            },
            new AuditAction
            {
                Id = (int)AuditActionEnum.DiscoveryItemAdded,
                Action = AuditActionEnum.DiscoveryItemAdded.ToString(),
                Details = "Discovery item {itemName} added to engagement {engagementName} by {actorName}"
            },
            new AuditAction
            {
                Id = (int)AuditActionEnum.ResourcingGapFlagRaised,
                Action = AuditActionEnum.ResourcingGapFlagRaised.ToString(),
                Details = "Resourcing gap flag raised for engagement {engagementName}: {unconfirmedCount} request(s) open, {daysToBilling} day(s) until billing"
            },
            new AuditAction
            {
                Id = (int)AuditActionEnum.ResourcingGapFlagCleared,
                Action = AuditActionEnum.ResourcingGapFlagCleared.ToString(),
                Details = "Resourcing gap flag cleared for engagement {engagementName}: all requested resources confirmed"
            },
            new AuditAction
            {
                Id = (int)AuditActionEnum.ChecklistItemEvidenceUpdated,
                Action = AuditActionEnum.ChecklistItemEvidenceUpdated.ToString(),
                Details = "Evidence updated for checklist item '{itemTitle}' on engagement '{engagementName}' by {actorName}"
            },
            new AuditAction
            {
                Id = (int)AuditActionEnum.EvidenceSnapshotCreated,
                Action = AuditActionEnum.EvidenceSnapshotCreated.ToString(),
                Details = "Evidence snapshot captured for engagement '{engagementName}' at Day-1 sign-off by {actorName}"
            });
    }
}
