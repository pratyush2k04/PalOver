using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Pdr.Api.Data.Configuration;
using Pdr.Api.Models;
using Pdr.Api.Models.LoginEntity;               // Resolves 'Role' and 'User'
using Pdr.Api.Models.EngagementAssignment;       // Resolves 'EngagementAssignment'
using Pdr.Api.Models.MasterChecklist;

using Pdr.Api.Models.Documents;
//using System.Reflection;

namespace Pdr.Api.Data;

public class AppDbContext : DbContext
{
    public DbSet<Engagement> Engagements => Set<Engagement>();
    public DbSet<EngagementStatus> EngagementStatuses => Set<EngagementStatus>();
    public DbSet<EngagementType> EngagementTypes => Set<EngagementType>();
    private readonly IHttpContextAccessor? _httpContextAccessor;

    public AppDbContext(
        DbContextOptions<AppDbContext> options,
        IHttpContextAccessor? httpContextAccessor = null) : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public DbSet<Role> Roles => Set<Role>();
    public DbSet<User> Users => Set<User>();
    public DbSet<EngagementAssignment> EngagementAssignments => Set<EngagementAssignment>();

    // Master Checklist
    public DbSet<ChecklistCategory> ChecklistCategories => Set<ChecklistCategory>();
    public DbSet<ChecklistStatus> ChecklistStatuses => Set<ChecklistStatus>();
    public DbSet<ChecklistVersion> ChecklistVersions => Set<ChecklistVersion>();
    public DbSet<ChecklistItem> ChecklistItems => Set<ChecklistItem>();

    // Documents
    public DbSet<DocumentCategory> DocumentCategories => Set<DocumentCategory>();
    public DbSet<Document> Documents => Set<Document>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply Configuration Classes from Assembly
        modelBuilder.ApplyConfiguration(new RoleConfiguration());
        modelBuilder.ApplyConfiguration(new EngagementAssignmentConfiguration());
        modelBuilder.ApplyConfiguration(new ChecklistCategoryConfiguration());
        modelBuilder.ApplyConfiguration(new ChecklistStatusConfiguration());
        modelBuilder.ApplyConfiguration(new DocumentCategoryConfiguration());
        modelBuilder.ApplyConfiguration(new DocumentConfiguration());

        // Global Query Filters (Soft Delete)
        modelBuilder.Entity<User>().HasQueryFilter(u => !u.IsDeleted);
        modelBuilder.Entity<Role>().HasQueryFilter(r => !r.IsDeleted);
        modelBuilder.Entity<EngagementAssignment>().HasQueryFilter(ea => !ea.User.IsDeleted);

