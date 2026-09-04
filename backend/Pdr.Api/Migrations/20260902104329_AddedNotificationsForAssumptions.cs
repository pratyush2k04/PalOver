using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pdr.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddedNotificationsForAssumptions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "notification_templates",
                columns: new[] { "id", "body", "created_at", "created_by", "default_priority", "key", "link_pattern", "title", "updated_at", "updated_by" },
                values: new object[] { 629481, "Assumption \"{AssumptionDescription}\" needs to be validated for {EngagementName}.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", 2, "AssumptionNeedsValidation", "/engagements/{EngagementId}/workspace/assumptions-and-decisions", "Assumption Needs Validation — {EngagementName}", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "notification_templates",
                keyColumn: "id",
                keyValue: 629481);
        }
    }
}
