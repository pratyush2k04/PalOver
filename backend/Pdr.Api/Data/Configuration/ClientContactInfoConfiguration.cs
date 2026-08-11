using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pdr.Api.Models;
using Pdr.Api.Models.Handover;

namespace Pdr.Api.Data.Configuration;

public class ClientContactInfoConfiguration : IEntityTypeConfiguration<ClientContactInfo>
{
    private static readonly DateTime SeedTimestamp = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public void Configure(EntityTypeBuilder<ClientContactInfo> builder)
    {
        builder.ToTable("client_contact_infos");

        builder.HasKey(c => c.ClientContactInfoId);
        builder.Property(c => c.ClientContactInfoId).HasColumnName("client_contact_info_id").UseIdentityAlwaysColumn();

        builder.Property(c => c.EngagementId).HasColumnName("engagement_id").IsRequired();
        builder.Property(c => c.Name).HasColumnName("name").HasMaxLength(200).IsRequired();
        builder.Property(c => c.Role).HasColumnName("role").HasMaxLength(100).IsRequired();

        // BaseAuditableEntity Column Mappings
        builder.Property(c => c.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
        builder.Property(c => c.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("NOW()");
        builder.Property(c => c.CreatedBy).HasColumnName("created_by").HasMaxLength(255);
        builder.Property(c => c.UpdatedBy).HasColumnName("updated_by").HasMaxLength(255);
        builder.Property(c => c.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);

        builder.HasQueryFilter(c => !c.IsDeleted);

        builder.HasOne(c => c.Engagement)
            .WithMany()
            .HasForeignKey(c => c.EngagementId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasData(
            // Engagement 1 contacts
            new ClientContactInfo
            {
                ClientContactInfoId = 1,
                EngagementId = 1,
                Name = "Sarah Jenkins",
                Role = "Client Executive Sponsor",
                CreatedAt = SeedTimestamp,
                UpdatedAt = SeedTimestamp,
                CreatedBy = "System",
                IsDeleted = false
            },
            new ClientContactInfo
            {
                ClientContactInfoId = 2,
                EngagementId = 1,
                Name = "David Miller",
                Role = "VP of Engineering",
                CreatedAt = SeedTimestamp,
                UpdatedAt = SeedTimestamp,
                CreatedBy = "System",
                IsDeleted = false
            },
            new ClientContactInfo
            {
                ClientContactInfoId = 3,
                EngagementId = 1,
                Name = "Emma Watson",
                Role = "Product Owner",
                CreatedAt = SeedTimestamp,
                UpdatedAt = SeedTimestamp,
                CreatedBy = "System",
                IsDeleted = false
            },
            new ClientContactInfo
            {
                ClientContactInfoId = 4,
                EngagementId = 1,
                Name = "Michael Brown",
                Role = "Security Lead",
                CreatedAt = SeedTimestamp,
                UpdatedAt = SeedTimestamp,
                CreatedBy = "System",
                IsDeleted = false
            },
            // Engagement 2 contacts
            new ClientContactInfo
            {
                ClientContactInfoId = 5,
                EngagementId = 2,
                Name = "Robert Chen",
                Role = "VP of Technology",
                CreatedAt = SeedTimestamp,
                UpdatedAt = SeedTimestamp,
                CreatedBy = "System",
                IsDeleted = false
            },
            new ClientContactInfo
            {
                ClientContactInfoId = 6,
                EngagementId = 2,
                Name = "Laura Davis",
                Role = "Chief Information Officer",
                CreatedAt = SeedTimestamp,
                UpdatedAt = SeedTimestamp,
                CreatedBy = "System",
                IsDeleted = false
            },
            new ClientContactInfo
            {
                ClientContactInfoId = 7,
                EngagementId = 2,
                Name = "James Wilson",
                Role = "Director of Operations",
                CreatedAt = SeedTimestamp,
                UpdatedAt = SeedTimestamp,
                CreatedBy = "System",
                IsDeleted = false
            }
        );
    }
}
