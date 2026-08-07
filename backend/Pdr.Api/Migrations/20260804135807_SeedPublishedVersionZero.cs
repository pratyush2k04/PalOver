using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Pdr.Api.Migrations
{
    /// <inheritdoc />
    public partial class SeedPublishedVersionZero : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "master_checklist_versions",
                keyColumn: "id",
                keyValue: 1L,
                columns: new[] { "status_id", "version_number" },
                values: new object[] { 2L, 0 });

            migrationBuilder.InsertData(
                table: "master_checklist_versions",
                columns: new[] { "id", "created_at", "created_by", "status_id", "updated_at", "updated_by", "version_number" },
                values: new object[] { 2L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1 });

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
                table: "master_checklist_versions",
                keyColumn: "id",
                keyValue: 2L);

            migrationBuilder.UpdateData(
                table: "master_checklist_versions",
                keyColumn: "id",
                keyValue: 1L,
                columns: new[] { "status_id", "version_number" },
                values: new object[] { 1L, 1 });
        }
    }
}
