using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pdr.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddEvidenceRequiredToMasterChecklistItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "evidence_required",
                table: "master_checklist_items",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 1,
                column: "evidence_required",
                value: true);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 2,
                column: "evidence_required",
                value: true);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 3,
                column: "evidence_required",
                value: true);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 4,
                column: "evidence_required",
                value: true);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 5,
                column: "evidence_required",
                value: true);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 6,
                column: "evidence_required",
                value: true);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 7,
                column: "evidence_required",
                value: true);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 8,
                column: "evidence_required",
                value: true);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 9,
                column: "evidence_required",
                value: true);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 10,
                column: "evidence_required",
                value: true);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 11,
                column: "evidence_required",
                value: true);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 12,
                column: "evidence_required",
                value: true);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 13,
                column: "evidence_required",
                value: true);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 14,
                column: "evidence_required",
                value: true);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 15,
                column: "evidence_required",
                value: true);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 16,
                column: "evidence_required",
                value: true);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 17,
                column: "evidence_required",
                value: true);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 18,
                column: "evidence_required",
                value: true);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 19,
                column: "evidence_required",
                value: true);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 20,
                column: "evidence_required",
                value: true);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 21,
                column: "evidence_required",
                value: true);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 22,
                column: "evidence_required",
                value: true);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 23,
                column: "evidence_required",
                value: true);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 24,
                column: "evidence_required",
                value: true);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 25,
                column: "evidence_required",
                value: true);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 26,
                column: "evidence_required",
                value: true);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 27,
                column: "evidence_required",
                value: true);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 28,
                column: "evidence_required",
                value: true);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 29,
                column: "evidence_required",
                value: true);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 30,
                column: "evidence_required",
                value: true);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 31,
                column: "evidence_required",
                value: true);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 32,
                column: "evidence_required",
                value: true);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 33,
                column: "evidence_required",
                value: true);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 34,
                column: "evidence_required",
                value: true);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 35,
                column: "evidence_required",
                value: true);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 36,
                column: "evidence_required",
                value: true);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 37,
                column: "evidence_required",
                value: true);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 38,
                column: "evidence_required",
                value: true);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 39,
                column: "evidence_required",
                value: true);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 40,
                column: "evidence_required",
                value: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "evidence_required",
                table: "master_checklist_items");
        }
    }
}
