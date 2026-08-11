using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pdr.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddChecklistItemOwnerAndBlocking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "block_reason",
                table: "engagement_checklist_items",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "owner_id",
                table: "engagement_checklist_items",
                type: "integer",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 1,
                columns: new[] { "block_reason", "owner_id" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 2,
                columns: new[] { "block_reason", "owner_id" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 3,
                columns: new[] { "block_reason", "owner_id" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 4,
                columns: new[] { "block_reason", "owner_id" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 5,
                columns: new[] { "block_reason", "owner_id" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 6,
                columns: new[] { "block_reason", "owner_id" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 7,
                columns: new[] { "block_reason", "owner_id" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 8,
                columns: new[] { "block_reason", "owner_id" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 9,
                columns: new[] { "block_reason", "owner_id" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 10,
                columns: new[] { "block_reason", "owner_id" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 11,
                columns: new[] { "block_reason", "owner_id" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 12,
                columns: new[] { "block_reason", "owner_id" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 13,
                columns: new[] { "block_reason", "owner_id" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 14,
                columns: new[] { "block_reason", "owner_id" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 15,
                columns: new[] { "block_reason", "owner_id" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 16,
                columns: new[] { "block_reason", "owner_id" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 17,
                columns: new[] { "block_reason", "owner_id" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 18,
                columns: new[] { "block_reason", "owner_id" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 19,
                columns: new[] { "block_reason", "owner_id" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 20,
                columns: new[] { "block_reason", "owner_id" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 21,
                columns: new[] { "block_reason", "owner_id" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 22,
                columns: new[] { "block_reason", "owner_id" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 23,
                columns: new[] { "block_reason", "owner_id" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 24,
                columns: new[] { "block_reason", "owner_id" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 25,
                columns: new[] { "block_reason", "owner_id" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 26,
                columns: new[] { "block_reason", "owner_id" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 27,
                columns: new[] { "block_reason", "owner_id" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 28,
                columns: new[] { "block_reason", "owner_id" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 29,
                columns: new[] { "block_reason", "owner_id" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 30,
                columns: new[] { "block_reason", "owner_id" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 31,
                columns: new[] { "block_reason", "owner_id" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 32,
                columns: new[] { "block_reason", "owner_id" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 33,
                columns: new[] { "block_reason", "owner_id" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 34,
                columns: new[] { "block_reason", "owner_id" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 35,
                columns: new[] { "block_reason", "owner_id" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 36,
                columns: new[] { "block_reason", "owner_id" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 37,
                columns: new[] { "block_reason", "owner_id" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 38,
                columns: new[] { "block_reason", "owner_id" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 39,
                columns: new[] { "block_reason", "owner_id" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 40,
                columns: new[] { "block_reason", "owner_id" },
                values: new object[] { null, null });

            migrationBuilder.CreateIndex(
                name: "IX_engagement_checklist_items_owner_id",
                table: "engagement_checklist_items",
                column: "owner_id");

            migrationBuilder.AddForeignKey(
                name: "FK_engagement_checklist_items_people_owner_id",
                table: "engagement_checklist_items",
                column: "owner_id",
                principalTable: "people",
                principalColumn: "person_id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_engagement_checklist_items_people_owner_id",
                table: "engagement_checklist_items");

            migrationBuilder.DropIndex(
                name: "IX_engagement_checklist_items_owner_id",
                table: "engagement_checklist_items");

            migrationBuilder.DropColumn(
                name: "block_reason",
                table: "engagement_checklist_items");

            migrationBuilder.DropColumn(
                name: "owner_id",
                table: "engagement_checklist_items");
        }
    }
}
