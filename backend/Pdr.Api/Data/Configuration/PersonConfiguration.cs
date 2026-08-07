using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pdr.Api.Models;

namespace Pdr.Api.Data.Configuration;

public class PersonConfiguration : IEntityTypeConfiguration<Person>
{
    private static readonly DateTime SeedTimestamp = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public void Configure(EntityTypeBuilder<Person> builder)
    {
        builder.ToTable("people");

        builder.HasKey(p => p.PersonId);
        builder.Property(p => p.PersonId).HasColumnName("person_id").UseIdentityAlwaysColumn();

        builder.Property(p => p.EngagementId).HasColumnName("engagement_id").IsRequired();
        builder.Property(p => p.Name).HasColumnName("name").HasMaxLength(200).IsRequired();
        builder.Property(p => p.Role).HasColumnName("role").HasMaxLength(100).IsRequired();
        builder.Property(p => p.PersonType)
            .HasColumnName("person_type")
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        // BaseAuditableEntity Column Mappings
        builder.Property(p => p.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
        builder.Property(p => p.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("NOW()");
        builder.Property(p => p.CreatedBy).HasColumnName("created_by").HasMaxLength(255);
        builder.Property(p => p.UpdatedBy).HasColumnName("updated_by").HasMaxLength(255);
        builder.Property(p => p.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);

        // Global Query Filter for Soft Delete
        builder.HasQueryFilter(p => !p.IsDeleted);

        // Foreign Key Relationship
        builder.HasOne(p => p.Engagement)
            .WithMany(e => e.People)
            .HasForeignKey(p => p.EngagementId)
            .OnDelete(DeleteBehavior.Cascade);

        // Data Seeding
        builder.HasData(
            new Person
            {
                PersonId = 1,
                EngagementId = 1,
                Name = "Sarah Jenkins",
                Role = "Client Executive Sponsor",
                PersonType = PersonType.Client,
                CreatedAt = SeedTimestamp,
                UpdatedAt = SeedTimestamp,
                CreatedBy = "System",
                IsDeleted = false
            },
            new Person
            {
                PersonId = 2,
                EngagementId = 1,
                Name = "Alex Rivera",
                Role = "Internal Technical Lead",
                PersonType = PersonType.Internal,
                CreatedAt = SeedTimestamp,
                UpdatedAt = SeedTimestamp,
                CreatedBy = "System",
                IsDeleted = false
            }
        );
    }
}
