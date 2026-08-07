using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Pdr.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddPersonEntityAndSeeding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Risks");

            migrationBuilder.DropColumn(
                name: "has_no_commitments",
                table: "handover_info");

            migrationBuilder.DropColumn(
                name: "quantity",
                table: "commitments");

            migrationBuilder.AddColumn<int>(
                name: "timeline",
                table: "commitments",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "timeline_unit",
                table: "commitments",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "title",
                table: "commitments",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "change_requests",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    engagement_id = table.Column<int>(type: "integer", nullable: false),
                    description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    impact = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    rejection_reason = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    reviewed_by_id = table.Column<int>(type: "integer", nullable: true),
                    reviewed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    created_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    updated_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_change_requests", x => x.id);
                    table.ForeignKey(
                        name: "FK_change_requests_engagements_engagement_id",
                        column: x => x.engagement_id,
                        principalTable: "engagements",
                        principalColumn: "engagement_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_change_requests_users_reviewed_by_id",
                        column: x => x.reviewed_by_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "communication_cadences",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    engagement_id = table.Column<int>(type: "integer", nullable: false),
                    stakeholder_role = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    cadence = table.Column<int>(type: "integer", nullable: false),
                    participants = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    created_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    updated_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_communication_cadences", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "kickoff_records",
                columns: table => new
                {
                    kickoff_record_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    engagement_id = table.Column<int>(type: "integer", nullable: false),
                    outcomes = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    created_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    updated_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_kickoff_records", x => x.kickoff_record_id);
                    table.ForeignKey(
                        name: "FK_kickoff_records_engagements_engagement_id",
                        column: x => x.engagement_id,
                        principalTable: "engagements",
                        principalColumn: "engagement_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "people",
                columns: table => new
                {
                    person_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    engagement_id = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    role = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    person_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    created_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    updated_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_people", x => x.person_id);
                    table.ForeignKey(
                        name: "FK_people_engagements_engagement_id",
                        column: x => x.engagement_id,
                        principalTable: "engagements",
                        principalColumn: "engagement_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "realistic_commitments",
                columns: table => new
                {
                    commitment_id = table.Column<int>(type: "integer", nullable: false),
                    description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    timeline = table.Column<int>(type: "integer", nullable: false),
                    timeline_unit = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    acknowledgement_message = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    is_possible = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    created_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    updated_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_realistic_commitments", x => x.commitment_id);
                    table.ForeignKey(
                        name: "FK_realistic_commitments_commitments_commitment_id",
                        column: x => x.commitment_id,
                        principalTable: "commitments",
                        principalColumn: "commitment_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "kickoff_action_items",
                columns: table => new
                {
                    kickoff_action_item_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    kickoff_record_id = table.Column<int>(type: "integer", nullable: false),
                    engagement_id = table.Column<int>(type: "integer", nullable: false),
                    title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    owner_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    assigned_to_user_id = table.Column<int>(type: "integer", nullable: true),
                    due_date = table.Column<DateOnly>(type: "date", nullable: false),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: "Open"),
                    is_notified = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    created_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    updated_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_kickoff_action_items", x => x.kickoff_action_item_id);
                    table.ForeignKey(
                        name: "FK_kickoff_action_items_engagements_engagement_id",
                        column: x => x.engagement_id,
                        principalTable: "engagements",
                        principalColumn: "engagement_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_kickoff_action_items_kickoff_records_kickoff_record_id",
                        column: x => x.kickoff_record_id,
                        principalTable: "kickoff_records",
                        principalColumn: "kickoff_record_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_kickoff_action_items_users_assigned_to_user_id",
                        column: x => x.assigned_to_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "change_requests",
                columns: new[] { "id", "created_at", "created_by", "description", "engagement_id", "impact", "rejection_reason", "reviewed_at", "reviewed_by_id", "status", "updated_at", "updated_by" },
                values: new object[] { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "2", "Add SMS notifications to the rewards flow", 1, "+1.5 weeks, +1 developer-week", null, null, null, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "2" });

            migrationBuilder.UpdateData(
                table: "commitments",
                keyColumn: "commitment_id",
                keyValue: 1,
                columns: new[] { "timeline", "timeline_unit", "title" },
                values: new object[] { 3, "Months", "MVP Banking Module Delivery" });

            migrationBuilder.InsertData(
                table: "communication_cadences",
                columns: new[] { "id", "cadence", "created_at", "created_by", "engagement_id", "participants", "stakeholder_role", "updated_at", "updated_by" },
                values: new object[] { 1, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", 1, "Rahul Verma, Asha Rao", "Client Sponsor", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null });

            migrationBuilder.InsertData(
                table: "people",
                columns: new[] { "person_id", "created_at", "created_by", "email", "engagement_id", "name", "person_type", "role", "updated_at", "updated_by" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "sarah.jenkins@client.com", 1, "Sarah Jenkins", "Client", "Client Executive Sponsor", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "alex.rivera@company.com", 1, "Alex Rivera", "Internal", "Internal Technical Lead", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_change_requests_engagement_id",
                table: "change_requests",
                column: "engagement_id");

            migrationBuilder.CreateIndex(
                name: "IX_change_requests_reviewed_by_id",
                table: "change_requests",
                column: "reviewed_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_kickoff_action_items_assigned_to_user_id",
                table: "kickoff_action_items",
                column: "assigned_to_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_kickoff_action_items_engagement_id",
                table: "kickoff_action_items",
                column: "engagement_id");

            migrationBuilder.CreateIndex(
                name: "IX_kickoff_action_items_kickoff_record_id",
                table: "kickoff_action_items",
                column: "kickoff_record_id");

            migrationBuilder.CreateIndex(
                name: "IX_kickoff_records_engagement_id",
                table: "kickoff_records",
                column: "engagement_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_people_engagement_id",
                table: "people",
                column: "engagement_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "change_requests");

            migrationBuilder.DropTable(
                name: "communication_cadences");

            migrationBuilder.DropTable(
                name: "kickoff_action_items");

            migrationBuilder.DropTable(
                name: "people");

            migrationBuilder.DropTable(
                name: "realistic_commitments");

            migrationBuilder.DropTable(
                name: "kickoff_records");

            migrationBuilder.DropColumn(
                name: "timeline",
                table: "commitments");

            migrationBuilder.DropColumn(
                name: "timeline_unit",
                table: "commitments");

            migrationBuilder.DropColumn(
                name: "title",
                table: "commitments");

            migrationBuilder.AddColumn<bool>(
                name: "has_no_commitments",
                table: "handover_info",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "quantity",
                table: "commitments",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Risks",
                columns: table => new
                {
                    RiskId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EngagementId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Risks", x => x.RiskId);
                    table.ForeignKey(
                        name: "FK_Risks_engagements_EngagementId",
                        column: x => x.EngagementId,
                        principalTable: "engagements",
                        principalColumn: "engagement_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "commitments",
                keyColumn: "commitment_id",
                keyValue: 1,
                column: "quantity",
                value: "1");

            migrationBuilder.UpdateData(
                table: "handover_info",
                keyColumn: "handover_id",
                keyValue: 1,
                columns: new string[0],
                values: new object[0]);

            migrationBuilder.CreateIndex(
                name: "IX_Risks_EngagementId",
                table: "Risks",
                column: "EngagementId");
        }
    }
}
