using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pdr.Api.Models.Handover;

namespace Pdr.Api.Data.Configuration;

public class ClientContactTransitionConfiguration : IEntityTypeConfiguration<ClientContactTransition>
{
    public void Configure(EntityTypeBuilder<ClientContactTransition> builder)
    {
        builder.ToTable("client_contact_transitions");

        builder.HasKey(c => c.ClientContactTransitionId);
        builder.Property(c => c.ClientContactTransitionId).HasColumnName("client_contact_transition_id").UseIdentityAlwaysColumn();

        builder.Property(c => c.EngagementId).HasColumnName("engagement_id").IsRequired();
        builder.Property(c => c.OutgoingContactName).HasColumnName("outgoing_contact_name").HasMaxLength(200).IsRequired();
        builder.Property(c => c.OutgoingContactRole).HasColumnName("outgoing_contact_role").HasMaxLength(200).IsRequired();
        builder.Property(c => c.IncomingContactName).HasColumnName("incoming_contact_name").HasMaxLength(200).IsRequired();
        builder.Property(c => c.IncomingContactRole).HasColumnName("incoming_contact_role").HasMaxLength(200).IsRequired();

        // BaseAuditableEntity Column Mappings
        builder.Property(c => c.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
        builder.Property(c => c.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("NOW()");
        builder.Property(c => c.CreatedBy).HasColumnName("created_by").HasMaxLength(255);
        builder.Property(c => c.UpdatedBy).HasColumnName("updated_by").HasMaxLength(255);
        builder.Property(c => c.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);

        // Multiple transitions per engagement allowed (show full history)
        builder.HasIndex(c => c.EngagementId);

        // Global Query Filter for Soft Delete
        builder.HasQueryFilter(c => !c.IsDeleted);

        // Foreign Key Relationship
        builder.HasOne(c => c.Engagement)
            .WithMany()
            .HasForeignKey(c => c.EngagementId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
