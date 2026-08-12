using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Pdr.Api.Migrations
{
    /// <inheritdoc />
    public partial class NotificationsSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "notification_templates",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    key = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    body = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    default_priority = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    created_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    updated_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notification_templates", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "notifications",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    template_id = table.Column<int>(type: "integer", nullable: true),
                    receiver_user_id = table.Column<int>(type: "integer", nullable: false),
                    priority = table.Column<int>(type: "integer", nullable: false),
                    read_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    scheduled_for = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    body = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    data = table.Column<string>(type: "jsonb", nullable: true),
                    link = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    created_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    updated_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notifications", x => x.id);
                    table.CheckConstraint("ck_notifications_read_at_after_scheduled", "read_at IS NULL OR read_at >= scheduled_for");
                    table.ForeignKey(
                        name: "FK_notifications_notification_templates_template_id",
                        column: x => x.template_id,
                        principalTable: "notification_templates",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_notifications_users_receiver_user_id",
                        column: x => x.receiver_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "notification_templates",
                columns: new[] { "id", "body", "created_at", "created_by", "default_priority", "key", "title", "updated_at", "updated_by" },
                values: new object[,]
                {
                    { 1, "{ItemName} was blocked by {TriggerUserName}. Reason: {BlockReason}", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", 3, "AccessItemBlocked", "Access Item Blocked — {ItemName}", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 2, "{PMName} has been allocated as Project Manager for {EngagementName}.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", 2, "PMAllocated", "Project Manager Assigned to {EngagementName}", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 3, "The handover for {EngagementName} has been submitted by {TriggerUserName} and is ready for your review.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", 3, "HandoverSubmitted", "Handover Submitted — {EngagementName}", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null }
                });

            migrationBuilder.InsertData(
                table: "notifications",
                columns: new[] { "id", "body", "created_at", "created_by", "data", "link", "priority", "read_at", "receiver_user_id", "scheduled_for", "template_id", "title", "updated_at", "updated_by" },
                values: new object[,]
                {
                    { 2, "This is a MOCK DB ENTRY notification seeded for manual testing. Use PUT /api/Notifications/2/read to mark it read, then confirm GET /api/Notifications/unread-count decreases by one.", new DateTime(2026, 8, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, "/engagements/1/workspace/overview", 2, null, 1, new DateTime(2026, 8, 5, 0, 0, 0, 0, DateTimeKind.Utc), null, "MOCK DB ENTRY notification — mark as read test", new DateTime(2026, 8, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 3, "This is a MOCK DB ENTRY notification seeded for manual testing. Use GET /api/Notifications?engagementId=1 to confirm this entry is included in the filtered results.", new DateTime(2026, 8, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "{\"EngagementId\":1}", "/engagements/1/workspace/handover", 3, null, 1, new DateTime(2026, 8, 6, 0, 0, 0, 0, DateTimeKind.Utc), null, "MOCK DB ENTRY notification — Engagement 1 filter test", new DateTime(2026, 8, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 4, "This is a MOCK DB ENTRY notification seeded for manual testing. Use GET /api/Notifications?engagementId=2 to confirm this entry is included, and GET /api/Notifications?engagementId=1 to confirm it's excluded. Link is intentionally null to test handling of notifications with no redirect target.", new DateTime(2026, 8, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "{\"EngagementId\":2}", null, 1, null, 1, new DateTime(2026, 8, 7, 0, 0, 0, 0, DateTimeKind.Utc), null, "MOCK DB ENTRY notification — Engagement 2, no link", new DateTime(2026, 8, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 5, "This is a MOCK DB ENTRY notification seeded for manual testing. ScheduledFor is in the future, so it should show Status: Pending and must be EXCLUDED from GET /api/Notifications/unread-count even though it is unread. Note: PUT /api/Notifications/read-all will still mark it read, since that endpoint doesn't check ScheduledFor.", new DateTime(2026, 8, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, "/engagements/2/workspace/overview", 2, null, 1, new DateTime(2026, 9, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "MOCK DB ENTRY notification — Pending (future scheduled)", new DateTime(2026, 8, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 6, "This is a MOCK DB ENTRY notification seeded for manual testing. It is already marked read and is intended as the target for DELETE /api/Notifications — send { \"ids\": [6] } to remove it and confirm it no longer appears in GET /api/Notifications.", new DateTime(2026, 8, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, null, 1, new DateTime(2026, 8, 4, 0, 0, 0, 0, DateTimeKind.Utc), 1, new DateTime(2026, 8, 3, 0, 0, 0, 0, DateTimeKind.Utc), null, "MOCK DB ENTRY notification — already read, safe to delete", new DateTime(2026, 8, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 1, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "{\"AssignedUserId\":2,\"EngagementId\":1}", "/engagements/1/workspace/overview", 2, null, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_notification_templates_key",
                table: "notification_templates",
                column: "key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_notifications_receiver_user_id_read_at",
                table: "notifications",
                columns: new[] { "receiver_user_id", "read_at" });

            migrationBuilder.CreateIndex(
                name: "IX_notifications_template_id",
                table: "notifications",
                column: "template_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "notifications");

            migrationBuilder.DropTable(
                name: "notification_templates");
        }
    }
}
