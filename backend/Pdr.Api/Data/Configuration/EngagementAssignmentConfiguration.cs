namespace Pdr.Api.Data.Configuration;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pdr.Api.Models.EngagementAssignment;

public class EngagementAssignmentConfiguration : IEntityTypeConfiguration<EngagementAssignment>
{
    public void Configure(EntityTypeBuilder<EngagementAssignment> builder)
    {
        builder.ToTable("engagement_assignments");

        builder.HasKey(ea => new { ea.UserId, ea.EngagementId, ea.EngagementRoleId });

        builder.Property(ea => ea.UserId).HasColumnName("user_id");
        builder.Property(ea => ea.EngagementId).HasColumnName("engagement_id");
        builder.Property(ea => ea.EngagementRoleId).HasColumnName("engagement_role_id");

        builder.HasOne(ea => ea.User)
               .WithMany(u => u.EngagementAssignments)
               .HasForeignKey(ea => ea.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ea => ea.Engagement)
               .WithMany(e => e.EngagementAssignments)
               .HasForeignKey(ea => ea.EngagementId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ea => ea.EngagementRole)
               .WithMany(er => er.EngagementAssignments)
               .HasForeignKey(ea => ea.EngagementRoleId)
               .OnDelete(DeleteBehavior.Restrict);

        // Matching filter: User, Engagement, and EngagementRole are required ends with their own
        // soft-delete query filters, so mirror it here to avoid EF's dangling-required-navigation warning.
        builder.HasQueryFilter(ea => !ea.User.IsDeleted && !ea.Engagement.IsDeleted && !ea.EngagementRole.IsDeleted);
    }
}
