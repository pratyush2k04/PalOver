using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Pdr.Api.Data.Configuration;
using Pdr.Api.Models;
using Pdr.Api.Models.LoginEntity;
using Pdr.Api.Models.EngagementAssignment;
using Pdr.Api.Models.MasterChecklist;
using Pdr.Api.Models.Handover;
using Pdr.Api.Models.EngagementChecklist;
using Pdr.Api.Models.Documents;
using Pdr.Api.Models.Cadence;
//using System.Reflection;

namespace Pdr.Api.Data;

public class AppDbContext : DbContext
{
    public DbSet<Engagement> Engagements => Set<Engagement>();
    public DbSet<EngagementStatus> EngagementStatuses => Set<EngagementStatus>();
    public DbSet<EngagementType> EngagementTypes => Set<EngagementType>();
    
    // Handover entities
    public DbSet<HandoverInfo> HandoverInfos => Set<HandoverInfo>();
    public DbSet<Commitment> Commitments => Set<Commitment>();
    public DbSet<RealisticCommitment> RealisticCommitments => Set<RealisticCommitment>();
    public DbSet<CommitmentType> CommitmentTypes => Set<CommitmentType>();
    public DbSet<HandoverStatus> HandoverStatuses => Set<HandoverStatus>();
    public DbSet<CommercialModel> CommercialModels => Set<CommercialModel>();
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

    public DbSet<MasterChecklistCategory> MasterChecklistCategories => Set<MasterChecklistCategory>();
    public DbSet<MasterChecklistStatus> MasterChecklistStatuses => Set<MasterChecklistStatus>();
    public DbSet<MasterChecklistVersion> MasterChecklistVersions => Set<MasterChecklistVersion>();
    public DbSet<MasterChecklistItem> MasterChecklistItems => Set<MasterChecklistItem>();

    public DbSet<EngagementChecklist> EngagementChecklists => Set<EngagementChecklist>();
    public DbSet<EngagementChecklistItem> EngagementChecklistItems => Set<EngagementChecklistItem>();

    private static readonly DateTime SeedTimestamp = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    // Documents
    public DbSet<DocumentCategory> DocumentCategories => Set<DocumentCategory>();
    public DbSet<Document> Documents => Set<Document>();
    public DbSet<CommunicationCadence> CommunicationCadences => Set<CommunicationCadence>();
    // Technical Readiness (Access Tracker, Environment & Tooling)
    public DbSet<Models.AccessTracker.AccessItem> AccessItems => Set<Models.AccessTracker.AccessItem>();
    public DbSet<Models.AccessTracker.AccessItemDefinition> AccessItemDefinitions => Set<Models.AccessTracker.AccessItemDefinition>();
    public DbSet<Models.EnvironmentTooling.EnvironmentToolingItem> EnvironmentToolingItems => Set<Models.EnvironmentTooling.EnvironmentToolingItem>();
    public DbSet<Models.EnvironmentTooling.EnvironmentToolingDefinition> EnvironmentToolingDefinitions => Set<Models.EnvironmentTooling.EnvironmentToolingDefinition>();
    public DbSet<Pdr.Api.Models.ChangeRequest.ChangeRequest> ChangeRequests => Set<Pdr.Api.Models.ChangeRequest.ChangeRequest>();

    // People Directory
    public DbSet<Person> People => Set<Person>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply Configuration Classes from Assembly
        modelBuilder.ApplyConfiguration(new RoleConfiguration());
        modelBuilder.ApplyConfiguration(new EngagementAssignmentConfiguration());

        modelBuilder.ApplyConfiguration(new HandoverInfoConfiguration());
        modelBuilder.ApplyConfiguration(new CommitmentConfiguration());
        modelBuilder.ApplyConfiguration(new RealisticCommitmentConfiguration());
        modelBuilder.ApplyConfiguration(new CommitmentTypeConfiguration());
        modelBuilder.ApplyConfiguration(new HandoverStatusConfiguration());

        modelBuilder.ApplyConfiguration(new MasterChecklistCategoryConfiguration());
        modelBuilder.ApplyConfiguration(new MasterChecklistStatusConfiguration());

