using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pdr.Api.Models.RoleBasedPermissions;

namespace Pdr.Api.Data.Configuration
{
    public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
    {
        public void Configure(EntityTypeBuilder<RolePermission> builder)
        {
            builder.ToTable("role_permissions");

            // Composite Primary Key
            builder.HasKey(rp => new { rp.RoleId, rp.Permission });

            builder.Property(rp => rp.RoleId)
                   .HasColumnName("role_id");

            builder.Property(rp => rp.Permission)
                   .HasColumnName("permission")
                   .HasMaxLength(100)
                   .IsRequired();

            // FK referencing teammate's Role entity
            builder.HasOne(rp => rp.Role)
                   .WithMany()
                   .HasForeignKey(rp => rp.RoleId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Seed Capabilities (Roles 1 to 4)
            builder.HasData(
                // 1. Pre Sales
                new RolePermission { RoleId = 1, Permission = "engagement:mark_sow_signed" },
                new RolePermission { RoleId = 1, Permission = "engagement:complete_handover" },
                new RolePermission { RoleId = 1, Permission = "engagement:record_committed_scope" },
                new RolePermission { RoleId = 1, Permission = "document:upload" },
                new RolePermission { RoleId = 1, Permission = "risk:raise" },
                new RolePermission { RoleId = 1, Permission = "discussion:comment" },

                // 2. Project Management
                new RolePermission { RoleId = 2, Permission = "engagement:set_commercial_model" },
                new RolePermission { RoleId = 2, Permission = "engagement:record_realistic_scope" },
                new RolePermission { RoleId = 2, Permission = "engagement:query_handover" },
                new RolePermission { RoleId = 2, Permission = "document:upload" },
                new RolePermission { RoleId = 2, Permission = "document:set_visibility" },
                new RolePermission { RoleId = 2, Permission = "stakeholder:manage" },
                new RolePermission { RoleId = 2, Permission = "checklist:tailor" },
                new RolePermission { RoleId = 2, Permission = "checklist:request_waiver" },
                new RolePermission { RoleId = 2, Permission = "governance:plan" },
                new RolePermission { RoleId = 2, Permission = "resource:request" },
                new RolePermission { RoleId = 2, Permission = "risk:raise" },
                new RolePermission { RoleId = 2, Permission = "discussion:comment" },
                new RolePermission { RoleId = 2, Permission = "signoff:request" },
                new RolePermission { RoleId = 2, Permission = "engagement:view_own" },
                new RolePermission { RoleId = 2, Permission = "one_pager:generate" },

                // 3. Tech Lead
                new RolePermission { RoleId = 3, Permission = "engagement:record_realistic_scope" },
                new RolePermission { RoleId = 3, Permission = "engagement:query_handover" },
                new RolePermission { RoleId = 3, Permission = "document:upload" },
                new RolePermission { RoleId = 3, Permission = "checklist:tailor_tech" },
                new RolePermission { RoleId = 3, Permission = "checklist:request_waiver" },
                new RolePermission { RoleId = 3, Permission = "tech_access:track" },
                new RolePermission { RoleId = 3, Permission = "tech_decision:log" },
                new RolePermission { RoleId = 3, Permission = "risk:raise" },
                new RolePermission { RoleId = 3, Permission = "discussion:comment" },
                new RolePermission { RoleId = 3, Permission = "engagement:view_own" },

                // 4. Leadership
                new RolePermission { RoleId = 4, Permission = "engagement:reassign_team" },
                new RolePermission { RoleId = 4, Permission = "checklist:approve_waiver" },
                new RolePermission { RoleId = 4, Permission = "change_request:approve" },
                new RolePermission { RoleId = 4, Permission = "resource:confirm" },
                new RolePermission { RoleId = 4, Permission = "discussion:comment" },
                new RolePermission { RoleId = 4, Permission = "signoff:approve" },
                new RolePermission { RoleId = 4, Permission = "dashboard:view_portfolio" },
                new RolePermission { RoleId = 4, Permission = "report:run_kpi" },
                new RolePermission { RoleId = 4, Permission = "audit:view_engagement" }
            );
        }
    }
}
