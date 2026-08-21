using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pdr.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddChecklistItemEvidence : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "evidence_link",
                table: "engagement_checklist_items",
                type: "character varying(2048)",
                maxLength: 2048,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "evidence_note",
                table: "engagement_checklist_items",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "evidence_required",
                table: "engagement_checklist_items",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "evidence_submitted_at",
                table: "engagement_checklist_items",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "evidence_submitted_by_id",
                table: "engagement_checklist_items",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EngagementId",
                table: "audit_logs",
                type: "integer",
                nullable: true);

            migrationBuilder.InsertData(
                table: "audit_actions",
                columns: new[] { "Id", "Action", "Details" },
                values: new object[] { 21, "ChecklistItemEvidenceUpdated", "Evidence updated for checklist item '{itemTitle}' on engagement '{engagementName}' by {actorName}" });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 1,
                columns: new[] { "evidence_link", "evidence_note", "evidence_required", "evidence_submitted_at", "evidence_submitted_by_id" },
                values: new object[] { null, null, true, null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 2,
                columns: new[] { "evidence_link", "evidence_note", "evidence_required", "evidence_submitted_at", "evidence_submitted_by_id" },
                values: new object[] { null, null, true, null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 3,
                columns: new[] { "evidence_link", "evidence_note", "evidence_required", "evidence_submitted_at", "evidence_submitted_by_id" },
                values: new object[] { null, null, true, null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 4,
                columns: new[] { "evidence_link", "evidence_note", "evidence_required", "evidence_submitted_at", "evidence_submitted_by_id" },
                values: new object[] { null, null, true, null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 5,
                columns: new[] { "evidence_link", "evidence_note", "evidence_required", "evidence_submitted_at", "evidence_submitted_by_id" },
                values: new object[] { null, null, true, null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 6,
                columns: new[] { "evidence_link", "evidence_note", "evidence_required", "evidence_submitted_at", "evidence_submitted_by_id" },
                values: new object[] { null, null, true, null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 7,
                columns: new[] { "evidence_link", "evidence_note", "evidence_required", "evidence_submitted_at", "evidence_submitted_by_id" },
                values: new object[] { null, null, true, null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 8,
                columns: new[] { "evidence_link", "evidence_note", "evidence_required", "evidence_submitted_at", "evidence_submitted_by_id" },
                values: new object[] { null, null, true, null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 9,
                columns: new[] { "evidence_link", "evidence_note", "evidence_required", "evidence_submitted_at", "evidence_submitted_by_id" },
                values: new object[] { null, null, true, null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 10,
                columns: new[] { "evidence_link", "evidence_note", "evidence_required", "evidence_submitted_at", "evidence_submitted_by_id" },
                values: new object[] { null, null, true, null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 11,
                columns: new[] { "evidence_link", "evidence_note", "evidence_required", "evidence_submitted_at", "evidence_submitted_by_id" },
                values: new object[] { null, null, true, null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 12,
                columns: new[] { "evidence_link", "evidence_note", "evidence_required", "evidence_submitted_at", "evidence_submitted_by_id" },
                values: new object[] { null, null, true, null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 13,
                columns: new[] { "evidence_link", "evidence_note", "evidence_required", "evidence_submitted_at", "evidence_submitted_by_id" },
                values: new object[] { null, null, true, null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 14,
                columns: new[] { "evidence_link", "evidence_note", "evidence_required", "evidence_submitted_at", "evidence_submitted_by_id" },
                values: new object[] { null, null, true, null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 15,
                columns: new[] { "evidence_link", "evidence_note", "evidence_required", "evidence_submitted_at", "evidence_submitted_by_id" },
                values: new object[] { null, null, true, null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 16,
                columns: new[] { "evidence_link", "evidence_note", "evidence_required", "evidence_submitted_at", "evidence_submitted_by_id" },
                values: new object[] { null, null, true, null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 17,
                columns: new[] { "evidence_link", "evidence_note", "evidence_required", "evidence_submitted_at", "evidence_submitted_by_id" },
                values: new object[] { null, null, true, null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 18,
                columns: new[] { "evidence_link", "evidence_note", "evidence_required", "evidence_submitted_at", "evidence_submitted_by_id" },
                values: new object[] { null, null, true, null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 19,
                columns: new[] { "evidence_link", "evidence_note", "evidence_required", "evidence_submitted_at", "evidence_submitted_by_id" },
                values: new object[] { null, null, true, null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 20,
                columns: new[] { "evidence_link", "evidence_note", "evidence_required", "evidence_submitted_at", "evidence_submitted_by_id" },
                values: new object[] { null, null, true, null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 21,
                columns: new[] { "evidence_link", "evidence_note", "evidence_required", "evidence_submitted_at", "evidence_submitted_by_id" },
                values: new object[] { null, null, true, null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 22,
                columns: new[] { "evidence_link", "evidence_note", "evidence_required", "evidence_submitted_at", "evidence_submitted_by_id" },
                values: new object[] { null, null, true, null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 23,
                columns: new[] { "evidence_link", "evidence_note", "evidence_required", "evidence_submitted_at", "evidence_submitted_by_id" },
                values: new object[] { null, null, true, null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 24,
                columns: new[] { "evidence_link", "evidence_note", "evidence_required", "evidence_submitted_at", "evidence_submitted_by_id" },
                values: new object[] { null, null, true, null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 25,
                columns: new[] { "evidence_link", "evidence_note", "evidence_required", "evidence_submitted_at", "evidence_submitted_by_id" },
                values: new object[] { null, null, true, null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 26,
                columns: new[] { "evidence_link", "evidence_note", "evidence_required", "evidence_submitted_at", "evidence_submitted_by_id" },
                values: new object[] { null, null, true, null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 27,
                columns: new[] { "evidence_link", "evidence_note", "evidence_required", "evidence_submitted_at", "evidence_submitted_by_id" },
                values: new object[] { null, null, true, null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 28,
                columns: new[] { "evidence_link", "evidence_note", "evidence_required", "evidence_submitted_at", "evidence_submitted_by_id" },
                values: new object[] { null, null, true, null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 29,
                columns: new[] { "evidence_link", "evidence_note", "evidence_required", "evidence_submitted_at", "evidence_submitted_by_id" },
                values: new object[] { null, null, true, null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 30,
                columns: new[] { "evidence_link", "evidence_note", "evidence_required", "evidence_submitted_at", "evidence_submitted_by_id" },
                values: new object[] { null, null, true, null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 31,
                columns: new[] { "evidence_link", "evidence_note", "evidence_required", "evidence_submitted_at", "evidence_submitted_by_id" },
                values: new object[] { null, null, true, null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 32,
                columns: new[] { "evidence_link", "evidence_note", "evidence_required", "evidence_submitted_at", "evidence_submitted_by_id" },
                values: new object[] { null, null, true, null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 33,
                columns: new[] { "evidence_link", "evidence_note", "evidence_required", "evidence_submitted_at", "evidence_submitted_by_id" },
                values: new object[] { null, null, true, null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 34,
                columns: new[] { "evidence_link", "evidence_note", "evidence_required", "evidence_submitted_at", "evidence_submitted_by_id" },
                values: new object[] { null, null, true, null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 35,
                columns: new[] { "evidence_link", "evidence_note", "evidence_required", "evidence_submitted_at", "evidence_submitted_by_id" },
                values: new object[] { null, null, true, null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 36,
                columns: new[] { "evidence_link", "evidence_note", "evidence_required", "evidence_submitted_at", "evidence_submitted_by_id" },
                values: new object[] { null, null, true, null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 37,
                columns: new[] { "evidence_link", "evidence_note", "evidence_required", "evidence_submitted_at", "evidence_submitted_by_id" },
                values: new object[] { null, null, true, null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 38,
                columns: new[] { "evidence_link", "evidence_note", "evidence_required", "evidence_submitted_at", "evidence_submitted_by_id" },
                values: new object[] { null, null, true, null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 39,
                columns: new[] { "evidence_link", "evidence_note", "evidence_required", "evidence_submitted_at", "evidence_submitted_by_id" },
                values: new object[] { null, null, true, null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 40,
                columns: new[] { "evidence_link", "evidence_note", "evidence_required", "evidence_submitted_at", "evidence_submitted_by_id" },
                values: new object[] { null, null, true, null, null });

            migrationBuilder.CreateIndex(
                name: "IX_engagement_checklist_items_evidence_submitted_by_id",
                table: "engagement_checklist_items",
                column: "evidence_submitted_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_audit_logs_EngagementId",
                table: "audit_logs",
                column: "EngagementId");

            migrationBuilder.AddForeignKey(
                name: "FK_engagement_checklist_items_users_evidence_submitted_by_id",
                table: "engagement_checklist_items",
                column: "evidence_submitted_by_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_engagement_checklist_items_users_evidence_submitted_by_id",
                table: "engagement_checklist_items");

            migrationBuilder.DropIndex(
                name: "IX_engagement_checklist_items_evidence_submitted_by_id",
                table: "engagement_checklist_items");

            migrationBuilder.DropIndex(
                name: "IX_audit_logs_EngagementId",
                table: "audit_logs");

            migrationBuilder.DeleteData(
                table: "audit_actions",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DropColumn(
                name: "evidence_link",
                table: "engagement_checklist_items");

            migrationBuilder.DropColumn(
                name: "evidence_note",
                table: "engagement_checklist_items");

            migrationBuilder.DropColumn(
                name: "evidence_required",
                table: "engagement_checklist_items");

            migrationBuilder.DropColumn(
                name: "evidence_submitted_at",
                table: "engagement_checklist_items");

            migrationBuilder.DropColumn(
                name: "evidence_submitted_by_id",
                table: "engagement_checklist_items");

            migrationBuilder.DropColumn(
                name: "EngagementId",
                table: "audit_logs");
        }
    }
}