        modelBuilder.ApplyConfiguration(new DocumentCategoryConfiguration());
        modelBuilder.ApplyConfiguration(new DocumentConfiguration());
        modelBuilder.ApplyConfiguration(new PersonConfiguration());
        modelBuilder.ApplyConfiguration(new CommunicationCadenceConfiguration());

        // Engagement & Engagement Checklist
        modelBuilder.ApplyConfiguration(new EngagementConfiguration());
        modelBuilder.ApplyConfiguration(new EngagementChecklistConfiguration());
        modelBuilder.ApplyConfiguration(new EngagementChecklistItemConfiguration());

        // Technical Readiness Configurations
        // modelBuilder.ApplyConfiguration(new Pdr.Api.Data.Configuration.AccessTracker.AccessItemConfiguration());
        // modelBuilder.ApplyConfiguration(new Pdr.Api.Data.Configuration.AccessTracker.AccessItemDefinitionConfiguration());
        // modelBuilder.ApplyConfiguration(new Pdr.Api.Data.Configuration.EnvironmentTooling.EnvironmentToolingItemConfiguration());
        // modelBuilder.ApplyConfiguration(new Pdr.Api.Data.Configuration.EnvironmentTooling.EnvironmentToolingDefinitionConfiguration());
        modelBuilder.ApplyConfiguration(new AccessItemConfiguration());
        modelBuilder.ApplyConfiguration(new AccessItemDefinitionConfiguration());
        modelBuilder.ApplyConfiguration(new EnvironmentToolingItemConfiguration());
        modelBuilder.ApplyConfiguration(new EnvironmentToolingDefinitionConfiguration());

        //change Request
        modelBuilder.ApplyConfiguration(new ChangeRequestConfiguration());

        // Global Query Filters (Soft Delete)
        modelBuilder.Entity<User>().HasQueryFilter(u => !u.IsDeleted);
        modelBuilder.Entity<Role>().HasQueryFilter(r => !r.IsDeleted);
        modelBuilder.Entity<EngagementAssignment>().HasQueryFilter(ea => !ea.User.IsDeleted);

        // ChecklistVersion
        modelBuilder.Entity<MasterChecklistVersion>(entity =>
        {
            entity.ToTable("master_checklist_versions");
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

            entity.HasData(
                new
                {
                    Id = 1,
                    VersionNumber = 0,
                    StatusId = 2,
                    CreatedAt = SeedTimestamp,
                    UpdatedAt = SeedTimestamp,
                    IsDeleted = false
                },
                new
                {
                    Id = 2,
                    VersionNumber = 1,
                    StatusId = 1,
                    CreatedAt = SeedTimestamp,
                    UpdatedAt = SeedTimestamp,
                    IsDeleted = false
                });
        });

