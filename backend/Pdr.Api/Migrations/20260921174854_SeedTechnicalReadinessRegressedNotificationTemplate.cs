using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pdr.Api.Migrations
{
    /// <inheritdoc />
    public partial class SeedTechnicalReadinessRegressedNotificationTemplate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "notification_templates",
                columns: new[] { "id", "body", "created_at", "created_by", "default_priority", "key", "link_pattern", "title", "updated_at", "updated_by" },
                values: new object[] { 23, "Technical readiness for {EngagementName} has dropped below 100% ({Reason}). Day 1 sign-off has been disabled until all technical blockers are resolved.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", 3, "TechnicalReadinessRegressed", "/engagements/{EngagementId}/workspace/technical-readiness", "Technical Readiness Dropped Below 100% — {EngagementName}", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "notification_templates",
                keyColumn: "id",
                keyValue: 23);
        }
    }
}
