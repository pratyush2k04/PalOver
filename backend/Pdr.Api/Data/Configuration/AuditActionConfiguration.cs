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
                Action = "UserLoggedIn",
                Details = "User logged into the system"
            },
            new AuditAction
            {
                AuditActionId = (int)AuditActionEnum.UserCreated,
                Action = "UserCreated",
                Details = "A new user account was created"
            },
            new AuditAction
            {
                AuditActionId = (int)AuditActionEnum.UserUpdated,
                Action = "UserUpdated",
                Details = "A user account was updated"
            },
            new AuditAction
            {
                AuditActionId = (int)AuditActionEnum.UserDeleted,
                Action = "UserDeleted",
                Details = "A user account was deleted"
            });
    }
}
