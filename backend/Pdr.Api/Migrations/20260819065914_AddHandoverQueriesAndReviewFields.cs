using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Pdr.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddHandoverQueriesAndReviewFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "reviewed_at",
                table: "handover_info",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "reviewed_by_user_id",
                table: "handover_info",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "submitted_at",
                table: "handover_info",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "submitted_by_user_id",
                table: "handover_info",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "handover_query_field_sections",
                columns: table => new
                {
                    handover_query_field_section_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_handover_query_field_sections", x => x.handover_query_field_section_id);
                });

            migrationBuilder.CreateTable(
                name: "handover_queries",
                columns: table => new
                {
                    query_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    handover_id = table.Column<int>(type: "integer", nullable: false),
                    field_section_id = table.Column<int>(type: "integer", nullable: false),
                    reference_id = table.Column<int>(type: "integer", nullable: true),
                    is_resolved = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    resolved_by_user_id = table.Column<int>(type: "integer", nullable: true),
                    resolved_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    created_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    updated_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_handover_queries", x => x.query_id);
                    table.ForeignKey(
                        name: "FK_handover_queries_handover_info_handover_id",
                        column: x => x.handover_id,
                        principalTable: "handover_info",
                        principalColumn: "handover_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_handover_queries_handover_query_field_sections_field_sectio~",
                        column: x => x.field_section_id,
                        principalTable: "handover_query_field_sections",
                        principalColumn: "handover_query_field_section_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_handover_queries_users_resolved_by_user_id",
                        column: x => x.resolved_by_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "handover_query_questions",
                columns: table => new
                {
                    query_id = table.Column<int>(type: "integer", nullable: false),
                    question_text = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    created_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    updated_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_handover_query_questions", x => x.query_id);
                    table.ForeignKey(
                        name: "FK_handover_query_questions_handover_queries_query_id",
                        column: x => x.query_id,
                        principalTable: "handover_queries",
                        principalColumn: "query_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "handover_query_responses",
                columns: table => new
                {
                    query_id = table.Column<int>(type: "integer", nullable: false),
                    response_text = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    created_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    updated_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_handover_query_responses", x => x.query_id);
                    table.ForeignKey(
                        name: "FK_handover_query_responses_handover_queries_query_id",
                        column: x => x.query_id,
                        principalTable: "handover_queries",
                        principalColumn: "query_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "audit_actions",
                columns: new[] { "Id", "Action", "Details" },
                values: new object[,]
                {
                    { 856017, "HandoverQueryRaised", "{actorName} raised a query on {fieldSection} for the handover on {engagementName}" },
                    { 856018, "HandoverQueryResponded", "{actorName} responded to a query on {fieldSection} for the handover on {engagementName}" },
                    { 856019, "HandoverQueryResolved", "{actorName} marked a query on {fieldSection} resolved for the handover on {engagementName}" }
                });

            migrationBuilder.UpdateData(
                table: "handover_info",
                keyColumn: "handover_id",
                keyValue: 1,
                columns: new[] { "reviewed_at", "reviewed_by_user_id", "submitted_at", "submitted_by_user_id" },
                values: new object[] { null, null, null, null });

            migrationBuilder.InsertData(
                table: "handover_query_field_sections",
                columns: new[] { "handover_query_field_section_id", "name" },
                values: new object[,]
                {
                    { 1, "ClientBackground" },
                    { 2, "BusinessProblem" },
                    { 3, "ClientSentiment" },
                    { 4, "DealOrigin" },
                    { 5, "Commitment" }
                });

            migrationBuilder.InsertData(
                table: "notification_templates",
                columns: new[] { "id", "body", "created_at", "created_by", "default_priority", "key", "link_pattern", "title", "updated_at", "updated_by" },
                values: new object[] { 856017, "{TriggerUserName} raised a query on {FieldSection} for {EngagementName}.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", 3, "HandoverQueryRaised", "/engagements/{EngagementId}/workspace/handover", "Handover Query Raised — {EngagementName}", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null });

            migrationBuilder.CreateIndex(
                name: "IX_handover_info_reviewed_by_user_id",
                table: "handover_info",
                column: "reviewed_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_handover_info_submitted_by_user_id",
                table: "handover_info",
                column: "submitted_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_handover_queries_field_section_id",
                table: "handover_queries",
                column: "field_section_id");

            migrationBuilder.CreateIndex(
                name: "IX_handover_queries_handover_id_is_resolved",
                table: "handover_queries",
                columns: new[] { "handover_id", "is_resolved" });

            migrationBuilder.CreateIndex(
                name: "IX_handover_queries_resolved_by_user_id",
                table: "handover_queries",
                column: "resolved_by_user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_handover_info_users_reviewed_by_user_id",
                table: "handover_info",
                column: "reviewed_by_user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_handover_info_users_submitted_by_user_id",
                table: "handover_info",
                column: "submitted_by_user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_handover_info_users_reviewed_by_user_id",
                table: "handover_info");

            migrationBuilder.DropForeignKey(
                name: "FK_handover_info_users_submitted_by_user_id",
                table: "handover_info");

            migrationBuilder.DropTable(
                name: "handover_query_questions");

            migrationBuilder.DropTable(
                name: "handover_query_responses");

            migrationBuilder.DropTable(
                name: "handover_queries");

            migrationBuilder.DropTable(
                name: "handover_query_field_sections");

            migrationBuilder.DropIndex(
                name: "IX_handover_info_reviewed_by_user_id",
                table: "handover_info");

            migrationBuilder.DropIndex(
                name: "IX_handover_info_submitted_by_user_id",
                table: "handover_info");

            migrationBuilder.DeleteData(
                table: "audit_actions",
                keyColumn: "Id",
                keyValue: 856017);

            migrationBuilder.DeleteData(
                table: "audit_actions",
                keyColumn: "Id",
                keyValue: 856018);

            migrationBuilder.DeleteData(
                table: "audit_actions",
                keyColumn: "Id",
                keyValue: 856019);

            migrationBuilder.DeleteData(
                table: "notification_templates",
                keyColumn: "id",
                keyValue: 856017);

            migrationBuilder.DropColumn(
                name: "reviewed_at",
                table: "handover_info");

            migrationBuilder.DropColumn(
                name: "reviewed_by_user_id",
                table: "handover_info");

            migrationBuilder.DropColumn(
                name: "submitted_at",
                table: "handover_info");

            migrationBuilder.DropColumn(
                name: "submitted_by_user_id",
                table: "handover_info");
        }
    }
}
