using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Pdr.Api.Migrations
{
    /// <inheritdoc />
    public partial class SeedMockNotifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "notifications",
                columns: new[] { "id", "body", "created_at", "created_by", "data", "link", "priority", "read_at", "receiver_user_id", "scheduled_for", "template_id", "title", "updated_at", "updated_by" },
                values: new object[,]
                {
                    { 2, "This is a MOCK DB ENTRY notification seeded for manual testing. Use PUT /api/Notifications/2/read to mark it read, then confirm GET /api/Notifications/unread-count decreases by one.", new DateTime(2026, 8, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, "/engagements/1/workspace/overview", 2, null, 1, new DateTime(2026, 8, 5, 0, 0, 0, 0, DateTimeKind.Utc), null, "MOCK DB ENTRY notification — mark as read test", new DateTime(2026, 8, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 3, "This is a MOCK DB ENTRY notification seeded for manual testing. Use GET /api/Notifications?engagementId=1 to confirm this entry is included in the filtered results.", new DateTime(2026, 8, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "{\"EngagementId\":1}", "/engagements/1/workspace/handover", 3, null, 1, new DateTime(2026, 8, 6, 0, 0, 0, 0, DateTimeKind.Utc), null, "MOCK DB ENTRY notification — Engagement 1 filter test", new DateTime(2026, 8, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 4, "This is a MOCK DB ENTRY notification seeded for manual testing. Use GET /api/Notifications?engagementId=2 to confirm this entry is included, and GET /api/Notifications?engagementId=1 to confirm it's excluded. Link is intentionally null to test handling of notifications with no redirect target.", new DateTime(2026, 8, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "{\"EngagementId\":2}", null, 1, null, 1, new DateTime(2026, 8, 7, 0, 0, 0, 0, DateTimeKind.Utc), null, "MOCK DB ENTRY notification — Engagement 2, no link", new DateTime(2026, 8, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 5, "This is a MOCK DB ENTRY notification seeded for manual testing. ScheduledFor is in the future, so it should show Status: Pending and must be EXCLUDED from GET /api/Notifications/unread-count even though it is unread. Note: PUT /api/Notifications/read-all will still mark it read, since that endpoint doesn't check ScheduledFor.", new DateTime(2026, 8, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, "/engagements/2/workspace/overview", 2, null, 1, new DateTime(2026, 9, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "MOCK DB ENTRY notification — Pending (future scheduled)", new DateTime(2026, 8, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 6, "This is a MOCK DB ENTRY notification seeded for manual testing. It is already marked read and is intended as the target for DELETE /api/Notifications — send { \"ids\": [6] } to remove it and confirm it no longer appears in GET /api/Notifications.", new DateTime(2026, 8, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, null, 1, new DateTime(2026, 8, 4, 0, 0, 0, 0, DateTimeKind.Utc), 1, new DateTime(2026, 8, 3, 0, 0, 0, 0, DateTimeKind.Utc), null, "MOCK DB ENTRY notification — already read, safe to delete", new DateTime(2026, 8, 1, 0, 0, 0, 0, DateTimeKind.Utc), null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "notifications",
                keyColumn: "id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "notifications",
                keyColumn: "id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "notifications",
                keyColumn: "id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "notifications",
                keyColumn: "id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "notifications",
                keyColumn: "id",
                keyValue: 6);
        }
    }
}
