namespace Pdr.Api.Data.Configuration;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pdr.Api.Models.EngagementAssignment;

public class EngagementAssignmentConfiguration : IEntityTypeConfiguration<EngagementAssignment>
{
    public void Configure(EntityTypeBuilder<EngagementAssignment> builder)
    {
        builder.ToTable("engagement_assignments");

        builder.HasKey(ea => new { ea.UserId, ea.EngagementId });

        builder.Property(ea => ea.UserId).HasColumnName("user_id");
        builder.Property(ea => ea.EngagementId).HasColumnName("engagement_id");

        builder.HasOne(ea => ea.User)
               .WithMany(u => u.EngagementAssignments)
               .HasForeignKey(ea => ea.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ea => ea.Engagement)
               .WithMany(e => e.EngagementAssignments)
               .HasForeignKey(ea => ea.EngagementId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
