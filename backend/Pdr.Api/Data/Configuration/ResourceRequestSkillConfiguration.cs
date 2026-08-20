using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pdr.Api.Models.ResourceRequests;

namespace Pdr.Api.Data.Configuration;

public class ResourceRequestSkillConfiguration : IEntityTypeConfiguration<ResourceRequestSkill>
{
    public void Configure(EntityTypeBuilder<ResourceRequestSkill> builder)
    {
        builder.ToTable("resource_request_skills");

        builder.HasKey(s => new { s.ResourceRequestId, s.Skill });

        builder.Property(s => s.ResourceRequestId).HasColumnName("resource_request_id");
        builder.Property(s => s.Skill).HasColumnName("skill");

        builder.HasOne(s => s.ResourceRequest)
            .WithMany(r => r.RequestSkills)
            .HasForeignKey(s => s.ResourceRequestId)
            .OnDelete(DeleteBehavior.Cascade);

        // Mirrors ResourceRequest's own soft-delete filter (required end of the relationship),
        // matching the pattern in EngagementAssignmentConfiguration.
        builder.HasQueryFilter(s => !s.ResourceRequest.IsDeleted);
    }
}
