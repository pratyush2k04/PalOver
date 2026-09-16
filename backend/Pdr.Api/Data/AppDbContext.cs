using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Pdr.Api.Models;
using Pdr.Api.Models.Engagements;
using Pdr.Api.Models.LoginEntity;
using Pdr.Api.Models.EngagementAssignment;
using Pdr.Api.Models.MasterChecklist;
using Pdr.Api.Models.Handover;
using Pdr.Api.Models.HandoverReadiness;
using Pdr.Api.Models.EngagementChecklist;
using Pdr.Api.Models.Documents;
using Pdr.Api.Models.Cadence;
using Pdr.Api.Models.Risks;
using Pdr.Api.Models.Kickoff;
using Pdr.Api.Models.People;

namespace Pdr.Api.Data;

public class AppDbContext : DbContext
{
    public DbSet<Engagement> Engagements => Set<Engagement>();
    public DbSet<EngagementStatus> EngagementStatuses => Set<EngagementStatus>();
    public DbSet<EngagementType> EngagementTypes => Set<EngagementType>();
    public DbSet<DayOneSignOffRecord> DayOneSignOffRecords => Set<DayOneSignOffRecord>();
    public DbSet<EvidenceSnapshot> EvidenceSnapshots => Set<EvidenceSnapshot>();
    public DbSet<EvidenceSnapshotItem> EvidenceSnapshotItems => Set<EvidenceSnapshotItem>();

    public DbSet<Assumption> Assumptions => Set<Assumption>();

    // Handover entities
    public DbSet<HandoverInfo> HandoverInfos => Set<HandoverInfo>();
    public DbSet<Commitment> Commitments => Set<Commitment>();
    public DbSet<RealisticCommitment> RealisticCommitments => Set<RealisticCommitment>();
    public DbSet<CommitmentType> CommitmentTypes => Set<CommitmentType>();
    public DbSet<HandoverStatus> HandoverStatuses => Set<HandoverStatus>();
    public DbSet<HandoverQuery> HandoverQueries => Set<HandoverQuery>();
    public DbSet<HandoverQueryMessage> HandoverQueryMessages => Set<HandoverQueryMessage>();
    public DbSet<HandoverQueryFieldSection> HandoverQueryFieldSections => Set<HandoverQueryFieldSection>();
    public DbSet<CommercialModel> CommercialModels => Set<CommercialModel>();
    private readonly IHttpContextAccessor? _httpContextAccessor;

    public AppDbContext(
        DbContextOptions<AppDbContext> options,
        IHttpContextAccessor? httpContextAccessor = null) : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<UserRole> Roles => UserRoles;
    public DbSet<EngagementRole> EngagementRoles => Set<EngagementRole>();
    public DbSet<User> Users => Set<User>();
    public DbSet<EngagementAssignment> EngagementAssignments => Set<EngagementAssignment>();

    public DbSet<MasterChecklistCategory> MasterChecklistCategories => Set<MasterChecklistCategory>();
    public DbSet<MasterChecklistStatus> MasterChecklistStatuses => Set<MasterChecklistStatus>();
    public DbSet<MasterChecklistVersion> MasterChecklistVersions => Set<MasterChecklistVersion>();
    public DbSet<MasterChecklistItem> MasterChecklistItems => Set<MasterChecklistItem>();

    public DbSet<EngagementChecklist> EngagementChecklists => Set<EngagementChecklist>();
    public DbSet<EngagementChecklistItem> EngagementChecklistItems => Set<EngagementChecklistItem>();

    // Documents
    public DbSet<DocumentCategory> DocumentCategories => Set<DocumentCategory>();
    public DbSet<Document> Documents => Set<Document>();
    public DbSet<DocumentVersion> DocumentVersions => Set<DocumentVersion>();
    public DbSet<CommunicationCadence> CommunicationCadences => Set<CommunicationCadence>();
    // People
    public DbSet<Person> People => Set<Person>();

    // Kickoff Records & Action Items
    public DbSet<KickoffRecord> KickoffRecords => Set<KickoffRecord>();
    public DbSet<KickoffActionItem> KickoffActionItems => Set<KickoffActionItem>();

    // Technical Readiness (Access Tracker, Environment & Tooling, Brownfield Review)
    public DbSet<Models.AccessTracker.AccessItem> AccessItems => Set<Models.AccessTracker.AccessItem>();
    public DbSet<Models.AccessTracker.AccessItemDefinition> AccessItemDefinitions => Set<Models.AccessTracker.AccessItemDefinition>();
    public DbSet<Models.EnvironmentTooling.EnvironmentToolingItem> EnvironmentToolingItems => Set<Models.EnvironmentTooling.EnvironmentToolingItem>();
    public DbSet<Models.EnvironmentTooling.EnvironmentToolingDefinition> EnvironmentToolingDefinitions => Set<Models.EnvironmentTooling.EnvironmentToolingDefinition>();
    public DbSet<Models.BrownField.BrownfieldFinding> BrownfieldFindings => Set<Models.BrownField.BrownfieldFinding>();
    public DbSet<Models.ArchitectureValidation.ArchitectureValidation> ArchitectureValidations => Set<Models.ArchitectureValidation.ArchitectureValidation>();
    public DbSet<Models.ArchitectureValidation.DiscoveryFeatureItem> DiscoveryFeatureItems => Set<Models.ArchitectureValidation.DiscoveryFeatureItem>();
    public DbSet<Pdr.Api.Models.ChangeRequest.ChangeRequest> ChangeRequests => Set<Pdr.Api.Models.ChangeRequest.ChangeRequest>();
    public DbSet<Risk> Risks => Set<Risk>();
    public DbSet<RiskEscalationHistory> RiskEscalationHistories => Set<RiskEscalationHistory>();
   
    public DbSet<RiskComment> RiskComments => Set<RiskComment>();


 

    public DbSet<ClientContactTransition> ClientContactTransitions => Set<ClientContactTransition>();
    // Notifications
    public DbSet<Models.Notifications.Notification> Notifications => Set<Models.Notifications.Notification>();
    public DbSet<Models.Notifications.NotificationTemplate> NotificationTemplates => Set<Models.Notifications.NotificationTemplate>();

    // Audit Log
    public DbSet<Models.Audit.AuditAction> AuditActions => Set<Models.Audit.AuditAction>();
    public DbSet<Models.Audit.AuditLog> AuditLogs => Set<Models.Audit.AuditLog>();

    // Resource Requests
    public DbSet<Models.ResourceRequests.ResourceRequest> ResourceRequests => Set<Models.ResourceRequests.ResourceRequest>();
    public DbSet<Models.ResourceRequests.ResourceRequestSkill> ResourceRequestSkills => Set<Models.ResourceRequests.ResourceRequestSkill>();
    public DbSet<Models.ResourceRequests.ResourceAllocation> ResourceAllocations => Set<Models.ResourceRequests.ResourceAllocation>();

    // Dependencies
    public DbSet<Models.Engagements.Dependencies.Dependency> Dependencies => Set<Models.Engagements.Dependencies.Dependency>();

    // Handover Readiness
    public DbSet<HandoverReadinessItem> HandoverReadinessItems => Set<HandoverReadinessItem>();
    public DbSet<HandoverReadinessDefinition> HandoverReadinessDefinitions => Set<HandoverReadinessDefinition>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all EntityTypeConfiguration classes from assembly (all configurations in Data/Configuration)
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
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