        // ChecklistItem
        modelBuilder.Entity<MasterChecklistItem>(entity =>
        {
            entity.ToTable("master_checklist_items");
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

            entity.HasData(GetMasterChecklistItemSeedData());
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
                    CreatedAt = SeedTimestamp,
                    UpdatedAt = SeedTimestamp
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
                    CreatedAt = SeedTimestamp,
                    UpdatedAt = SeedTimestamp
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
                    CreatedAt = SeedTimestamp,
                    UpdatedAt = SeedTimestamp
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
                    CreatedAt = SeedTimestamp,
                    UpdatedAt = SeedTimestamp
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
                    CreatedAt = SeedTimestamp,
                    UpdatedAt = SeedTimestamp
                }
            );
        });

        modelBuilder.Entity<EngagementStatus>(entity =>
        {
            entity.ToTable("engagement_statuses");
            entity.HasKey(s => s.StatusId);
            entity.Property(s => s.StatusId).HasColumnName("status_id");
            entity.Property(s => s.StatusName).HasColumnName("status_name").HasMaxLength(50).IsRequired();

            var statusSeeds = Enum.GetValues<EngagementStatusEnum>()
                .Select((s, index) => new EngagementStatus { StatusId = index + 1, StatusName = s.ToString() })
                .ToArray();
            entity.HasData(statusSeeds);
        });

        modelBuilder.Entity<EngagementType>(entity =>
        {
            entity.ToTable("engagement_types");
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Id).HasColumnName("id");
            entity.Property(t => t.Name).HasColumnName("name").HasMaxLength(100).IsRequired();

            var typeSeeds = Enum.GetValues<EngagementTypeEnum>()
                .Select((t, index) => new EngagementType { Id = index + 1, Name = t.ToString() })
                .ToArray();
            entity.HasData(typeSeeds);
        });

        modelBuilder.Entity<CommercialModel>(entity =>
        {
            entity.ToTable("commercial_models");
            entity.HasKey(m => m.Id);
            entity.Property(m => m.Id).HasColumnName("id");
            entity.Property(m => m.Name).HasColumnName("name").HasMaxLength(100).IsRequired();

            entity.HasData(
                new { Id = 1, Name = "Time and Material" },
                new { Id = 2, Name = "Fixed" },
                new { Id = 3, Name = "Capped T&M" }
            );
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

    private static object[] GetMasterChecklistItemSeedData()
    {
        var items = new (string Title, string Description, int CategoryId, bool IsMandatory)[]
        {
            ("Sign commercial contract", "Confirm the signed commercial contract is on file before kickoff.", 1, true),
            ("Confirm engagement budget", "Verify the approved budget matches the statement of work.", 1, true),
            ("Confirm billing schedule", "Agree invoicing milestones and payment terms with the client.", 1, true),
            ("Complete sales-to-delivery handover call", "Sales team walks delivery team through client history and expectations.", 2, true),
            ("Share CRM opportunity notes", "Ensure delivery has access to all pre-sales notes and correspondence.", 2, true),
            ("Transfer key client contacts", "Confirm delivery team has contact details for all client stakeholders.", 2, true),
            ("Document engagement scope", "Capture in-scope and out-of-scope items in the kickoff scope document.", 3, true),
            ("Confirm deliverables list", "List and confirm all deliverables committed to the client.", 3, true),
            ("Identify scope assumptions and exclusions", "Record assumptions made during the sales cycle that affect scope.", 3, false),
            ("Set up engagement governance cadence", "Agree recurring status meetings and escalation paths.", 4, true),
            ("Assign engagement steering committee", "Identify decision-makers on both client and delivery sides.", 4, true),
            ("Define change control process", "Agree how scope changes will be raised, approved, and tracked.", 4, true),
            ("Confirm RAID log owner", "Assign an owner to maintain the Risks, Assumptions, Issues, and Dependencies log.", 4, false),
            ("Allocate delivery team members", "Confirm named resources are assigned and available for the engagement.", 5, true),
            ("Confirm resource availability calendar", "Check assigned resources have capacity for the engagement duration.", 5, true),
            ("Identify backup/on-call resources", "Name backup resources in case of absence during the engagement.", 5, false),
            ("Provision environment access", "Request and confirm access to required client systems and environments.", 6, true),
            ("Verify technical prerequisites", "Confirm client infrastructure and tooling meet engagement requirements.", 6, true),
            ("Set up source control and CI/CD access", "Ensure delivery team has access to repositories and pipelines.", 6, true),
            ("Confirm security and compliance requirements", "Review any client-specific security or compliance obligations.", 6, false)
        };

        var seed = new List<object>();
        var versionIds = new[] { 1, 2 };
        var nextId = 1;
        foreach (var versionId in versionIds)
        {
            for (var i = 0; i < items.Length; i++)
            {
                var (title, description, categoryId, isMandatory) = items[i];
                seed.Add(new
                {
                    Id = nextId++,
                    VersionId = versionId,
                    Title = title,
                    Description = description,
                    CategoryId = categoryId,
                    IsMandatory = isMandatory,
                    DisplayOrder = i + 1,
                    IsRetired = false,
                    CreatedAt = SeedTimestamp,
                    UpdatedAt = SeedTimestamp,
                    IsDeleted = false
                });
            }
        }

        return seed.ToArray();
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
