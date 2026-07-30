using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pdr.Api.Migrations
{
    /// <inheritdoc />
    public partial class Role_Configuration_Changes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "created_at",
                table: "roles");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "roles");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "roles",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "NOW()");

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_at",
                table: "roles",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "NOW()");

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "id",
                keyValue: 1,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 7, 29, 17, 25, 36, 200, DateTimeKind.Utc).AddTicks(3146), new DateTime(2026, 7, 29, 17, 25, 36, 200, DateTimeKind.Utc).AddTicks(3149) });

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "id",
                keyValue: 2,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 7, 29, 17, 25, 36, 200, DateTimeKind.Utc).AddTicks(3502), new DateTime(2026, 7, 29, 17, 25, 36, 200, DateTimeKind.Utc).AddTicks(3502) });

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "id",
                keyValue: 3,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 7, 29, 17, 25, 36, 200, DateTimeKind.Utc).AddTicks(3503), new DateTime(2026, 7, 29, 17, 25, 36, 200, DateTimeKind.Utc).AddTicks(3503) });

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "id",
                keyValue: 4,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 7, 29, 17, 25, 36, 200, DateTimeKind.Utc).AddTicks(3504), new DateTime(2026, 7, 29, 17, 25, 36, 200, DateTimeKind.Utc).AddTicks(3504) });
        }
    }
}
