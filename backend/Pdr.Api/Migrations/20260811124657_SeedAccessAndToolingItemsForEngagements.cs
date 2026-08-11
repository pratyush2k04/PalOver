using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pdr.Api.Migrations
{
    /// <inheritdoc />
    public partial class SeedAccessAndToolingItemsForEngagements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AccessItems",
                columns: new[] { "AccessItemId", "AccessItemDefinitionId", "BlockReason", "CreatedAt", "CreatedBy", "EngagementId", "ExpectedTurnaroundDays", "GrantedDate", "IsClientSide", "IsDeleted", "Name", "OwnerId", "Status", "TimeZoneNote", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, 1, null, new DateTime(2026, 8, 11, 12, 46, 54, 637, DateTimeKind.Utc).AddTicks(1447), null, 1, null, null, false, false, "VPN Access", 1, 1, null, new DateTime(2026, 8, 11, 12, 46, 54, 637, DateTimeKind.Utc).AddTicks(1452), null },
                    { 2, 2, null, new DateTime(2026, 8, 11, 12, 46, 54, 637, DateTimeKind.Utc).AddTicks(4504), null, 1, null, null, true, false, "Jira", 1, 1, null, new DateTime(2026, 8, 11, 12, 46, 54, 637, DateTimeKind.Utc).AddTicks(4504), null },
                    { 3, 3, null, new DateTime(2026, 8, 11, 12, 46, 54, 637, DateTimeKind.Utc).AddTicks(4510), null, 1, null, null, true, false, "SandBox", 1, 1, null, new DateTime(2026, 8, 11, 12, 46, 54, 637, DateTimeKind.Utc).AddTicks(4511), null },
                    { 4, 1, null, new DateTime(2026, 8, 11, 12, 46, 54, 637, DateTimeKind.Utc).AddTicks(4514), null, 2, null, new DateOnly(2026, 6, 2), false, false, "VPN Access", 1, 2, null, new DateTime(2026, 8, 11, 12, 46, 54, 637, DateTimeKind.Utc).AddTicks(4514), null },
                    { 5, 2, null, new DateTime(2026, 8, 11, 12, 46, 54, 637, DateTimeKind.Utc).AddTicks(5687), null, 2, null, new DateOnly(2026, 6, 2), true, false, "Jira", 1, 2, null, new DateTime(2026, 8, 11, 12, 46, 54, 637, DateTimeKind.Utc).AddTicks(5687), null },
                    { 6, 3, null, new DateTime(2026, 8, 11, 12, 46, 54, 637, DateTimeKind.Utc).AddTicks(5702), null, 2, null, null, true, false, "SandBox", 1, 1, null, new DateTime(2026, 8, 11, 12, 46, 54, 637, DateTimeKind.Utc).AddTicks(5702), null }
                });

            migrationBuilder.InsertData(
                table: "EnvironmentToolingItems",
                columns: new[] { "EnvironmentToolingItemId", "BlockReason", "CreatedAt", "CreatedBy", "CustomName", "EngagementId", "EnvironmentToolingDefinitionId", "IsDeleted", "Name", "NotApplicableReason", "OwnerId", "Status", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, null, new DateTime(2026, 8, 11, 12, 46, 54, 744, DateTimeKind.Utc).AddTicks(9274), null, null, 1, 1, false, "CI/CD", null, 1, 1, new DateTime(2026, 8, 11, 12, 46, 54, 744, DateTimeKind.Utc).AddTicks(9282), null },
                    { 2, null, new DateTime(2026, 8, 11, 12, 46, 54, 745, DateTimeKind.Utc).AddTicks(1398), null, null, 1, 2, false, "Dev", null, 1, 1, new DateTime(2026, 8, 11, 12, 46, 54, 745, DateTimeKind.Utc).AddTicks(1399), null },
                    { 3, null, new DateTime(2026, 8, 11, 12, 46, 54, 745, DateTimeKind.Utc).AddTicks(1403), null, null, 1, 3, false, "UAT Environment", null, 1, 1, new DateTime(2026, 8, 11, 12, 46, 54, 745, DateTimeKind.Utc).AddTicks(1403), null },
                    { 4, null, new DateTime(2026, 8, 11, 12, 46, 54, 745, DateTimeKind.Utc).AddTicks(1406), null, null, 2, 1, false, "CI/CD", null, 1, 2, new DateTime(2026, 8, 11, 12, 46, 54, 745, DateTimeKind.Utc).AddTicks(1406), null },
                    { 5, null, new DateTime(2026, 8, 11, 12, 46, 54, 745, DateTimeKind.Utc).AddTicks(1408), null, null, 2, 2, false, "Dev", null, 1, 2, new DateTime(2026, 8, 11, 12, 46, 54, 745, DateTimeKind.Utc).AddTicks(1408), null },
                    { 6, null, new DateTime(2026, 8, 11, 12, 46, 54, 745, DateTimeKind.Utc).AddTicks(1410), null, null, 2, 3, false, "UAT Environment", null, 1, 1, new DateTime(2026, 8, 11, 12, 46, 54, 745, DateTimeKind.Utc).AddTicks(1411), null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AccessItems",
                keyColumn: "AccessItemId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AccessItems",
                keyColumn: "AccessItemId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "AccessItems",
                keyColumn: "AccessItemId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "AccessItems",
                keyColumn: "AccessItemId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "AccessItems",
                keyColumn: "AccessItemId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "AccessItems",
                keyColumn: "AccessItemId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "EnvironmentToolingItems",
                keyColumn: "EnvironmentToolingItemId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "EnvironmentToolingItems",
                keyColumn: "EnvironmentToolingItemId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "EnvironmentToolingItems",
                keyColumn: "EnvironmentToolingItemId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "EnvironmentToolingItems",
                keyColumn: "EnvironmentToolingItemId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "EnvironmentToolingItems",
                keyColumn: "EnvironmentToolingItemId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "EnvironmentToolingItems",
                keyColumn: "EnvironmentToolingItemId",
                keyValue: 6);
        }
    }
}
