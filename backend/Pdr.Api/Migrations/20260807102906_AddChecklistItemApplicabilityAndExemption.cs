using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pdr.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddChecklistItemApplicabilityAndExemption : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "exemption_rejection_reason",
                table: "engagement_checklist_items",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "exemption_requested_at",
                table: "engagement_checklist_items",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "exemption_requested_by_id",
                table: "engagement_checklist_items",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "exemption_reviewed_at",
                table: "engagement_checklist_items",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "exemption_reviewed_by_id",
                table: "engagement_checklist_items",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "exemption_status",
                table: "engagement_checklist_items",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "None");

            migrationBuilder.AddColumn<string>(
                name: "not_applicable_reason",
                table: "engagement_checklist_items",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 1,
                columns: new[] { "exemption_rejection_reason", "exemption_requested_at", "exemption_requested_by_id", "exemption_reviewed_at", "exemption_reviewed_by_id", "not_applicable_reason" },
                values: new object[] { null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 2,
                columns: new[] { "exemption_rejection_reason", "exemption_requested_at", "exemption_requested_by_id", "exemption_reviewed_at", "exemption_reviewed_by_id", "not_applicable_reason" },
                values: new object[] { null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 3,
                columns: new[] { "exemption_rejection_reason", "exemption_requested_at", "exemption_requested_by_id", "exemption_reviewed_at", "exemption_reviewed_by_id", "not_applicable_reason" },
                values: new object[] { null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 4,
                columns: new[] { "exemption_rejection_reason", "exemption_requested_at", "exemption_requested_by_id", "exemption_reviewed_at", "exemption_reviewed_by_id", "not_applicable_reason" },
                values: new object[] { null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 5,
                columns: new[] { "exemption_rejection_reason", "exemption_requested_at", "exemption_requested_by_id", "exemption_reviewed_at", "exemption_reviewed_by_id", "not_applicable_reason" },
                values: new object[] { null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 6,
                columns: new[] { "exemption_rejection_reason", "exemption_requested_at", "exemption_requested_by_id", "exemption_reviewed_at", "exemption_reviewed_by_id", "not_applicable_reason" },
                values: new object[] { null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 7,
                columns: new[] { "exemption_rejection_reason", "exemption_requested_at", "exemption_requested_by_id", "exemption_reviewed_at", "exemption_reviewed_by_id", "not_applicable_reason" },
                values: new object[] { null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 8,
                columns: new[] { "exemption_rejection_reason", "exemption_requested_at", "exemption_requested_by_id", "exemption_reviewed_at", "exemption_reviewed_by_id", "not_applicable_reason" },
                values: new object[] { null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 9,
                columns: new[] { "exemption_rejection_reason", "exemption_requested_at", "exemption_requested_by_id", "exemption_reviewed_at", "exemption_reviewed_by_id", "not_applicable_reason" },
                values: new object[] { null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 10,
                columns: new[] { "exemption_rejection_reason", "exemption_requested_at", "exemption_requested_by_id", "exemption_reviewed_at", "exemption_reviewed_by_id", "not_applicable_reason" },
                values: new object[] { null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 11,
                columns: new[] { "exemption_rejection_reason", "exemption_requested_at", "exemption_requested_by_id", "exemption_reviewed_at", "exemption_reviewed_by_id", "not_applicable_reason" },
                values: new object[] { null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 12,
                columns: new[] { "exemption_rejection_reason", "exemption_requested_at", "exemption_requested_by_id", "exemption_reviewed_at", "exemption_reviewed_by_id", "not_applicable_reason" },
                values: new object[] { null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 13,
                columns: new[] { "exemption_rejection_reason", "exemption_requested_at", "exemption_requested_by_id", "exemption_reviewed_at", "exemption_reviewed_by_id", "not_applicable_reason" },
                values: new object[] { null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 14,
                columns: new[] { "exemption_rejection_reason", "exemption_requested_at", "exemption_requested_by_id", "exemption_reviewed_at", "exemption_reviewed_by_id", "not_applicable_reason" },
                values: new object[] { null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 15,
                columns: new[] { "exemption_rejection_reason", "exemption_requested_at", "exemption_requested_by_id", "exemption_reviewed_at", "exemption_reviewed_by_id", "not_applicable_reason" },
                values: new object[] { null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 16,
                columns: new[] { "exemption_rejection_reason", "exemption_requested_at", "exemption_requested_by_id", "exemption_reviewed_at", "exemption_reviewed_by_id", "not_applicable_reason" },
                values: new object[] { null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 17,
                columns: new[] { "exemption_rejection_reason", "exemption_requested_at", "exemption_requested_by_id", "exemption_reviewed_at", "exemption_reviewed_by_id", "not_applicable_reason" },
                values: new object[] { null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 18,
                columns: new[] { "exemption_rejection_reason", "exemption_requested_at", "exemption_requested_by_id", "exemption_reviewed_at", "exemption_reviewed_by_id", "not_applicable_reason" },
                values: new object[] { null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 19,
                columns: new[] { "exemption_rejection_reason", "exemption_requested_at", "exemption_requested_by_id", "exemption_reviewed_at", "exemption_reviewed_by_id", "not_applicable_reason" },
                values: new object[] { null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 20,
                columns: new[] { "exemption_rejection_reason", "exemption_requested_at", "exemption_requested_by_id", "exemption_reviewed_at", "exemption_reviewed_by_id", "not_applicable_reason" },
                values: new object[] { null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 21,
                columns: new[] { "exemption_rejection_reason", "exemption_requested_at", "exemption_requested_by_id", "exemption_reviewed_at", "exemption_reviewed_by_id", "not_applicable_reason" },
                values: new object[] { null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 22,
                columns: new[] { "exemption_rejection_reason", "exemption_requested_at", "exemption_requested_by_id", "exemption_reviewed_at", "exemption_reviewed_by_id", "not_applicable_reason" },
                values: new object[] { null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 23,
                columns: new[] { "exemption_rejection_reason", "exemption_requested_at", "exemption_requested_by_id", "exemption_reviewed_at", "exemption_reviewed_by_id", "not_applicable_reason" },
                values: new object[] { null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 24,
                columns: new[] { "exemption_rejection_reason", "exemption_requested_at", "exemption_requested_by_id", "exemption_reviewed_at", "exemption_reviewed_by_id", "not_applicable_reason" },
                values: new object[] { null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 25,
                columns: new[] { "exemption_rejection_reason", "exemption_requested_at", "exemption_requested_by_id", "exemption_reviewed_at", "exemption_reviewed_by_id", "not_applicable_reason" },
                values: new object[] { null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 26,
                columns: new[] { "exemption_rejection_reason", "exemption_requested_at", "exemption_requested_by_id", "exemption_reviewed_at", "exemption_reviewed_by_id", "not_applicable_reason" },
                values: new object[] { null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 27,
                columns: new[] { "exemption_rejection_reason", "exemption_requested_at", "exemption_requested_by_id", "exemption_reviewed_at", "exemption_reviewed_by_id", "not_applicable_reason" },
                values: new object[] { null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 28,
                columns: new[] { "exemption_rejection_reason", "exemption_requested_at", "exemption_requested_by_id", "exemption_reviewed_at", "exemption_reviewed_by_id", "not_applicable_reason" },
                values: new object[] { null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 29,
                columns: new[] { "exemption_rejection_reason", "exemption_requested_at", "exemption_requested_by_id", "exemption_reviewed_at", "exemption_reviewed_by_id", "not_applicable_reason" },
                values: new object[] { null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 30,
                columns: new[] { "exemption_rejection_reason", "exemption_requested_at", "exemption_requested_by_id", "exemption_reviewed_at", "exemption_reviewed_by_id", "not_applicable_reason" },
                values: new object[] { null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 31,
                columns: new[] { "exemption_rejection_reason", "exemption_requested_at", "exemption_requested_by_id", "exemption_reviewed_at", "exemption_reviewed_by_id", "not_applicable_reason" },
                values: new object[] { null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 32,
                columns: new[] { "exemption_rejection_reason", "exemption_requested_at", "exemption_requested_by_id", "exemption_reviewed_at", "exemption_reviewed_by_id", "not_applicable_reason" },
                values: new object[] { null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 33,
                columns: new[] { "exemption_rejection_reason", "exemption_requested_at", "exemption_requested_by_id", "exemption_reviewed_at", "exemption_reviewed_by_id", "not_applicable_reason" },
                values: new object[] { null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 34,
                columns: new[] { "exemption_rejection_reason", "exemption_requested_at", "exemption_requested_by_id", "exemption_reviewed_at", "exemption_reviewed_by_id", "not_applicable_reason" },
                values: new object[] { null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 35,
                columns: new[] { "exemption_rejection_reason", "exemption_requested_at", "exemption_requested_by_id", "exemption_reviewed_at", "exemption_reviewed_by_id", "not_applicable_reason" },
                values: new object[] { null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 36,
                columns: new[] { "exemption_rejection_reason", "exemption_requested_at", "exemption_requested_by_id", "exemption_reviewed_at", "exemption_reviewed_by_id", "not_applicable_reason" },
                values: new object[] { null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 37,
                columns: new[] { "exemption_rejection_reason", "exemption_requested_at", "exemption_requested_by_id", "exemption_reviewed_at", "exemption_reviewed_by_id", "not_applicable_reason" },
                values: new object[] { null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 38,
                columns: new[] { "exemption_rejection_reason", "exemption_requested_at", "exemption_requested_by_id", "exemption_reviewed_at", "exemption_reviewed_by_id", "not_applicable_reason" },
                values: new object[] { null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 39,
                columns: new[] { "exemption_rejection_reason", "exemption_requested_at", "exemption_requested_by_id", "exemption_reviewed_at", "exemption_reviewed_by_id", "not_applicable_reason" },
                values: new object[] { null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 40,
                columns: new[] { "exemption_rejection_reason", "exemption_requested_at", "exemption_requested_by_id", "exemption_reviewed_at", "exemption_reviewed_by_id", "not_applicable_reason" },
                values: new object[] { null, null, null, null, null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "exemption_rejection_reason",
                table: "engagement_checklist_items");

            migrationBuilder.DropColumn(
                name: "exemption_requested_at",
                table: "engagement_checklist_items");

            migrationBuilder.DropColumn(
                name: "exemption_requested_by_id",
                table: "engagement_checklist_items");

            migrationBuilder.DropColumn(
                name: "exemption_reviewed_at",
                table: "engagement_checklist_items");

            migrationBuilder.DropColumn(
                name: "exemption_reviewed_by_id",
                table: "engagement_checklist_items");

            migrationBuilder.DropColumn(
                name: "exemption_status",
                table: "engagement_checklist_items");

            migrationBuilder.DropColumn(
                name: "not_applicable_reason",
                table: "engagement_checklist_items");
        }
    }
}
