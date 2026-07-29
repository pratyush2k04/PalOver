using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pdr.Api.Models.RoleBasedPermissions;

namespace Pdr.Api.Data.Configuration
{
    public class EngagementAssignmentConfiguration : IEntityTypeConfiguration<EngagementAssignment>
    {
        public void Configure(EntityTypeBuilder<EngagementAssignment> builder)
        {
            builder.ToTable("engagement_assignments");

            // Composite Primary Key
            builder.HasKey(ea => new { ea.UserId, ea.EngagementId });

            builder.Property(ea => ea.UserId)
                   .HasColumnName("user_id");

            builder.Property(ea => ea.EngagementId)
                   .HasColumnName("engagement_id");

            // FK referencing teammate's User entity
            builder.HasOne(ea => ea.User)
                   .WithMany()
                   .HasForeignKey(ea => ea.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
