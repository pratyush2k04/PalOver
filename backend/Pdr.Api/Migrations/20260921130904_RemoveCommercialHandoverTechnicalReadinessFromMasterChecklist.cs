using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Pdr.Api.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCommercialHandoverTechnicalReadinessFromMasterChecklist : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                keyValue: 6);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 7,
                column: "display_order",
                value: 1);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 8,
                column: "display_order",
                value: 2);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 9,
                column: "display_order",
                value: 3);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 10,
                column: "display_order",
                value: 4);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 11,
                column: "display_order",
                value: 5);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 12,
                column: "display_order",
                value: 6);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 13,
                column: "display_order",
                value: 7);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 14,
                column: "display_order",
                value: 8);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 15,
                column: "display_order",
                value: 9);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 16,
                column: "display_order",
                value: 10);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 27,
                column: "display_order",
                value: 1);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 28,
                column: "display_order",
                value: 2);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 29,
                column: "display_order",
                value: 3);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 30,
                column: "display_order",
                value: 4);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 31,
                column: "display_order",
                value: 5);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 32,
                column: "display_order",
                value: 6);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 33,
                column: "display_order",
                value: 7);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 34,
                column: "display_order",
                value: 8);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 35,
                column: "display_order",
                value: 9);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 36,
                column: "display_order",
                value: 10);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "checklist_categories",
                columns: new[] { "id", "category_name" },
                values: new object[,]
                {
                    { 1, "Commercial" },
                    { 2, "Handover" },
                    { 6, "Technical Readiness" }
                });

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 7,
                column: "display_order",
                value: 7);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 8,
                column: "display_order",
                value: 8);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 9,
                column: "display_order",
                value: 9);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 10,
                column: "display_order",
                value: 10);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 11,
                column: "display_order",
                value: 11);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 12,
                column: "display_order",
                value: 12);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 13,
                column: "display_order",
                value: 13);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 14,
                column: "display_order",
                value: 14);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 15,
                column: "display_order",
                value: 15);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 16,
                column: "display_order",
                value: 16);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 27,
                column: "display_order",
                value: 7);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 28,
                column: "display_order",
                value: 8);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 29,
                column: "display_order",
                value: 9);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 30,
                column: "display_order",
                value: 10);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 31,
                column: "display_order",
                value: 11);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 32,
                column: "display_order",
                value: 12);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 33,
                column: "display_order",
                value: 13);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 34,
                column: "display_order",
                value: 14);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 35,
                column: "display_order",
                value: 15);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 36,
                column: "display_order",
                value: 16);

            migrationBuilder.InsertData(
                table: "master_checklist_items",
                columns: new[] { "id", "category_id", "created_at", "created_by", "description", "display_order", "evidence_required", "is_mandatory", "title", "updated_at", "updated_by", "version_id" },
                values: new object[] { 1, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Confirm the signed commercial contract is on file before kickoff.", 1, true, true, "Sign commercial contract", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1 });

            migrationBuilder.InsertData(
                table: "master_checklist_items",
                columns: new[] { "id", "category_id", "created_at", "created_by", "description", "display_order", "title", "updated_at", "updated_by", "version_id" },
                values: new object[,]
                {
                    { 2, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Verify the approved budget matches the statement of work.", 2, "Confirm engagement budget", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1 },
                    { 3, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Agree invoicing milestones and payment terms with the client.", 3, "Confirm billing schedule", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1 }
                });

            migrationBuilder.InsertData(
                table: "master_checklist_items",
                columns: new[] { "id", "category_id", "created_at", "created_by", "description", "display_order", "evidence_required", "is_mandatory", "title", "updated_at", "updated_by", "version_id" },
                values: new object[] { 4, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Sales team walks delivery team through client history and expectations.", 4, true, true, "Complete sales-to-delivery handover call", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1 });

            migrationBuilder.InsertData(
                table: "master_checklist_items",
                columns: new[] { "id", "category_id", "created_at", "created_by", "description", "display_order", "title", "updated_at", "updated_by", "version_id" },
                values: new object[,]
                {
                    { 5, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Ensure delivery has access to all pre-sales notes and correspondence.", 5, "Share CRM opportunity notes", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1 },
                    { 6, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Confirm delivery team has contact details for all client stakeholders.", 6, "Transfer key client contacts", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1 }
                });

            migrationBuilder.InsertData(
                table: "master_checklist_items",
                columns: new[] { "id", "category_id", "created_at", "created_by", "description", "display_order", "evidence_required", "is_mandatory", "title", "updated_at", "updated_by", "version_id" },
                values: new object[] { 17, 6, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Request and confirm access to required client systems and environments.", 17, true, true, "Provision environment access", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1 });

            migrationBuilder.InsertData(
                table: "master_checklist_items",
                columns: new[] { "id", "category_id", "created_at", "created_by", "description", "display_order", "title", "updated_at", "updated_by", "version_id" },
                values: new object[,]
                {
                    { 18, 6, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Confirm client infrastructure and tooling meet engagement requirements.", 18, "Verify technical prerequisites", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1 },
                    { 19, 6, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Ensure delivery team has access to repositories and pipelines.", 19, "Set up source control and CI/CD access", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1 },
                    { 20, 6, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Review any client-specific security or compliance obligations.", 20, "Confirm security and compliance requirements", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1 }
                });

            migrationBuilder.InsertData(
                table: "master_checklist_items",
                columns: new[] { "id", "category_id", "created_at", "created_by", "description", "display_order", "evidence_required", "is_mandatory", "title", "updated_at", "updated_by", "version_id" },
                values: new object[] { 21, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Confirm the signed commercial contract is on file before kickoff.", 1, true, true, "Sign commercial contract", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2 });

            migrationBuilder.InsertData(
                table: "master_checklist_items",
                columns: new[] { "id", "category_id", "created_at", "created_by", "description", "display_order", "title", "updated_at", "updated_by", "version_id" },
                values: new object[,]
                {
                    { 22, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Verify the approved budget matches the statement of work.", 2, "Confirm engagement budget", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2 },
                    { 23, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Agree invoicing milestones and payment terms with the client.", 3, "Confirm billing schedule", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2 }
                });

            migrationBuilder.InsertData(
                table: "master_checklist_items",
                columns: new[] { "id", "category_id", "created_at", "created_by", "description", "display_order", "evidence_required", "is_mandatory", "title", "updated_at", "updated_by", "version_id" },
                values: new object[] { 24, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Sales team walks delivery team through client history and expectations.", 4, true, true, "Complete sales-to-delivery handover call", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2 });

            migrationBuilder.InsertData(
                table: "master_checklist_items",
                columns: new[] { "id", "category_id", "created_at", "created_by", "description", "display_order", "title", "updated_at", "updated_by", "version_id" },
                values: new object[,]
                {
                    { 25, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Ensure delivery has access to all pre-sales notes and correspondence.", 5, "Share CRM opportunity notes", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2 },
                    { 26, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Confirm delivery team has contact details for all client stakeholders.", 6, "Transfer key client contacts", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2 }
                });

            migrationBuilder.InsertData(
                table: "master_checklist_items",
                columns: new[] { "id", "category_id", "created_at", "created_by", "description", "display_order", "evidence_required", "is_mandatory", "title", "updated_at", "updated_by", "version_id" },
                values: new object[] { 37, 6, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Request and confirm access to required client systems and environments.", 17, true, true, "Provision environment access", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2 });

            migrationBuilder.InsertData(
                table: "master_checklist_items",
                columns: new[] { "id", "category_id", "created_at", "created_by", "description", "display_order", "title", "updated_at", "updated_by", "version_id" },
                values: new object[,]
                {
                    { 38, 6, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Confirm client infrastructure and tooling meet engagement requirements.", 18, "Verify technical prerequisites", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2 },
                    { 39, 6, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Ensure delivery team has access to repositories and pipelines.", 19, "Set up source control and CI/CD access", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2 },
                    { 40, 6, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Review any client-specific security or compliance obligations.", 20, "Confirm security and compliance requirements", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2 }
                });
        }
    }
}
