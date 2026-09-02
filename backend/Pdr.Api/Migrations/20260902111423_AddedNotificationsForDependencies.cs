using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Pdr.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddedNotificationsForDependencies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "notification_templates",
                columns: new[] { "id", "body", "created_at", "created_by", "default_priority", "key", "link_pattern", "title", "updated_at", "updated_by" },
                values: new object[,]
                {
                    { 742619, "You have been assigned the dependency \"{DependencyDescription}\" for {EngagementName}.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", 2, "DependencyAssignedToOwner", "/engagements/{EngagementId}/workspace/assumptions-and-decisions", "Dependency Assigned — {EngagementName}", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 853720, "Dependency \"{DependencyDescription}\" was assigned to external owner {OwnerName} for {EngagementName}.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", 2, "DependencyAssignedExternal", "/engagements/{EngagementId}/workspace/assumptions-and-decisions", "External Dependency Added — {EngagementName}", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "notification_templates",
                keyColumn: "id",
                keyValue: 742619);

            migrationBuilder.DeleteData(
                table: "notification_templates",
                keyColumn: "id",
                keyValue: 853720);
        }
    }
}
