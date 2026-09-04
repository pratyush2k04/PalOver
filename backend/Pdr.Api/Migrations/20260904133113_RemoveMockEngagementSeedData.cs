using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Pdr.Api.Migrations
{
    /// <inheritdoc />
    public partial class RemoveMockEngagementSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AccessItems",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AccessItems",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "AccessItems",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "AccessItems",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "AccessItems",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "AccessItems",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "EnvironmentToolingItems",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "EnvironmentToolingItems",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "EnvironmentToolingItems",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "EnvironmentToolingItems",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "EnvironmentToolingItems",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "EnvironmentToolingItems",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "change_requests",
                keyColumn: "id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "commitments",
                keyColumn: "commitment_id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "communication_cadences",
                keyColumn: "id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "engagement_assignments",
                keyColumns: new[] { "engagement_id", "user_id" },
                keyValues: new object[] { 1, 1 });

            migrationBuilder.DeleteData(
                table: "engagement_assignments",
                keyColumns: new[] { "engagement_id", "user_id" },
                keyValues: new object[] { 1, 2 });

            migrationBuilder.DeleteData(
                table: "engagement_assignments",
                keyColumns: new[] { "engagement_id", "user_id" },
                keyValues: new object[] { 2, 2 });

            migrationBuilder.DeleteData(
                table: "engagement_assignments",
                keyColumns: new[] { "engagement_id", "user_id" },
                keyValues: new object[] { 1, 3 });

            migrationBuilder.DeleteData(
                table: "engagement_assignments",
                keyColumns: new[] { "engagement_id", "user_id" },
                keyValues: new object[] { 2, 3 });

            migrationBuilder.DeleteData(
                table: "engagement_assignments",
                keyColumns: new[] { "engagement_id", "user_id" },
                keyValues: new object[] { 1, 4 });

            migrationBuilder.DeleteData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 39);

            migrationBuilder.DeleteData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 40);

            migrationBuilder.DeleteData(
                table: "notifications",
                keyColumn: "id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "notifications",
                keyColumn: "id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "notifications",
                keyColumn: "id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "notifications",
                keyColumn: "id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "notifications",
                keyColumn: "id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "notifications",
                keyColumn: "id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "people",
                keyColumn: "person_id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "people",
                keyColumn: "person_id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "engagement_checklists",
                keyColumn: "id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "engagement_checklists",
                keyColumn: "id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "handover_info",
                keyColumn: "handover_id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "engagements",
                keyColumn: "engagement_id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "engagements",
                keyColumn: "engagement_id",
                keyValue: 2);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "communication_cadences",
                columns: new[] { "id", "cadence", "created_at", "created_by", "engagement_id", "participants", "stakeholder_role", "updated_at", "updated_by" },
                values: new object[] { 1, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", 1, "Rahul Verma, Asha Rao", "Client Sponsor", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null });

            migrationBuilder.InsertData(
                table: "engagements",
                columns: new[] { "engagement_id", "billing_start_date", "client_name", "commercial_model_id", "created_at", "created_by", "engagement_description", "engagement_name", "engagement_type_id", "kickoff_date", "status_id", "updated_at", "updated_by" },
                values: new object[,]
                {
                    { 1, null, "FinTech Solutions", 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Core Banking Greenfield Implementation", "FinTech Core Banking", 1, new DateOnly(2026, 8, 15), 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 2, new DateOnly(2026, 7, 1), "HealthCare Plus", 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Legacy System Upgrade and Support", "HealthCare Legacy Upgrade", 2, new DateOnly(2026, 6, 1), 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null }
                });

            migrationBuilder.InsertData(
                table: "notifications",
                columns: new[] { "id", "body", "created_at", "created_by", "data", "link", "priority", "read_at", "receiver_user_id", "scheduled_for", "template_id", "title", "updated_at", "updated_by" },
                values: new object[,]
                {
                    { 1, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "{\"AssignedUserId\":2,\"EngagementId\":1}", "/engagements/1/workspace/overview", 2, null, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 2, "This is a MOCK DB ENTRY notification seeded for manual testing. Use PUT /api/Notifications/2/read to mark it read, then confirm GET /api/Notifications/unread-count decreases by one.", new DateTime(2026, 8, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, "/engagements/1/workspace/overview", 2, null, 1, new DateTime(2026, 8, 5, 0, 0, 0, 0, DateTimeKind.Utc), null, "MOCK DB ENTRY notification — mark as read test", new DateTime(2026, 8, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 3, "This is a MOCK DB ENTRY notification seeded for manual testing. Use GET /api/Notifications?engagementId=1 to confirm this entry is included in the filtered results.", new DateTime(2026, 8, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "{\"EngagementId\":1}", "/engagements/1/workspace/handover", 3, null, 1, new DateTime(2026, 8, 6, 0, 0, 0, 0, DateTimeKind.Utc), null, "MOCK DB ENTRY notification — Engagement 1 filter test", new DateTime(2026, 8, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 4, "This is a MOCK DB ENTRY notification seeded for manual testing. Use GET /api/Notifications?engagementId=2 to confirm this entry is included, and GET /api/Notifications?engagementId=1 to confirm it's excluded. Link is intentionally null to test handling of notifications with no redirect target.", new DateTime(2026, 8, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "{\"EngagementId\":2}", null, 1, null, 1, new DateTime(2026, 8, 7, 0, 0, 0, 0, DateTimeKind.Utc), null, "MOCK DB ENTRY notification — Engagement 2, no link", new DateTime(2026, 8, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 5, "This is a MOCK DB ENTRY notification seeded for manual testing. ScheduledFor is in the future, so it should show Status: Pending and must be EXCLUDED from GET /api/Notifications/unread-count even though it is unread. Note: PUT /api/Notifications/read-all will still mark it read, since that endpoint doesn't check ScheduledFor.", new DateTime(2026, 8, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, "/engagements/2/workspace/overview", 2, null, 1, new DateTime(2026, 9, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "MOCK DB ENTRY notification — Pending (future scheduled)", new DateTime(2026, 8, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 6, "This is a MOCK DB ENTRY notification seeded for manual testing. It is already marked read and is intended as the target for DELETE /api/Notifications — send { \"ids\": [6] } to remove it and confirm it no longer appears in GET /api/Notifications.", new DateTime(2026, 8, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, null, 1, new DateTime(2026, 8, 4, 0, 0, 0, 0, DateTimeKind.Utc), 1, new DateTime(2026, 8, 3, 0, 0, 0, 0, DateTimeKind.Utc), null, "MOCK DB ENTRY notification — already read, safe to delete", new DateTime(2026, 8, 1, 0, 0, 0, 0, DateTimeKind.Utc), null }
                });

            migrationBuilder.InsertData(
                table: "AccessItems",
                columns: new[] { "Id", "AccessItemDefinitionId", "BlockReason", "CreatedAt", "CreatedBy", "EngagementId", "GrantedDate", "IsClientSide", "IsDeleted", "Name", "OwnerId", "Status", "TimeZoneNote", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, 1, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, null, false, false, "VPN Access", 1, 1, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 2, 2, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, null, true, false, "Jira", 1, 1, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 3, 3, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, null, true, false, "SandBox", 1, 1, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 4, 1, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, new DateOnly(2026, 6, 2), false, false, "VPN Access", 1, 2, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 5, 2, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, new DateOnly(2026, 6, 2), true, false, "Jira", 1, 2, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 6, 3, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, null, true, false, "SandBox", 1, 1, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null }
                });

            migrationBuilder.InsertData(
                table: "EnvironmentToolingItems",
                columns: new[] { "Id", "BlockReason", "CreatedAt", "CreatedBy", "CustomName", "EngagementId", "EnvironmentToolingDefinitionId", "IsDeleted", "Name", "OwnerId", "Status", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, 1, 1, false, "CI/CD", 1, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 2, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, 1, 2, false, "Dev", 1, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 3, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, 1, 3, false, "UAT Environment", 1, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 4, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, 2, 1, false, "CI/CD", 1, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 5, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, 2, 2, false, "Dev", 1, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 6, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, 2, 3, false, "UAT Environment", 1, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null }
                });

            migrationBuilder.InsertData(
                table: "change_requests",
                columns: new[] { "id", "created_at", "created_by", "description", "engagement_id", "impact", "rejection_reason", "reviewed_at", "reviewed_by_id", "status", "updated_at", "updated_by" },
                values: new object[] { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "2", "Add SMS notifications to the rewards flow", 1, "+1.5 weeks, +1 developer-week", null, null, null, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "2" });

            migrationBuilder.InsertData(
                table: "engagement_assignments",
                columns: new[] { "engagement_id", "user_id" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 1, 2 },
                    { 2, 2 },
                    { 1, 3 },
                    { 2, 3 },
                    { 1, 4 }
                });

            migrationBuilder.InsertData(
                table: "engagement_checklists",
                columns: new[] { "id", "created_at", "created_by", "engagement_id", "source_master_version_id", "updated_at", "updated_by", "version_number" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", 1, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", 0 },
                    { 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", 2, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", 0 }
                });

            migrationBuilder.InsertData(
                table: "handover_info",
                columns: new[] { "handover_id", "business_problem", "client_background", "client_sentiment", "created_at", "created_by", "deal_origin", "engagement_id", "reviewed_at", "reviewed_by_user_id", "status_id", "submitted_at", "submitted_by_user_id", "updated_at", "updated_by" },
                values: new object[] { 1, "They need a modern core banking platform to support new digital initiatives and mobile apps.", "FinTech Solutions is a mid-sized financial institution expanding its digital footprint.", "Excited but cautious about delivery timelines due to past vendor failures.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Strategic Partner Referral", 1, null, null, 1, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null });

            migrationBuilder.InsertData(
                table: "people",
                columns: new[] { "person_id", "created_at", "created_by", "engagement_id", "name", "person_type", "role", "source_role_id", "updated_at", "updated_by", "user_id" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", 1, "Sarah Jenkins", "Client", "Client Executive Sponsor", null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null },
                    { 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", 1, "Alex Rivera", "Internal", "Internal Technical Lead", null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null }
                });

            migrationBuilder.InsertData(
                table: "commitments",
                columns: new[] { "commitment_id", "commitment_type_id", "created_at", "created_by", "description", "handover_id", "timeline", "timeline_unit", "title", "updated_at", "updated_by" },
                values: new object[] { 1, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Delivery of MVP banking module within 3 months of kickoff.", 1, 3, "Months", "MVP Banking Module Delivery", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null });

            migrationBuilder.InsertData(
                table: "engagement_checklist_items",
                columns: new[] { "id", "block_reason", "category_id", "created_at", "created_by", "description", "display_order", "engagement_checklist_id", "evidence_document_id", "evidence_required", "evidence_submitted_at", "evidence_submitted_by_id", "exemption_rejection_reason", "exemption_requested_at", "exemption_requested_by_id", "exemption_reviewed_at", "exemption_reviewed_by_id", "is_applicable", "is_mandatory", "not_applicable_reason", "owner_id", "source_master_item_id", "status", "title", "updated_at", "updated_by" },
                values: new object[,]
                {
                    { 1, null, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, 1, 1, null, true, null, null, null, null, null, null, null, true, true, null, null, 1, "Incomplete", "Sign commercial contract", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 2, null, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, 2, 1, null, true, null, null, null, null, null, null, null, true, true, null, null, 2, "Incomplete", "Confirm engagement budget", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 3, null, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, 3, 1, null, true, null, null, null, null, null, null, null, true, true, null, null, 3, "Incomplete", "Confirm billing schedule", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 4, null, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, 4, 1, null, true, null, null, null, null, null, null, null, true, true, null, null, 4, "Incomplete", "Complete sales-to-delivery handover call", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 5, null, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, 5, 1, null, true, null, null, null, null, null, null, null, true, true, null, null, 5, "Incomplete", "Share CRM opportunity notes", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 6, null, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, 6, 1, null, true, null, null, null, null, null, null, null, true, true, null, null, 6, "Incomplete", "Transfer key client contacts", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 7, null, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, 7, 1, null, true, null, null, null, null, null, null, null, true, true, null, null, 7, "Incomplete", "Document engagement scope", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 8, null, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, 8, 1, null, true, null, null, null, null, null, null, null, true, true, null, null, 8, "Incomplete", "Confirm deliverables list", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null }
                });

            migrationBuilder.InsertData(
                table: "engagement_checklist_items",
                columns: new[] { "id", "block_reason", "category_id", "created_at", "created_by", "description", "display_order", "engagement_checklist_id", "evidence_document_id", "evidence_required", "evidence_submitted_at", "evidence_submitted_by_id", "exemption_rejection_reason", "exemption_requested_at", "exemption_requested_by_id", "exemption_reviewed_at", "exemption_reviewed_by_id", "is_applicable", "not_applicable_reason", "owner_id", "source_master_item_id", "status", "title", "updated_at", "updated_by" },
                values: new object[] { 9, null, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, 9, 1, null, true, null, null, null, null, null, null, null, true, null, null, 9, "Incomplete", "Identify scope assumptions and exclusions", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null });

            migrationBuilder.InsertData(
                table: "engagement_checklist_items",
                columns: new[] { "id", "block_reason", "category_id", "created_at", "created_by", "description", "display_order", "engagement_checklist_id", "evidence_document_id", "evidence_required", "evidence_submitted_at", "evidence_submitted_by_id", "exemption_rejection_reason", "exemption_requested_at", "exemption_requested_by_id", "exemption_reviewed_at", "exemption_reviewed_by_id", "is_applicable", "is_mandatory", "not_applicable_reason", "owner_id", "source_master_item_id", "status", "title", "updated_at", "updated_by" },
                values: new object[,]
                {
                    { 10, null, 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, 10, 1, null, true, null, null, null, null, null, null, null, true, true, null, null, 10, "Incomplete", "Set up engagement governance cadence", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 11, null, 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, 11, 1, null, true, null, null, null, null, null, null, null, true, true, null, null, 11, "Incomplete", "Assign engagement steering committee", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 12, null, 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, 12, 1, null, true, null, null, null, null, null, null, null, true, true, null, null, 12, "Incomplete", "Define change control process", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null }
                });

            migrationBuilder.InsertData(
                table: "engagement_checklist_items",
                columns: new[] { "id", "block_reason", "category_id", "created_at", "created_by", "description", "display_order", "engagement_checklist_id", "evidence_document_id", "evidence_required", "evidence_submitted_at", "evidence_submitted_by_id", "exemption_rejection_reason", "exemption_requested_at", "exemption_requested_by_id", "exemption_reviewed_at", "exemption_reviewed_by_id", "is_applicable", "not_applicable_reason", "owner_id", "source_master_item_id", "status", "title", "updated_at", "updated_by" },
                values: new object[] { 13, null, 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, 13, 1, null, true, null, null, null, null, null, null, null, true, null, null, 13, "Incomplete", "Confirm RAID log owner", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null });

            migrationBuilder.InsertData(
                table: "engagement_checklist_items",
                columns: new[] { "id", "block_reason", "category_id", "created_at", "created_by", "description", "display_order", "engagement_checklist_id", "evidence_document_id", "evidence_required", "evidence_submitted_at", "evidence_submitted_by_id", "exemption_rejection_reason", "exemption_requested_at", "exemption_requested_by_id", "exemption_reviewed_at", "exemption_reviewed_by_id", "is_applicable", "is_mandatory", "not_applicable_reason", "owner_id", "source_master_item_id", "status", "title", "updated_at", "updated_by" },
                values: new object[,]
                {
                    { 14, null, 5, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, 14, 1, null, true, null, null, null, null, null, null, null, true, true, null, null, 14, "Incomplete", "Allocate delivery team members", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 15, null, 5, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, 15, 1, null, true, null, null, null, null, null, null, null, true, true, null, null, 15, "Incomplete", "Confirm resource availability calendar", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null }
                });

            migrationBuilder.InsertData(
                table: "engagement_checklist_items",
                columns: new[] { "id", "block_reason", "category_id", "created_at", "created_by", "description", "display_order", "engagement_checklist_id", "evidence_document_id", "evidence_required", "evidence_submitted_at", "evidence_submitted_by_id", "exemption_rejection_reason", "exemption_requested_at", "exemption_requested_by_id", "exemption_reviewed_at", "exemption_reviewed_by_id", "is_applicable", "not_applicable_reason", "owner_id", "source_master_item_id", "status", "title", "updated_at", "updated_by" },
                values: new object[] { 16, null, 5, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, 16, 1, null, true, null, null, null, null, null, null, null, true, null, null, 16, "Incomplete", "Identify backup/on-call resources", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null });

            migrationBuilder.InsertData(
                table: "engagement_checklist_items",
                columns: new[] { "id", "block_reason", "category_id", "created_at", "created_by", "description", "display_order", "engagement_checklist_id", "evidence_document_id", "evidence_required", "evidence_submitted_at", "evidence_submitted_by_id", "exemption_rejection_reason", "exemption_requested_at", "exemption_requested_by_id", "exemption_reviewed_at", "exemption_reviewed_by_id", "is_applicable", "is_mandatory", "not_applicable_reason", "owner_id", "source_master_item_id", "status", "title", "updated_at", "updated_by" },
                values: new object[,]
                {
                    { 17, null, 6, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, 17, 1, null, true, null, null, null, null, null, null, null, true, true, null, null, 17, "Incomplete", "Provision environment access", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 18, null, 6, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, 18, 1, null, true, null, null, null, null, null, null, null, true, true, null, null, 18, "Incomplete", "Verify technical prerequisites", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 19, null, 6, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, 19, 1, null, true, null, null, null, null, null, null, null, true, true, null, null, 19, "Incomplete", "Set up source control and CI/CD access", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null }
                });

            migrationBuilder.InsertData(
                table: "engagement_checklist_items",
                columns: new[] { "id", "block_reason", "category_id", "created_at", "created_by", "description", "display_order", "engagement_checklist_id", "evidence_document_id", "evidence_required", "evidence_submitted_at", "evidence_submitted_by_id", "exemption_rejection_reason", "exemption_requested_at", "exemption_requested_by_id", "exemption_reviewed_at", "exemption_reviewed_by_id", "is_applicable", "not_applicable_reason", "owner_id", "source_master_item_id", "status", "title", "updated_at", "updated_by" },
                values: new object[] { 20, null, 6, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, 20, 1, null, true, null, null, null, null, null, null, null, true, null, null, 20, "Incomplete", "Confirm security and compliance requirements", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null });

            migrationBuilder.InsertData(
                table: "engagement_checklist_items",
                columns: new[] { "id", "block_reason", "category_id", "created_at", "created_by", "description", "display_order", "engagement_checklist_id", "evidence_document_id", "evidence_required", "evidence_submitted_at", "evidence_submitted_by_id", "exemption_rejection_reason", "exemption_requested_at", "exemption_requested_by_id", "exemption_reviewed_at", "exemption_reviewed_by_id", "is_applicable", "is_mandatory", "not_applicable_reason", "owner_id", "source_master_item_id", "status", "title", "updated_at", "updated_by" },
                values: new object[,]
                {
                    { 21, null, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, 1, 2, null, true, null, null, null, null, null, null, null, true, true, null, null, 1, "Incomplete", "Sign commercial contract", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 22, null, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, 2, 2, null, true, null, null, null, null, null, null, null, true, true, null, null, 2, "Incomplete", "Confirm engagement budget", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 23, null, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, 3, 2, null, true, null, null, null, null, null, null, null, true, true, null, null, 3, "Incomplete", "Confirm billing schedule", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 24, null, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, 4, 2, null, true, null, null, null, null, null, null, null, true, true, null, null, 4, "Incomplete", "Complete sales-to-delivery handover call", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 25, null, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, 5, 2, null, true, null, null, null, null, null, null, null, true, true, null, null, 5, "Incomplete", "Share CRM opportunity notes", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 26, null, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, 6, 2, null, true, null, null, null, null, null, null, null, true, true, null, null, 6, "Incomplete", "Transfer key client contacts", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 27, null, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, 7, 2, null, true, null, null, null, null, null, null, null, true, true, null, null, 7, "Incomplete", "Document engagement scope", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 28, null, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, 8, 2, null, true, null, null, null, null, null, null, null, true, true, null, null, 8, "Incomplete", "Confirm deliverables list", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null }
                });

            migrationBuilder.InsertData(
                table: "engagement_checklist_items",
                columns: new[] { "id", "block_reason", "category_id", "created_at", "created_by", "description", "display_order", "engagement_checklist_id", "evidence_document_id", "evidence_required", "evidence_submitted_at", "evidence_submitted_by_id", "exemption_rejection_reason", "exemption_requested_at", "exemption_requested_by_id", "exemption_reviewed_at", "exemption_reviewed_by_id", "is_applicable", "not_applicable_reason", "owner_id", "source_master_item_id", "status", "title", "updated_at", "updated_by" },
                values: new object[] { 29, null, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, 9, 2, null, true, null, null, null, null, null, null, null, true, null, null, 9, "Incomplete", "Identify scope assumptions and exclusions", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null });

            migrationBuilder.InsertData(
                table: "engagement_checklist_items",
                columns: new[] { "id", "block_reason", "category_id", "created_at", "created_by", "description", "display_order", "engagement_checklist_id", "evidence_document_id", "evidence_required", "evidence_submitted_at", "evidence_submitted_by_id", "exemption_rejection_reason", "exemption_requested_at", "exemption_requested_by_id", "exemption_reviewed_at", "exemption_reviewed_by_id", "is_applicable", "is_mandatory", "not_applicable_reason", "owner_id", "source_master_item_id", "status", "title", "updated_at", "updated_by" },
                values: new object[,]
                {
                    { 30, null, 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, 10, 2, null, true, null, null, null, null, null, null, null, true, true, null, null, 10, "Incomplete", "Set up engagement governance cadence", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 31, null, 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, 11, 2, null, true, null, null, null, null, null, null, null, true, true, null, null, 11, "Incomplete", "Assign engagement steering committee", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 32, null, 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, 12, 2, null, true, null, null, null, null, null, null, null, true, true, null, null, 12, "Incomplete", "Define change control process", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null }
                });

            migrationBuilder.InsertData(
                table: "engagement_checklist_items",
                columns: new[] { "id", "block_reason", "category_id", "created_at", "created_by", "description", "display_order", "engagement_checklist_id", "evidence_document_id", "evidence_required", "evidence_submitted_at", "evidence_submitted_by_id", "exemption_rejection_reason", "exemption_requested_at", "exemption_requested_by_id", "exemption_reviewed_at", "exemption_reviewed_by_id", "is_applicable", "not_applicable_reason", "owner_id", "source_master_item_id", "status", "title", "updated_at", "updated_by" },
                values: new object[] { 33, null, 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, 13, 2, null, true, null, null, null, null, null, null, null, true, null, null, 13, "Incomplete", "Confirm RAID log owner", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null });

            migrationBuilder.InsertData(
                table: "engagement_checklist_items",
                columns: new[] { "id", "block_reason", "category_id", "created_at", "created_by", "description", "display_order", "engagement_checklist_id", "evidence_document_id", "evidence_required", "evidence_submitted_at", "evidence_submitted_by_id", "exemption_rejection_reason", "exemption_requested_at", "exemption_requested_by_id", "exemption_reviewed_at", "exemption_reviewed_by_id", "is_applicable", "is_mandatory", "not_applicable_reason", "owner_id", "source_master_item_id", "status", "title", "updated_at", "updated_by" },
                values: new object[,]
                {
                    { 34, null, 5, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, 14, 2, null, true, null, null, null, null, null, null, null, true, true, null, null, 14, "Incomplete", "Allocate delivery team members", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 35, null, 5, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, 15, 2, null, true, null, null, null, null, null, null, null, true, true, null, null, 15, "Incomplete", "Confirm resource availability calendar", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null }
                });

            migrationBuilder.InsertData(
                table: "engagement_checklist_items",
                columns: new[] { "id", "block_reason", "category_id", "created_at", "created_by", "description", "display_order", "engagement_checklist_id", "evidence_document_id", "evidence_required", "evidence_submitted_at", "evidence_submitted_by_id", "exemption_rejection_reason", "exemption_requested_at", "exemption_requested_by_id", "exemption_reviewed_at", "exemption_reviewed_by_id", "is_applicable", "not_applicable_reason", "owner_id", "source_master_item_id", "status", "title", "updated_at", "updated_by" },
                values: new object[] { 36, null, 5, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, 16, 2, null, true, null, null, null, null, null, null, null, true, null, null, 16, "Incomplete", "Identify backup/on-call resources", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null });

            migrationBuilder.InsertData(
                table: "engagement_checklist_items",
                columns: new[] { "id", "block_reason", "category_id", "created_at", "created_by", "description", "display_order", "engagement_checklist_id", "evidence_document_id", "evidence_required", "evidence_submitted_at", "evidence_submitted_by_id", "exemption_rejection_reason", "exemption_requested_at", "exemption_requested_by_id", "exemption_reviewed_at", "exemption_reviewed_by_id", "is_applicable", "is_mandatory", "not_applicable_reason", "owner_id", "source_master_item_id", "status", "title", "updated_at", "updated_by" },
                values: new object[,]
                {
                    { 37, null, 6, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, 17, 2, null, true, null, null, null, null, null, null, null, true, true, null, null, 17, "Incomplete", "Provision environment access", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 38, null, 6, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, 18, 2, null, true, null, null, null, null, null, null, null, true, true, null, null, 18, "Incomplete", "Verify technical prerequisites", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 39, null, 6, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, 19, 2, null, true, null, null, null, null, null, null, null, true, true, null, null, 19, "Incomplete", "Set up source control and CI/CD access", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null }
                });

            migrationBuilder.InsertData(
                table: "engagement_checklist_items",
                columns: new[] { "id", "block_reason", "category_id", "created_at", "created_by", "description", "display_order", "engagement_checklist_id", "evidence_document_id", "evidence_required", "evidence_submitted_at", "evidence_submitted_by_id", "exemption_rejection_reason", "exemption_requested_at", "exemption_requested_by_id", "exemption_reviewed_at", "exemption_reviewed_by_id", "is_applicable", "not_applicable_reason", "owner_id", "source_master_item_id", "status", "title", "updated_at", "updated_by" },
                values: new object[] { 40, null, 6, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, 20, 2, null, true, null, null, null, null, null, null, null, true, null, null, 20, "Incomplete", "Confirm security and compliance requirements", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null });
        }
    }
}
