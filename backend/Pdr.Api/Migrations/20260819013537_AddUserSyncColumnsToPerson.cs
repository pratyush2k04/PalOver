using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pdr.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddUserSyncColumnsToPerson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "source_role_id",
                table: "people",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "user_id",
                table: "people",
                type: "integer",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "people",
                keyColumn: "person_id",
                keyValue: 1,
                columns: new[] { "source_role_id", "user_id" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "people",
                keyColumn: "person_id",
                keyValue: 2,
                columns: new[] { "source_role_id", "user_id" },
                values: new object[] { null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "source_role_id",
                table: "people");

            migrationBuilder.DropColumn(
                name: "user_id",
                table: "people");
        }
    }
}
