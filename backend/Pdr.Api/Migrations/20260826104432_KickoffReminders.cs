using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pdr.Api.Migrations
{
    /// <inheritdoc />
    public partial class KickoffReminders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "notification_templates",
                columns: new[] { "id", "body", "created_at", "created_by", "default_priority", "key", "link_pattern", "title", "updated_at", "updated_by" },
                values: new object[] { 96837, "{EngagementName} still has open mandatory items with kickoff in {DaysBeforeKickoff} days.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", 2, "KickoffReadinessReminder", "/engagements/{EngagementId}/workspace/{TabKey}", "{DaysBeforeKickoff} days until kickoff — {EngagementName} readiness incomplete", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "notification_templates",
                keyColumn: "id",
                keyValue: 96837);
        }
    }
}
