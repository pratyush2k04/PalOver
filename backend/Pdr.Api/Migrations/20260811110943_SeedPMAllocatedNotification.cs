using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pdr.Api.Migrations
{
    /// <inheritdoc />
    public partial class SeedPMAllocatedNotification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "notifications",
                columns: new[] { "id", "body", "created_at", "created_by", "data", "link", "priority", "read_at", "receiver_user_id", "scheduled_for", "template_id", "title", "updated_at", "updated_by" },
                values: new object[] { 1, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "{\"AssignedUserId\":2,\"EngagementId\":1}", "/engagements/1/workspace/overview", 2, null, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "notifications",
                keyColumn: "id",
                keyValue: 1);
        }
    }
}
