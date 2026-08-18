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
                Id = (int)AuditActionEnum.HandoverQueryRaised,
                Action = AuditActionEnum.HandoverQueryRaised.ToString(),
                Details = "{actorName} raised a query on {fieldSection} for the handover on {engagementName}"
            },
            new AuditAction
            {
                Id = (int)AuditActionEnum.HandoverQueryResponded,
                Action = AuditActionEnum.HandoverQueryResponded.ToString(),
                Details = "{actorName} responded to a query on {fieldSection} for the handover on {engagementName}"
            },
            new AuditAction
            {
                Id = (int)AuditActionEnum.HandoverQueryResolved,
                Action = AuditActionEnum.HandoverQueryResolved.ToString(),
                Details = "{actorName} marked a query on {fieldSection} resolved for the handover on {engagementName}"
            });
    }
}