        // ChecklistVersion
        modelBuilder.Entity<ChecklistVersion>(entity =>
        {
            entity.ToTable("checklist_versions");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").UseIdentityAlwaysColumn();
            entity.Property(e => e.VersionNumber).HasColumnName("version_number").IsRequired();
            entity.Property(e => e.StatusId).HasColumnName("status_id").IsRequired();

            // BaseAuditableEntity Column Mappings
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("NOW()");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by").HasMaxLength(255);
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by").HasMaxLength(255);
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);

            // Global Query Filter for Soft Delete
            entity.HasQueryFilter(e => !e.IsDeleted);

            entity.HasOne(d => d.Status)
                .WithMany()
                .HasForeignKey(d => d.StatusId);
        });

        // ChecklistItem
        modelBuilder.Entity<ChecklistItem>(entity =>
        {
            entity.ToTable("checklist_items");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").UseIdentityAlwaysColumn();
            entity.Property(e => e.VersionId).HasColumnName("version_id").IsRequired();
            entity.Property(e => e.Title).HasColumnName("title").HasMaxLength(255).IsRequired();
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.CategoryId).HasColumnName("category_id").IsRequired();
            entity.Property(e => e.IsMandatory).HasColumnName("is_mandatory").HasDefaultValue(true);
            entity.Property(e => e.DisplayOrder).HasColumnName("display_order").IsRequired();
            entity.Property(e => e.IsRetired).HasColumnName("is_retired").HasDefaultValue(false);

            // BaseAuditableEntity Column Mappings
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("NOW()");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by").HasMaxLength(255);
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by").HasMaxLength(255);
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);

            // Global Query Filter for Soft Delete
            entity.HasQueryFilter(e => !e.IsDeleted);

            entity.HasOne(d => d.Version)
                .WithMany(p => p.Items)
                .HasForeignKey(d => d.VersionId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.Category)
                .WithMany()
                .HasForeignKey(d => d.CategoryId);
        });

        // User Configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Id).HasColumnName("id");
            entity.Property(u => u.FullName).HasColumnName("full_name").HasMaxLength(255).IsRequired();
            entity.Property(u => u.Email).HasColumnName("email").HasMaxLength(255).IsRequired();

            // Performance Indexes
            entity.HasIndex(u => u.Email, "idx_users_email").IsUnique();
            entity.HasIndex(u => u.RoleId, "idx_users_role_id");

            entity.Property(u => u.Password).HasColumnName("password").HasMaxLength(255).IsRequired();
            entity.Property(u => u.RoleId).HasColumnName("role_id").IsRequired();
            entity.Property(u => u.IsAdmin).HasColumnName("is_admin").IsRequired();
            entity.Property(u => u.FailedAttemptCount).HasColumnName("failed_attempt_count").HasDefaultValue(0);
            entity.Property(u => u.LockoutEnd).HasColumnName("lockout_end");

            // BaseAuditableEntity Column Mappings
            entity.Property(u => u.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);
            entity.Property(u => u.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
            entity.Property(u => u.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("NOW()");
            entity.Property(u => u.CreatedBy).HasColumnName("created_by").HasMaxLength(100);
            entity.Property(u => u.UpdatedBy).HasColumnName("updated_by").HasMaxLength(100);

            entity.HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            // Fixed baseline date for seed data
            var seededAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            // Seed System Users with static values
            entity.HasData(
                new User
                {
                    Id = 1,
                    FullName = "Rahul Verma",
                    Email = "presales@company.com",
                    Password = "Password@123",
                    RoleId = 1,
                    IsAdmin = false,
                    IsDeleted = false,
                    CreatedBy = "System",
                    CreatedAt = seededAt,
                    UpdatedAt = seededAt
                },
                new User
                {
                    Id = 2,
                    FullName = "Asha Rao",
                    Email = "pm@company.com",
                    Password = "Password@123",
                    RoleId = 2,
                    IsAdmin = false,
                    IsDeleted = false,
                    CreatedBy = "System",
                    CreatedAt = seededAt,
                    UpdatedAt = seededAt
                },
                new User
                {
                    Id = 3,
                    FullName = "Vikram Singh",
                    Email = "techlead@company.com",
                    Password = "Password@123",
                    RoleId = 3,
                    IsAdmin = false,
                    IsDeleted = false,
                    CreatedBy = "System",
                    CreatedAt = seededAt,
                    UpdatedAt = seededAt
                },
                new User
                {
                    Id = 4,
                    FullName = "Neha Sharma",
                    Email = "leadership@company.com",
                    Password = "Password@123",
                    RoleId = 4,
                    IsAdmin = false,
                    IsDeleted = false,
                    CreatedBy = "System",
                    CreatedAt = seededAt,
                    UpdatedAt = seededAt
                },
                new User
                {
                    Id = 5,
                    FullName = "Admin User",
                    Email = "admin@company.com",
                    Password = "Password@123",
                    RoleId = 4,
                    IsAdmin = true,
                    IsDeleted = false,
                    CreatedBy = "System",
                    CreatedAt = seededAt,
                    UpdatedAt = seededAt
                }
            );
        });
        
        modelBuilder.Entity<EngagementStatus>(entity =>
        {
            entity.ToTable("engagement_statuses");
            entity.HasKey(s => s.StatusId);
            entity.Property(s => s.StatusId).HasColumnName("status_id");
            entity.Property(s => s.StatusName).HasColumnName("status_name").HasMaxLength(50).IsRequired();
        });

        modelBuilder.Entity<EngagementType>(entity =>
        {
            entity.ToTable("engagement_types");
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Id).HasColumnName("id");
            entity.Property(t => t.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
        });

        modelBuilder.Entity<Engagement>(entity =>
        {
            entity.ToTable("engagements");
            entity.HasKey(e => e.EngagementId);
            entity.Property(e => e.EngagementId).HasColumnName("engagement_id");
            entity.Property(e => e.ClientName).HasColumnName("client_name").HasMaxLength(200).IsRequired();
            entity.Property(e => e.EngagementDescription).HasColumnName("engagement_description");
            entity.Property(e => e.EngagementTypeId).HasColumnName("engagement_type_id").IsRequired();
            entity.Property(e => e.KickoffDate).HasColumnName("kickoff_date").IsRequired();
            entity.Property(e => e.BillingStartDate).HasColumnName("billing_start_date");
            entity.Property(e => e.StatusId).HasColumnName("status_id");

            // BaseAuditableEntity Column Mappings
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("NOW()");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by").HasMaxLength(255);
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by").HasMaxLength(255);
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);

            // Global Query Filter for Soft Delete
            entity.HasQueryFilter(e => !e.IsDeleted);

            entity.HasOne(e => e.EngagementType)
                  .WithMany()
                  .HasForeignKey(e => e.EngagementTypeId);

            entity.HasOne(e => e.Status)
                  .WithMany(s => s.Engagements)
                  .HasForeignKey(e => e.StatusId);
        });
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateAuditFields();
        return base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        UpdateAuditFields();
        return base.SaveChanges();
    }

    private void UpdateAuditFields()
    {
        var currentUser = _httpContextAccessor?.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                       ?? _httpContextAccessor?.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value
                       ?? "System";

        var entries = ChangeTracker.Entries<BaseAuditableEntity>();

        foreach (var entry in entries)
        {
            var now = DateTime.UtcNow;

            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = now;
                entry.Entity.UpdatedAt = now;
                entry.Entity.CreatedBy ??= currentUser;
                entry.Entity.UpdatedBy ??= currentUser;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = now;
                entry.Entity.UpdatedBy = currentUser;
            }
        }
    }
}
