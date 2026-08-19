using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Pdr.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddResourceRequestModelsAndNotificationTemplate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_client_contact_transitions_engagement_id",
                table: "client_contact_transitions");

            migrationBuilder.CreateTable(
                name: "resource_requests",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    engagement_id = table.Column<int>(type: "integer", nullable: false),
                    standard_role = table.Column<int>(type: "integer", nullable: false),
                    people_needed = table.Column<int>(type: "integer", nullable: false),
                    required_by_date = table.Column<DateOnly>(type: "date", nullable: false),
                    job_description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    requested_by_id = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    created_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    updated_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_resource_requests", x => x.id);
                    table.CheckConstraint("ck_resource_requests_job_description_min_length", "char_length(job_description) >= 15");
                    table.CheckConstraint("ck_resource_requests_people_needed_positive", "people_needed > 0");
                    table.ForeignKey(
                        name: "FK_resource_requests_engagements_engagement_id",
                        column: x => x.engagement_id,
                        principalTable: "engagements",
                        principalColumn: "engagement_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_resource_requests_users_requested_by_id",
                        column: x => x.requested_by_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "resource_request_skills",
                columns: table => new
                {
                    resource_request_id = table.Column<int>(type: "integer", nullable: false),
                    skill = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_resource_request_skills", x => new { x.resource_request_id, x.skill });
                    table.ForeignKey(
                        name: "FK_resource_request_skills_resource_requests_resource_request_~",
                        column: x => x.resource_request_id,
                        principalTable: "resource_requests",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "notification_templates",
                columns: new[] { "id", "body", "created_at", "created_by", "default_priority", "key", "link_pattern", "title", "updated_at", "updated_by" },
                values: new object[,]
                {
                    { 4, "{PeopleNeeded} {RoleName} requested for {EngagementName} by {RequiredByDate}.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", 3, "ResourceRequestRaised", "/engagements/{EngagementId}/workspace/resources", "Resource Request Raised — {EngagementName}", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 7, "You have been assigned as Engagement Leader for {EngagementName}.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", 2, "LeadershipAssigned", "/engagements/{EngagementId}/workspace/overview", "Assigned to Engagement — {EngagementName}", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 8, "You have been assigned as Project Manager for {EngagementName}.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", 2, "PMAssigned", "/engagements/{EngagementId}/workspace/overview", "Assigned as Project Manager — {EngagementName}", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 9, "You have been assigned as Technical Lead for {EngagementName}.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", 2, "TechLeadAssigned", "/engagements/{EngagementId}/workspace/overview", "Assigned as Technical Lead — {EngagementName}", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 10, "{UploaderName} uploaded {DocumentName} to {EngagementName}.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", 2, "DocumentUploaded", "/engagements/{EngagementId}/workspace/documents", "New Document Uploaded — {DocumentName}", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_client_contact_transitions_engagement_id",
                table: "client_contact_transitions",
                column: "engagement_id");

            migrationBuilder.CreateIndex(
                name: "IX_resource_requests_engagement_id_status",
                table: "resource_requests",
                columns: new[] { "engagement_id", "status" });

            migrationBuilder.CreateIndex(
                name: "IX_resource_requests_requested_by_id",
                table: "resource_requests",
                column: "requested_by_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "resource_request_skills");

            migrationBuilder.DropTable(
                name: "resource_requests");

            migrationBuilder.DropIndex(
                name: "IX_client_contact_transitions_engagement_id",
                table: "client_contact_transitions");

            migrationBuilder.DeleteData(
                table: "notification_templates",
                keyColumn: "id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "notification_templates",
                keyColumn: "id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "notification_templates",
                keyColumn: "id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "notification_templates",
                keyColumn: "id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "notification_templates",
                keyColumn: "id",
                keyValue: 10);

            migrationBuilder.CreateIndex(
                name: "IX_client_contact_transitions_engagement_id",
                table: "client_contact_transitions",
                column: "engagement_id",
                unique: true);
        }
    }
}
