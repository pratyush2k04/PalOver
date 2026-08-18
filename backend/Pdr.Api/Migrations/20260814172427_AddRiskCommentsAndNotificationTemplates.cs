using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Pdr.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddRiskCommentsAndNotificationTemplates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "risk_comments",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    risk_id = table.Column<int>(type: "integer", nullable: false),
                    content = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    is_edited = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    author_id = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    created_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    updated_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_risk_comments", x => x.id);
                    table.ForeignKey(
                        name: "FK_risk_comments_risks_risk_id",
                        column: x => x.risk_id,
                        principalTable: "risks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_risk_comments_users_author_id",
                        column: x => x.author_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "notification_templates",
                keyColumn: "id",
                keyValue: 1,
                column: "link_pattern",
                value: "/engagements/{EngagementId}/workspace/technical-readiness");

            migrationBuilder.UpdateData(
                table: "notification_templates",
                keyColumn: "id",
                keyValue: 2,
                column: "link_pattern",
                value: "/engagements/{EngagementId}/workspace/overview");

            migrationBuilder.UpdateData(
                table: "notification_templates",
                keyColumn: "id",
                keyValue: 3,
                column: "link_pattern",
                value: "/engagements/{EngagementId}/workspace/handover");

            migrationBuilder.InsertData(
                table: "notification_templates",
                columns: new[] { "id", "body", "created_at", "created_by", "default_priority", "key", "link_pattern", "title", "updated_at", "updated_by" },
                values: new object[] { 6, "{AuthorName} mentioned you in a comment on risk '{RiskTitle}'", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", 2, "RiskCommentMention", "/engagements/{EngagementId}/workspace/risks", "Mentioned in Risk — {RiskTitle}", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null });

            migrationBuilder.CreateIndex(
                name: "IX_risk_comments_author_id",
                table: "risk_comments",
                column: "author_id");

            migrationBuilder.CreateIndex(
                name: "IX_risk_comments_risk_id",
                table: "risk_comments",
                column: "risk_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "risk_comments");

            migrationBuilder.DeleteData(
                table: "notification_templates",
                keyColumn: "id",
                keyValue: 6);
        }
    }
}
