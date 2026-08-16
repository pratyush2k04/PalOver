using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pdr.Api.Models.Audit;

namespace Pdr.Api.Data.Configuration;

public class AuditActionConfiguration : IEntityTypeConfiguration<AuditAction>
{
    public void Configure(EntityTypeBuilder<AuditAction> builder)
    {
        builder.ToTable("audit_actions");

        builder.HasKey(x => x.AuditActionId);

        builder.Property(x => x.AuditActionId)
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
                AuditActionId = (int)AuditActionEnum.UserLoggedIn,
                Action = AuditActionEnum.UserLoggedIn.ToString(),
                Details = "User {userName} logged into the system"
            },
            new AuditAction
            {
                AuditActionId = (int)AuditActionEnum.UserCreated,
                Action = AuditActionEnum.UserCreated.ToString(),
                Details = "User {userName} was created by {actorName}"
            },
            new AuditAction
            {
                AuditActionId = (int)AuditActionEnum.UserUpdated,
                Action = AuditActionEnum.UserUpdated.ToString(),
                Details = "User {userName} was updated by {actorName}"
            },
            new AuditAction
            {
                AuditActionId = (int)AuditActionEnum.UserDeleted,
                Action = AuditActionEnum.UserDeleted.ToString(),
                Details = "User {userName} was deleted by {actorName}"
            });
    }
}
