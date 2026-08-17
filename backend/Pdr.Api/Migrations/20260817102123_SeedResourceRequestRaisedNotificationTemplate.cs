using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pdr.Api.Migrations
{
    /// <inheritdoc />
    public partial class SeedResourceRequestRaisedNotificationTemplate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "notification_templates",
                columns: new[] { "id", "body", "created_at", "created_by", "default_priority", "key", "link_pattern", "title", "updated_at", "updated_by" },
                values: new object[] { 4, "{PeopleNeeded} {RoleName} requested for {EngagementName} by {RequiredByDate}.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", 3, "ResourceRequestRaised", "/engagements/{EngagementId}/workspace/resources", "Resource Request Raised — {EngagementName}", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "notification_templates",
                keyColumn: "id",
                keyValue: 4);
        }
    }
}
