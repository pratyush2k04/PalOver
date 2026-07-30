using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pdr.Api.Migrations
{
    /// <inheritdoc />
    public partial class Seed_Checklist_Categories_And_Statuses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "id",
                keyValue: 1,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 7, 30, 13, 12, 34, 682, DateTimeKind.Utc).AddTicks(387), new DateTime(2026, 7, 30, 13, 12, 34, 682, DateTimeKind.Utc).AddTicks(391) });

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "id",
                keyValue: 2,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 7, 30, 13, 12, 34, 682, DateTimeKind.Utc).AddTicks(2443), new DateTime(2026, 7, 30, 13, 12, 34, 682, DateTimeKind.Utc).AddTicks(2447) });

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "id",
                keyValue: 3,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 7, 30, 13, 12, 34, 682, DateTimeKind.Utc).AddTicks(2451), new DateTime(2026, 7, 30, 13, 12, 34, 682, DateTimeKind.Utc).AddTicks(2451) });

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "id",
                keyValue: 4,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 7, 30, 13, 12, 34, 682, DateTimeKind.Utc).AddTicks(2453), new DateTime(2026, 7, 30, 13, 12, 34, 682, DateTimeKind.Utc).AddTicks(2453) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "id",
                keyValue: 1,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 7, 30, 12, 51, 18, 901, DateTimeKind.Utc).AddTicks(1005), new DateTime(2026, 7, 30, 12, 51, 18, 901, DateTimeKind.Utc).AddTicks(1008) });

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "id",
                keyValue: 2,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 7, 30, 12, 51, 18, 901, DateTimeKind.Utc).AddTicks(1903), new DateTime(2026, 7, 30, 12, 51, 18, 901, DateTimeKind.Utc).AddTicks(1904) });

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "id",
                keyValue: 3,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 7, 30, 12, 51, 18, 901, DateTimeKind.Utc).AddTicks(1905), new DateTime(2026, 7, 30, 12, 51, 18, 901, DateTimeKind.Utc).AddTicks(1906) });

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "id",
                keyValue: 4,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 7, 30, 12, 51, 18, 901, DateTimeKind.Utc).AddTicks(1907), new DateTime(2026, 7, 30, 12, 51, 18, 901, DateTimeKind.Utc).AddTicks(1907) });
        }
    }
}
