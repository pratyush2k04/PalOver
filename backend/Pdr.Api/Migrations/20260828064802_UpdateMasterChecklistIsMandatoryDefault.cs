using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pdr.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMasterChecklistIsMandatoryDefault : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "is_mandatory",
                table: "master_checklist_items",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: true);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 2,
                column: "is_mandatory",
                value: false);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 3,
                column: "is_mandatory",
                value: false);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 5,
                column: "is_mandatory",
                value: false);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 6,
                column: "is_mandatory",
                value: false);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 8,
                column: "is_mandatory",
                value: false);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 9,
                column: "is_mandatory",
                value: false);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 10,
                column: "is_mandatory",
                value: false);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 11,
                column: "is_mandatory",
                value: false);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 12,
                column: "is_mandatory",
                value: false);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 13,
                column: "is_mandatory",
                value: false);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 14,
                column: "is_mandatory",
                value: false);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 15,
                column: "is_mandatory",
                value: false);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 16,
                column: "is_mandatory",
                value: false);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 18,
                column: "is_mandatory",
                value: false);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 19,
                column: "is_mandatory",
                value: false);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 20,
                column: "is_mandatory",
                value: false);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 22,
                column: "is_mandatory",
                value: false);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 23,
                column: "is_mandatory",
                value: false);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 25,
                column: "is_mandatory",
                value: false);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 26,
                column: "is_mandatory",
                value: false);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 28,
                column: "is_mandatory",
                value: false);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 29,
                column: "is_mandatory",
                value: false);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 30,
                column: "is_mandatory",
                value: false);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 31,
                column: "is_mandatory",
                value: false);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 32,
                column: "is_mandatory",
                value: false);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 33,
                column: "is_mandatory",
                value: false);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 34,
                column: "is_mandatory",
                value: false);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 35,
                column: "is_mandatory",
                value: false);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 36,
                column: "is_mandatory",
                value: false);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 38,
                column: "is_mandatory",
                value: false);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 39,
                column: "is_mandatory",
                value: false);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 40,
                column: "is_mandatory",
                value: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "is_mandatory",
                table: "master_checklist_items",
                type: "boolean",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 2,
                column: "is_mandatory",
                value: true);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 3,
                column: "is_mandatory",
                value: true);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 5,
                column: "is_mandatory",
                value: true);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 6,
                column: "is_mandatory",
                value: true);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 8,
                column: "is_mandatory",
                value: true);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 9,
                column: "is_mandatory",
                value: true);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 10,
                column: "is_mandatory",
                value: true);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 11,
                column: "is_mandatory",
                value: true);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 12,
                column: "is_mandatory",
                value: true);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 13,
                column: "is_mandatory",
                value: true);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 14,
                column: "is_mandatory",
                value: true);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 15,
                column: "is_mandatory",
                value: true);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 16,
                column: "is_mandatory",
                value: true);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 18,
                column: "is_mandatory",
                value: true);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 19,
                column: "is_mandatory",
                value: true);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 20,
                column: "is_mandatory",
                value: true);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 22,
                column: "is_mandatory",
                value: true);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 23,
                column: "is_mandatory",
                value: true);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 25,
                column: "is_mandatory",
                value: true);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 26,
                column: "is_mandatory",
                value: true);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 28,
                column: "is_mandatory",
                value: true);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 29,
                column: "is_mandatory",
                value: true);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 30,
                column: "is_mandatory",
                value: true);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 31,
                column: "is_mandatory",
                value: true);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 32,
                column: "is_mandatory",
                value: true);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 33,
                column: "is_mandatory",
                value: true);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 34,
                column: "is_mandatory",
                value: true);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 35,
                column: "is_mandatory",
                value: true);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 36,
                column: "is_mandatory",
                value: true);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 38,
                column: "is_mandatory",
                value: true);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 39,
                column: "is_mandatory",
                value: true);

            migrationBuilder.UpdateData(
                table: "master_checklist_items",
                keyColumn: "id",
                keyValue: 40,
                column: "is_mandatory",
                value: true);
        }
    }
}
