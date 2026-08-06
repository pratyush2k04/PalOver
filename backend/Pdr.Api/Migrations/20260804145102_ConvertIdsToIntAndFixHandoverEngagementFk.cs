using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Pdr.Api.Migrations
{
    /// <inheritdoc />
    public partial class ConvertIdsToIntAndFixHandoverEngagementFk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_engagements_handover_info_HandoverInfoHandoverId",
                table: "engagements");

            migrationBuilder.DropIndex(
                name: "IX_engagements_HandoverInfoHandoverId",
                table: "engagements");

            migrationBuilder.DeleteData(
                table: "commitments",
                keyColumn: "commitment_id",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 2L);

            migrationBuilder.DeleteData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 3L);

            migrationBuilder.DeleteData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 4L);

            migrationBuilder.DeleteData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 5L);

            migrationBuilder.DeleteData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 6L);

            migrationBuilder.DeleteData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 7L);

            migrationBuilder.DeleteData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 8L);

            migrationBuilder.DeleteData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 9L);

            migrationBuilder.DeleteData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 10L);

            migrationBuilder.DeleteData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 11L);

            migrationBuilder.DeleteData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 12L);

            migrationBuilder.DeleteData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 13L);

            migrationBuilder.DeleteData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 14L);

            migrationBuilder.DeleteData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 15L);

            migrationBuilder.DeleteData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 16L);

            migrationBuilder.DeleteData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 17L);

            migrationBuilder.DeleteData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 18L);

            migrationBuilder.DeleteData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 19L);

            migrationBuilder.DeleteData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 20L);

            migrationBuilder.DeleteData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 21L);

            migrationBuilder.DeleteData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 22L);

            migrationBuilder.DeleteData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 23L);

            migrationBuilder.DeleteData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 24L);

            migrationBuilder.DeleteData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 25L);

            migrationBuilder.DeleteData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 26L);

            migrationBuilder.DeleteData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 27L);

            migrationBuilder.DeleteData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 28L);

            migrationBuilder.DeleteData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 29L);

            migrationBuilder.DeleteData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 30L);

            migrationBuilder.DeleteData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 31L);

            migrationBuilder.DeleteData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 32L);

            migrationBuilder.DeleteData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 33L);

            migrationBuilder.DeleteData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 34L);

            migrationBuilder.DeleteData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 35L);

            migrationBuilder.DeleteData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 36L);

            migrationBuilder.DeleteData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 37L);

            migrationBuilder.DeleteData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 38L);

            migrationBuilder.DeleteData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 39L);

            migrationBuilder.DeleteData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 40L);

            migrationBuilder.DeleteData(
                table: "checklist_categories",
                keyColumn: "id",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "checklist_categories",
                keyColumn: "id",
                keyValue: 2L);

            migrationBuilder.DeleteData(
                table: "checklist_categories",
                keyColumn: "id",
                keyValue: 3L);

            migrationBuilder.DeleteData(
                table: "checklist_categories",
                keyColumn: "id",
                keyValue: 4L);

            migrationBuilder.DeleteData(
                table: "checklist_categories",
                keyColumn: "id",
                keyValue: 5L);

            migrationBuilder.DeleteData(
                table: "checklist_categories",
                keyColumn: "id",
                keyValue: 6L);

            migrationBuilder.DeleteData(
                table: "handover_info",
                keyColumn: "handover_id",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "master_checklist_versions",
                keyColumn: "id",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "master_checklist_versions",
                keyColumn: "id",
                keyValue: 2L);

            migrationBuilder.DeleteData(
                table: "checklist_statuses",
                keyColumn: "id",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "checklist_statuses",
                keyColumn: "id",
                keyValue: 2L);

            migrationBuilder.DropColumn(
                name: "HandoverInfoHandoverId",
                table: "engagements");

            migrationBuilder.AlterColumn<int>(
                name: "status_id",
                table: "master_checklist_versions",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "master_checklist_versions",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn);

            migrationBuilder.AlterColumn<int>(
                name: "version_id",
                table: "master_checklist_items",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "category_id",
                table: "master_checklist_items",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "master_checklist_items",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn);

            migrationBuilder.AlterColumn<int>(
                name: "engagement_id",
                table: "handover_info",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "handover_id",
                table: "handover_info",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn);

            migrationBuilder.AddColumn<int>(
                name: "checklist_version_id",
                table: "engagements",
                type: "integer",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "handover_id",
                table: "commitments",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "commitment_id",
                table: "commitments",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn);

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "checklist_statuses",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn);

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "checklist_categories",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn);

            migrationBuilder.InsertData(
                table: "checklist_categories",
                columns: new[] { "id", "category_name" },
                values: new object[,]
                {
                    { 1, "Commercial" },
                    { 2, "Handover" },
                    { 3, "Scope" },
                    { 4, "Governance" },
                    { 5, "Resourcing" },
                    { 6, "Technical Readiness" }
                });

            migrationBuilder.InsertData(
                table: "checklist_statuses",
                columns: new[] { "id", "status_name" },
                values: new object[,]
                {
                    { 1, "Draft" },
                    { 2, "Published" }
                });

            migrationBuilder.UpdateData(
                table: "engagements",
                keyColumn: "engagement_id",
                keyValue: 1,
                column: "checklist_version_id",
                value: null);

            migrationBuilder.UpdateData(
                table: "engagements",
                keyColumn: "engagement_id",
                keyValue: 2,
                column: "checklist_version_id",
                value: null);

            migrationBuilder.InsertData(
                table: "handover_info",
                columns: new[] { "handover_id", "business_problem", "client_background", "client_sentiment", "created_at", "created_by", "deal_origin", "engagement_id", "status_id", "updated_at", "updated_by" },
                values: new object[] { 1, "They need a modern core banking platform to support new digital initiatives and mobile apps.", "FinTech Solutions is a mid-sized financial institution expanding its digital footprint.", "Excited but cautious about delivery timelines due to past vendor failures.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Strategic Partner Referral", 1, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null });

            migrationBuilder.InsertData(
                table: "commitments",
                columns: new[] { "commitment_id", "commitment_type_id", "created_at", "created_by", "description", "handover_id", "quantity", "updated_at", "updated_by" },
                values: new object[] { 1, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Delivery of MVP banking module within 3 months of kickoff.", 1, "1", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null });

            migrationBuilder.InsertData(
                table: "master_checklist_versions",
                columns: new[] { "id", "created_at", "created_by", "status_id", "updated_at", "updated_by", "version_number" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 0 },
                    { 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1 }
                });

            migrationBuilder.InsertData(
                table: "master_checklist_items",
                columns: new[] { "id", "category_id", "created_at", "created_by", "description", "display_order", "is_mandatory", "title", "updated_at", "updated_by", "version_id" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Confirm the signed commercial contract is on file before kickoff.", 1, true, "Sign commercial contract", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1 },
                    { 2, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Verify the approved budget matches the statement of work.", 2, true, "Confirm engagement budget", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1 },
                    { 3, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Agree invoicing milestones and payment terms with the client.", 3, true, "Confirm billing schedule", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1 },
                    { 4, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Sales team walks delivery team through client history and expectations.", 4, true, "Complete sales-to-delivery handover call", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1 },
                    { 5, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Ensure delivery has access to all pre-sales notes and correspondence.", 5, true, "Share CRM opportunity notes", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1 },
                    { 6, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Confirm delivery team has contact details for all client stakeholders.", 6, true, "Transfer key client contacts", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1 },
                    { 7, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Capture in-scope and out-of-scope items in the kickoff scope document.", 7, true, "Document engagement scope", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1 },
                    { 8, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "List and confirm all deliverables committed to the client.", 8, true, "Confirm deliverables list", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1 }
                });

            migrationBuilder.InsertData(
                table: "master_checklist_items",
                columns: new[] { "id", "category_id", "created_at", "created_by", "description", "display_order", "title", "updated_at", "updated_by", "version_id" },
                values: new object[] { 9, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Record assumptions made during the sales cycle that affect scope.", 9, "Identify scope assumptions and exclusions", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1 });

            migrationBuilder.InsertData(
                table: "master_checklist_items",
                columns: new[] { "id", "category_id", "created_at", "created_by", "description", "display_order", "is_mandatory", "title", "updated_at", "updated_by", "version_id" },
                values: new object[,]
                {
                    { 10, 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Agree recurring status meetings and escalation paths.", 10, true, "Set up engagement governance cadence", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1 },
                    { 11, 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Identify decision-makers on both client and delivery sides.", 11, true, "Assign engagement steering committee", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1 },
                    { 12, 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Agree how scope changes will be raised, approved, and tracked.", 12, true, "Define change control process", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1 }
                });

            migrationBuilder.InsertData(
                table: "master_checklist_items",
                columns: new[] { "id", "category_id", "created_at", "created_by", "description", "display_order", "title", "updated_at", "updated_by", "version_id" },
                values: new object[] { 13, 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Assign an owner to maintain the Risks, Assumptions, Issues, and Dependencies log.", 13, "Confirm RAID log owner", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1 });

            migrationBuilder.InsertData(
                table: "master_checklist_items",
                columns: new[] { "id", "category_id", "created_at", "created_by", "description", "display_order", "is_mandatory", "title", "updated_at", "updated_by", "version_id" },
                values: new object[,]
                {
                    { 14, 5, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Confirm named resources are assigned and available for the engagement.", 14, true, "Allocate delivery team members", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1 },
                    { 15, 5, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Check assigned resources have capacity for the engagement duration.", 15, true, "Confirm resource availability calendar", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1 }
                });

            migrationBuilder.InsertData(
                table: "master_checklist_items",
                columns: new[] { "id", "category_id", "created_at", "created_by", "description", "display_order", "title", "updated_at", "updated_by", "version_id" },
                values: new object[] { 16, 5, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Name backup resources in case of absence during the engagement.", 16, "Identify backup/on-call resources", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1 });

            migrationBuilder.InsertData(
                table: "master_checklist_items",
                columns: new[] { "id", "category_id", "created_at", "created_by", "description", "display_order", "is_mandatory", "title", "updated_at", "updated_by", "version_id" },
                values: new object[,]
                {
                    { 17, 6, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Request and confirm access to required client systems and environments.", 17, true, "Provision environment access", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1 },
                    { 18, 6, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Confirm client infrastructure and tooling meet engagement requirements.", 18, true, "Verify technical prerequisites", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1 },
                    { 19, 6, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Ensure delivery team has access to repositories and pipelines.", 19, true, "Set up source control and CI/CD access", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1 }
                });

            migrationBuilder.InsertData(
                table: "master_checklist_items",
                columns: new[] { "id", "category_id", "created_at", "created_by", "description", "display_order", "title", "updated_at", "updated_by", "version_id" },
                values: new object[] { 20, 6, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Review any client-specific security or compliance obligations.", 20, "Confirm security and compliance requirements", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1 });

            migrationBuilder.InsertData(
                table: "master_checklist_items",
                columns: new[] { "id", "category_id", "created_at", "created_by", "description", "display_order", "is_mandatory", "title", "updated_at", "updated_by", "version_id" },
                values: new object[,]
                {
                    { 21, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Confirm the signed commercial contract is on file before kickoff.", 1, true, "Sign commercial contract", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2 },
                    { 22, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Verify the approved budget matches the statement of work.", 2, true, "Confirm engagement budget", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2 },
                    { 23, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Agree invoicing milestones and payment terms with the client.", 3, true, "Confirm billing schedule", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2 },
                    { 24, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Sales team walks delivery team through client history and expectations.", 4, true, "Complete sales-to-delivery handover call", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2 },
                    { 25, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Ensure delivery has access to all pre-sales notes and correspondence.", 5, true, "Share CRM opportunity notes", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2 },
                    { 26, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Confirm delivery team has contact details for all client stakeholders.", 6, true, "Transfer key client contacts", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2 },
                    { 27, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Capture in-scope and out-of-scope items in the kickoff scope document.", 7, true, "Document engagement scope", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2 },
                    { 28, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "List and confirm all deliverables committed to the client.", 8, true, "Confirm deliverables list", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2 }
                });

            migrationBuilder.InsertData(
                table: "master_checklist_items",
                columns: new[] { "id", "category_id", "created_at", "created_by", "description", "display_order", "title", "updated_at", "updated_by", "version_id" },
                values: new object[] { 29, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Record assumptions made during the sales cycle that affect scope.", 9, "Identify scope assumptions and exclusions", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2 });

            migrationBuilder.InsertData(
                table: "master_checklist_items",
                columns: new[] { "id", "category_id", "created_at", "created_by", "description", "display_order", "is_mandatory", "title", "updated_at", "updated_by", "version_id" },
                values: new object[,]
                {
                    { 30, 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Agree recurring status meetings and escalation paths.", 10, true, "Set up engagement governance cadence", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2 },
                    { 31, 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Identify decision-makers on both client and delivery sides.", 11, true, "Assign engagement steering committee", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2 },
                    { 32, 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Agree how scope changes will be raised, approved, and tracked.", 12, true, "Define change control process", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2 }
                });

            migrationBuilder.InsertData(
                table: "master_checklist_items",
                columns: new[] { "id", "category_id", "created_at", "created_by", "description", "display_order", "title", "updated_at", "updated_by", "version_id" },
                values: new object[] { 33, 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Assign an owner to maintain the Risks, Assumptions, Issues, and Dependencies log.", 13, "Confirm RAID log owner", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2 });

            migrationBuilder.InsertData(
                table: "master_checklist_items",
                columns: new[] { "id", "category_id", "created_at", "created_by", "description", "display_order", "is_mandatory", "title", "updated_at", "updated_by", "version_id" },
                values: new object[,]
                {
                    { 34, 5, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Confirm named resources are assigned and available for the engagement.", 14, true, "Allocate delivery team members", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2 },
                    { 35, 5, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Check assigned resources have capacity for the engagement duration.", 15, true, "Confirm resource availability calendar", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2 }
                });

            migrationBuilder.InsertData(
                table: "master_checklist_items",
                columns: new[] { "id", "category_id", "created_at", "created_by", "description", "display_order", "title", "updated_at", "updated_by", "version_id" },
                values: new object[] { 36, 5, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Name backup resources in case of absence during the engagement.", 16, "Identify backup/on-call resources", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2 });

            migrationBuilder.InsertData(
                table: "master_checklist_items",
                columns: new[] { "id", "category_id", "created_at", "created_by", "description", "display_order", "is_mandatory", "title", "updated_at", "updated_by", "version_id" },
                values: new object[,]
                {
                    { 37, 6, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Request and confirm access to required client systems and environments.", 17, true, "Provision environment access", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2 },
                    { 38, 6, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Confirm client infrastructure and tooling meet engagement requirements.", 18, true, "Verify technical prerequisites", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2 },
                    { 39, 6, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Ensure delivery team has access to repositories and pipelines.", 19, true, "Set up source control and CI/CD access", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2 }
                });

            migrationBuilder.InsertData(
                table: "master_checklist_items",
                columns: new[] { "id", "category_id", "created_at", "created_by", "description", "display_order", "title", "updated_at", "updated_by", "version_id" },
                values: new object[] { 40, 6, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Review any client-specific security or compliance obligations.", 20, "Confirm security and compliance requirements", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2 });

            migrationBuilder.CreateIndex(
                name: "IX_handover_info_engagement_id",
                table: "handover_info",
                column: "engagement_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_engagements_checklist_version_id",
                table: "engagements",
                column: "checklist_version_id");

            migrationBuilder.AddForeignKey(
                name: "FK_engagements_master_checklist_versions_checklist_version_id",
                table: "engagements",
                column: "checklist_version_id",
                principalTable: "master_checklist_versions",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_handover_info_engagements_engagement_id",
                table: "handover_info",
                column: "engagement_id",
                principalTable: "engagements",
                principalColumn: "engagement_id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_engagements_master_checklist_versions_checklist_version_id",
                table: "engagements");

            migrationBuilder.DropForeignKey(
                name: "FK_handover_info_engagements_engagement_id",
                table: "handover_info");

            migrationBuilder.DropIndex(
                name: "IX_handover_info_engagement_id",
                table: "handover_info");

            migrationBuilder.DropIndex(
                name: "IX_engagements_checklist_version_id",
                table: "engagements");

            migrationBuilder.DeleteData(
                table: "commitments",
                keyColumn: "commitment_id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 39);

            migrationBuilder.DeleteData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 40);

            migrationBuilder.DeleteData(
                table: "checklist_categories",
                keyColumn: "id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "checklist_categories",
                keyColumn: "id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "checklist_categories",
                keyColumn: "id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "checklist_categories",
                keyColumn: "id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "checklist_categories",
                keyColumn: "id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "checklist_categories",
                keyColumn: "id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "handover_info",
                keyColumn: "handover_id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "master_checklist_versions",
                keyColumn: "id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "master_checklist_versions",
                keyColumn: "id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "checklist_statuses",
                keyColumn: "id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "checklist_statuses",
                keyColumn: "id",
                keyValue: 2);

            migrationBuilder.DropColumn(
                name: "checklist_version_id",
                table: "engagements");

            migrationBuilder.AlterColumn<long>(
                name: "status_id",
                table: "master_checklist_versions",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<long>(
                name: "id",
                table: "master_checklist_versions",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn);

            migrationBuilder.AlterColumn<long>(
                name: "version_id",
                table: "master_checklist_items",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<long>(
                name: "category_id",
                table: "master_checklist_items",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<long>(
                name: "id",
                table: "master_checklist_items",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn);

            migrationBuilder.AlterColumn<long>(
                name: "engagement_id",
                table: "handover_info",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<long>(
                name: "handover_id",
                table: "handover_info",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn);

            migrationBuilder.AddColumn<long>(
                name: "HandoverInfoHandoverId",
                table: "engagements",
                type: "bigint",
                nullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "handover_id",
                table: "commitments",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<long>(
                name: "commitment_id",
                table: "commitments",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn);

            migrationBuilder.AlterColumn<long>(
                name: "id",
                table: "checklist_statuses",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn);

            migrationBuilder.AlterColumn<long>(
                name: "id",
                table: "checklist_categories",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn);

            migrationBuilder.InsertData(
                table: "checklist_categories",
                columns: new[] { "id", "category_name" },
                values: new object[,]
                {
                    { 1L, "Commercial" },
                    { 2L, "Handover" },
                    { 3L, "Scope" },
                    { 4L, "Governance" },
                    { 5L, "Resourcing" },
                    { 6L, "Technical Readiness" }
                });

            migrationBuilder.InsertData(
                table: "checklist_statuses",
                columns: new[] { "id", "status_name" },
                values: new object[,]
                {
                    { 1L, "Draft" },
                    { 2L, "Published" }
                });

            migrationBuilder.UpdateData(
                table: "engagements",
                keyColumn: "engagement_id",
                keyValue: 1,
                column: "HandoverInfoHandoverId",
                value: null);

            migrationBuilder.UpdateData(
                table: "engagements",
                keyColumn: "engagement_id",
                keyValue: 2,
                column: "HandoverInfoHandoverId",
                value: null);

            migrationBuilder.InsertData(
                table: "handover_info",
                columns: new[] { "handover_id", "business_problem", "client_background", "client_sentiment", "created_at", "created_by", "deal_origin", "engagement_id", "status_id", "updated_at", "updated_by" },
                values: new object[] { 1L, "They need a modern core banking platform to support new digital initiatives and mobile apps.", "FinTech Solutions is a mid-sized financial institution expanding its digital footprint.", "Excited but cautious about delivery timelines due to past vendor failures.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Strategic Partner Referral", 1L, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null });

            migrationBuilder.InsertData(
                table: "commitments",
                columns: new[] { "commitment_id", "commitment_type_id", "created_at", "created_by", "description", "handover_id", "quantity", "updated_at", "updated_by" },
                values: new object[] { 1L, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Delivery of MVP banking module within 3 months of kickoff.", 1L, "1", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null });

            migrationBuilder.InsertData(
                table: "master_checklist_versions",
                columns: new[] { "id", "created_at", "created_by", "status_id", "updated_at", "updated_by", "version_number" },
                values: new object[,]
                {
                    { 1L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 0 },
                    { 2L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1 }
                });

            migrationBuilder.InsertData(
                table: "master_checklist_items",
                columns: new[] { "id", "category_id", "created_at", "created_by", "description", "display_order", "is_mandatory", "title", "updated_at", "updated_by", "version_id" },
                values: new object[,]
                {
                    { 1L, 1L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Confirm the signed commercial contract is on file before kickoff.", 1, true, "Sign commercial contract", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1L },
                    { 2L, 1L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Verify the approved budget matches the statement of work.", 2, true, "Confirm engagement budget", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1L },
                    { 3L, 1L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Agree invoicing milestones and payment terms with the client.", 3, true, "Confirm billing schedule", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1L },
                    { 4L, 2L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Sales team walks delivery team through client history and expectations.", 4, true, "Complete sales-to-delivery handover call", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1L },
                    { 5L, 2L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Ensure delivery has access to all pre-sales notes and correspondence.", 5, true, "Share CRM opportunity notes", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1L },
                    { 6L, 2L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Confirm delivery team has contact details for all client stakeholders.", 6, true, "Transfer key client contacts", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1L },
                    { 7L, 3L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Capture in-scope and out-of-scope items in the kickoff scope document.", 7, true, "Document engagement scope", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1L },
                    { 8L, 3L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "List and confirm all deliverables committed to the client.", 8, true, "Confirm deliverables list", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1L }
                });

            migrationBuilder.InsertData(
                table: "master_checklist_items",
                columns: new[] { "id", "category_id", "created_at", "created_by", "description", "display_order", "title", "updated_at", "updated_by", "version_id" },
                values: new object[] { 9L, 3L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Record assumptions made during the sales cycle that affect scope.", 9, "Identify scope assumptions and exclusions", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1L });

            migrationBuilder.InsertData(
                table: "master_checklist_items",
                columns: new[] { "id", "category_id", "created_at", "created_by", "description", "display_order", "is_mandatory", "title", "updated_at", "updated_by", "version_id" },
                values: new object[,]
                {
                    { 10L, 4L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Agree recurring status meetings and escalation paths.", 10, true, "Set up engagement governance cadence", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1L },
                    { 11L, 4L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Identify decision-makers on both client and delivery sides.", 11, true, "Assign engagement steering committee", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1L },
                    { 12L, 4L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Agree how scope changes will be raised, approved, and tracked.", 12, true, "Define change control process", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1L }
                });

            migrationBuilder.InsertData(
                table: "master_checklist_items",
                columns: new[] { "id", "category_id", "created_at", "created_by", "description", "display_order", "title", "updated_at", "updated_by", "version_id" },
                values: new object[] { 13L, 4L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Assign an owner to maintain the Risks, Assumptions, Issues, and Dependencies log.", 13, "Confirm RAID log owner", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1L });

            migrationBuilder.InsertData(
                table: "master_checklist_items",
                columns: new[] { "id", "category_id", "created_at", "created_by", "description", "display_order", "is_mandatory", "title", "updated_at", "updated_by", "version_id" },
                values: new object[,]
                {
                    { 14L, 5L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Confirm named resources are assigned and available for the engagement.", 14, true, "Allocate delivery team members", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1L },
                    { 15L, 5L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Check assigned resources have capacity for the engagement duration.", 15, true, "Confirm resource availability calendar", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1L }
                });

            migrationBuilder.InsertData(
                table: "master_checklist_items",
                columns: new[] { "id", "category_id", "created_at", "created_by", "description", "display_order", "title", "updated_at", "updated_by", "version_id" },
                values: new object[] { 16L, 5L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Name backup resources in case of absence during the engagement.", 16, "Identify backup/on-call resources", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1L });

            migrationBuilder.InsertData(
                table: "master_checklist_items",
                columns: new[] { "id", "category_id", "created_at", "created_by", "description", "display_order", "is_mandatory", "title", "updated_at", "updated_by", "version_id" },
                values: new object[,]
                {
                    { 17L, 6L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Request and confirm access to required client systems and environments.", 17, true, "Provision environment access", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1L },
                    { 18L, 6L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Confirm client infrastructure and tooling meet engagement requirements.", 18, true, "Verify technical prerequisites", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1L },
                    { 19L, 6L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Ensure delivery team has access to repositories and pipelines.", 19, true, "Set up source control and CI/CD access", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1L }
                });

            migrationBuilder.InsertData(
                table: "master_checklist_items",
                columns: new[] { "id", "category_id", "created_at", "created_by", "description", "display_order", "title", "updated_at", "updated_by", "version_id" },
                values: new object[] { 20L, 6L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Review any client-specific security or compliance obligations.", 20, "Confirm security and compliance requirements", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1L });

            migrationBuilder.InsertData(
                table: "master_checklist_items",
                columns: new[] { "id", "category_id", "created_at", "created_by", "description", "display_order", "is_mandatory", "title", "updated_at", "updated_by", "version_id" },
                values: new object[,]
                {
                    { 21L, 1L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Confirm the signed commercial contract is on file before kickoff.", 1, true, "Sign commercial contract", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2L },
                    { 22L, 1L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Verify the approved budget matches the statement of work.", 2, true, "Confirm engagement budget", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2L },
                    { 23L, 1L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Agree invoicing milestones and payment terms with the client.", 3, true, "Confirm billing schedule", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2L },
                    { 24L, 2L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Sales team walks delivery team through client history and expectations.", 4, true, "Complete sales-to-delivery handover call", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2L },
                    { 25L, 2L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Ensure delivery has access to all pre-sales notes and correspondence.", 5, true, "Share CRM opportunity notes", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2L },
                    { 26L, 2L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Confirm delivery team has contact details for all client stakeholders.", 6, true, "Transfer key client contacts", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2L },
                    { 27L, 3L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Capture in-scope and out-of-scope items in the kickoff scope document.", 7, true, "Document engagement scope", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2L },
                    { 28L, 3L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "List and confirm all deliverables committed to the client.", 8, true, "Confirm deliverables list", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2L }
                });

            migrationBuilder.InsertData(
                table: "master_checklist_items",
                columns: new[] { "id", "category_id", "created_at", "created_by", "description", "display_order", "title", "updated_at", "updated_by", "version_id" },
                values: new object[] { 29L, 3L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Record assumptions made during the sales cycle that affect scope.", 9, "Identify scope assumptions and exclusions", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2L });

            migrationBuilder.InsertData(
                table: "master_checklist_items",
                columns: new[] { "id", "category_id", "created_at", "created_by", "description", "display_order", "is_mandatory", "title", "updated_at", "updated_by", "version_id" },
                values: new object[,]
                {
                    { 30L, 4L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Agree recurring status meetings and escalation paths.", 10, true, "Set up engagement governance cadence", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2L },
                    { 31L, 4L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Identify decision-makers on both client and delivery sides.", 11, true, "Assign engagement steering committee", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2L },
                    { 32L, 4L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Agree how scope changes will be raised, approved, and tracked.", 12, true, "Define change control process", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2L }
                });

            migrationBuilder.InsertData(
                table: "master_checklist_items",
                columns: new[] { "id", "category_id", "created_at", "created_by", "description", "display_order", "title", "updated_at", "updated_by", "version_id" },
                values: new object[] { 33L, 4L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Assign an owner to maintain the Risks, Assumptions, Issues, and Dependencies log.", 13, "Confirm RAID log owner", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2L });

            migrationBuilder.InsertData(
                table: "master_checklist_items",
                columns: new[] { "id", "category_id", "created_at", "created_by", "description", "display_order", "is_mandatory", "title", "updated_at", "updated_by", "version_id" },
                values: new object[,]
                {
                    { 34L, 5L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Confirm named resources are assigned and available for the engagement.", 14, true, "Allocate delivery team members", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2L },
                    { 35L, 5L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Check assigned resources have capacity for the engagement duration.", 15, true, "Confirm resource availability calendar", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2L }
                });

            migrationBuilder.InsertData(
                table: "master_checklist_items",
                columns: new[] { "id", "category_id", "created_at", "created_by", "description", "display_order", "title", "updated_at", "updated_by", "version_id" },
                values: new object[] { 36L, 5L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Name backup resources in case of absence during the engagement.", 16, "Identify backup/on-call resources", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2L });

            migrationBuilder.InsertData(
                table: "master_checklist_items",
                columns: new[] { "id", "category_id", "created_at", "created_by", "description", "display_order", "is_mandatory", "title", "updated_at", "updated_by", "version_id" },
                values: new object[,]
                {
                    { 37L, 6L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Request and confirm access to required client systems and environments.", 17, true, "Provision environment access", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2L },
                    { 38L, 6L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Confirm client infrastructure and tooling meet engagement requirements.", 18, true, "Verify technical prerequisites", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2L },
                    { 39L, 6L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Ensure delivery team has access to repositories and pipelines.", 19, true, "Set up source control and CI/CD access", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2L }
                });

            migrationBuilder.InsertData(
                table: "master_checklist_items",
                columns: new[] { "id", "category_id", "created_at", "created_by", "description", "display_order", "title", "updated_at", "updated_by", "version_id" },
                values: new object[] { 40L, 6L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Review any client-specific security or compliance obligations.", 20, "Confirm security and compliance requirements", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2L });

            migrationBuilder.CreateIndex(
                name: "IX_engagements_HandoverInfoHandoverId",
                table: "engagements",
                column: "HandoverInfoHandoverId");

            migrationBuilder.AddForeignKey(
                name: "FK_engagements_handover_info_HandoverInfoHandoverId",
                table: "engagements",
                column: "HandoverInfoHandoverId",
                principalTable: "handover_info",
                principalColumn: "handover_id");
        }
    }
}
