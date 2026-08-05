using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Pdr.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddEngagementChecklist : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_engagements_master_checklist_versions_checklist_version_id",
                table: "engagements");

            migrationBuilder.DropIndex(
                name: "IX_engagements_checklist_version_id",
                table: "engagements");

            migrationBuilder.DropColumn(
                name: "checklist_version_id",
                table: "engagements");

            migrationBuilder.RenameColumn(
                name: "EngagementName",
                table: "engagements",
                newName: "engagement_name");

            migrationBuilder.CreateTable(
                name: "engagement_checklists",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    engagement_id = table.Column<int>(type: "integer", nullable: false),
                    source_master_version_id = table.Column<int>(type: "integer", nullable: false),
                    version_number = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    created_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    updated_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_engagement_checklists", x => x.id);
                    table.ForeignKey(
                        name: "FK_engagement_checklists_engagements_engagement_id",
                        column: x => x.engagement_id,
                        principalTable: "engagements",
                        principalColumn: "engagement_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "engagement_checklist_items",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    engagement_checklist_id = table.Column<int>(type: "integer", nullable: false),
                    source_master_item_id = table.Column<int>(type: "integer", nullable: true),
                    title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    category_id = table.Column<int>(type: "integer", nullable: false),
                    is_mandatory = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    display_order = table.Column<int>(type: "integer", nullable: false),
                    is_applicable = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    created_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    updated_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_engagement_checklist_items", x => x.id);
                    table.ForeignKey(
                        name: "FK_engagement_checklist_items_checklist_categories_category_id",
                        column: x => x.category_id,
                        principalTable: "checklist_categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_engagement_checklist_items_engagement_checklists_engagement~",
                        column: x => x.engagement_checklist_id,
                        principalTable: "engagement_checklists",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "engagement_checklists",
                columns: new[] { "id", "created_at", "created_by", "engagement_id", "source_master_version_id", "updated_at", "updated_by", "version_number" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", 1, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", 0 },
                    { 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", 2, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", 0 }
                });

            migrationBuilder.UpdateData(
                table: "engagements",
                keyColumn: "engagement_id",
                keyValue: 1,
                column: "engagement_name",
                value: "FinTech Core Banking");

            migrationBuilder.UpdateData(
                table: "engagements",
                keyColumn: "engagement_id",
                keyValue: 2,
                column: "engagement_name",
                value: "HealthCare Legacy Upgrade");

            migrationBuilder.InsertData(
                table: "engagement_checklist_items",
                columns: new[] { "id", "category_id", "created_at", "created_by", "description", "display_order", "engagement_checklist_id", "is_applicable", "is_mandatory", "source_master_item_id", "status", "title", "updated_at", "updated_by" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, 1, 1, true, true, 1, "Incomplete", "Sign commercial contract", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 2, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, 2, 1, true, true, 2, "Incomplete", "Confirm engagement budget", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 3, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, 3, 1, true, true, 3, "Incomplete", "Confirm billing schedule", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 4, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, 4, 1, true, true, 4, "Incomplete", "Complete sales-to-delivery handover call", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 5, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, 5, 1, true, true, 5, "Incomplete", "Share CRM opportunity notes", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 6, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, 6, 1, true, true, 6, "Incomplete", "Transfer key client contacts", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 7, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, 7, 1, true, true, 7, "Incomplete", "Document engagement scope", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 8, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, 8, 1, true, true, 8, "Incomplete", "Confirm deliverables list", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null }
                });

            migrationBuilder.InsertData(
                table: "engagement_checklist_items",
                columns: new[] { "id", "category_id", "created_at", "created_by", "description", "display_order", "engagement_checklist_id", "is_applicable", "source_master_item_id", "status", "title", "updated_at", "updated_by" },
                values: new object[] { 9, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, 9, 1, true, 9, "Incomplete", "Identify scope assumptions and exclusions", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null });

            migrationBuilder.InsertData(
                table: "engagement_checklist_items",
                columns: new[] { "id", "category_id", "created_at", "created_by", "description", "display_order", "engagement_checklist_id", "is_applicable", "is_mandatory", "source_master_item_id", "status", "title", "updated_at", "updated_by" },
                values: new object[,]
                {
                    { 10, 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, 10, 1, true, true, 10, "Incomplete", "Set up engagement governance cadence", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 11, 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, 11, 1, true, true, 11, "Incomplete", "Assign engagement steering committee", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 12, 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, 12, 1, true, true, 12, "Incomplete", "Define change control process", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null }
                });

            migrationBuilder.InsertData(
                table: "engagement_checklist_items",
                columns: new[] { "id", "category_id", "created_at", "created_by", "description", "display_order", "engagement_checklist_id", "is_applicable", "source_master_item_id", "status", "title", "updated_at", "updated_by" },
                values: new object[] { 13, 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, 13, 1, true, 13, "Incomplete", "Confirm RAID log owner", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null });

            migrationBuilder.InsertData(
                table: "engagement_checklist_items",
                columns: new[] { "id", "category_id", "created_at", "created_by", "description", "display_order", "engagement_checklist_id", "is_applicable", "is_mandatory", "source_master_item_id", "status", "title", "updated_at", "updated_by" },
                values: new object[,]
                {
                    { 14, 5, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, 14, 1, true, true, 14, "Incomplete", "Allocate delivery team members", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 15, 5, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, 15, 1, true, true, 15, "Incomplete", "Confirm resource availability calendar", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null }
                });

            migrationBuilder.InsertData(
                table: "engagement_checklist_items",
                columns: new[] { "id", "category_id", "created_at", "created_by", "description", "display_order", "engagement_checklist_id", "is_applicable", "source_master_item_id", "status", "title", "updated_at", "updated_by" },
                values: new object[] { 16, 5, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, 16, 1, true, 16, "Incomplete", "Identify backup/on-call resources", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null });

            migrationBuilder.InsertData(
                table: "engagement_checklist_items",
                columns: new[] { "id", "category_id", "created_at", "created_by", "description", "display_order", "engagement_checklist_id", "is_applicable", "is_mandatory", "source_master_item_id", "status", "title", "updated_at", "updated_by" },
                values: new object[,]
                {
                    { 17, 6, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, 17, 1, true, true, 17, "Incomplete", "Provision environment access", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 18, 6, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, 18, 1, true, true, 18, "Incomplete", "Verify technical prerequisites", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 19, 6, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, 19, 1, true, true, 19, "Incomplete", "Set up source control and CI/CD access", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null }
                });

            migrationBuilder.InsertData(
                table: "engagement_checklist_items",
                columns: new[] { "id", "category_id", "created_at", "created_by", "description", "display_order", "engagement_checklist_id", "is_applicable", "source_master_item_id", "status", "title", "updated_at", "updated_by" },
                values: new object[] { 20, 6, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, 20, 1, true, 20, "Incomplete", "Confirm security and compliance requirements", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null });

            migrationBuilder.InsertData(
                table: "engagement_checklist_items",
                columns: new[] { "id", "category_id", "created_at", "created_by", "description", "display_order", "engagement_checklist_id", "is_applicable", "is_mandatory", "source_master_item_id", "status", "title", "updated_at", "updated_by" },
                values: new object[,]
                {
                    { 21, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, 1, 2, true, true, 1, "Incomplete", "Sign commercial contract", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 22, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, 2, 2, true, true, 2, "Incomplete", "Confirm engagement budget", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 23, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, 3, 2, true, true, 3, "Incomplete", "Confirm billing schedule", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 24, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, 4, 2, true, true, 4, "Incomplete", "Complete sales-to-delivery handover call", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 25, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, 5, 2, true, true, 5, "Incomplete", "Share CRM opportunity notes", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 26, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, 6, 2, true, true, 6, "Incomplete", "Transfer key client contacts", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 27, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, 7, 2, true, true, 7, "Incomplete", "Document engagement scope", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 28, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, 8, 2, true, true, 8, "Incomplete", "Confirm deliverables list", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null }
                });

            migrationBuilder.InsertData(
                table: "engagement_checklist_items",
                columns: new[] { "id", "category_id", "created_at", "created_by", "description", "display_order", "engagement_checklist_id", "is_applicable", "source_master_item_id", "status", "title", "updated_at", "updated_by" },
                values: new object[] { 29, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, 9, 2, true, 9, "Incomplete", "Identify scope assumptions and exclusions", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null });

            migrationBuilder.InsertData(
                table: "engagement_checklist_items",
                columns: new[] { "id", "category_id", "created_at", "created_by", "description", "display_order", "engagement_checklist_id", "is_applicable", "is_mandatory", "source_master_item_id", "status", "title", "updated_at", "updated_by" },
                values: new object[,]
                {
                    { 30, 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, 10, 2, true, true, 10, "Incomplete", "Set up engagement governance cadence", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 31, 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, 11, 2, true, true, 11, "Incomplete", "Assign engagement steering committee", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 32, 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, 12, 2, true, true, 12, "Incomplete", "Define change control process", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null }
                });

            migrationBuilder.InsertData(
                table: "engagement_checklist_items",
                columns: new[] { "id", "category_id", "created_at", "created_by", "description", "display_order", "engagement_checklist_id", "is_applicable", "source_master_item_id", "status", "title", "updated_at", "updated_by" },
                values: new object[] { 33, 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, 13, 2, true, 13, "Incomplete", "Confirm RAID log owner", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null });

            migrationBuilder.InsertData(
                table: "engagement_checklist_items",
                columns: new[] { "id", "category_id", "created_at", "created_by", "description", "display_order", "engagement_checklist_id", "is_applicable", "is_mandatory", "source_master_item_id", "status", "title", "updated_at", "updated_by" },
                values: new object[,]
                {
                    { 34, 5, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, 14, 2, true, true, 14, "Incomplete", "Allocate delivery team members", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 35, 5, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, 15, 2, true, true, 15, "Incomplete", "Confirm resource availability calendar", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null }
                });

            migrationBuilder.InsertData(
                table: "engagement_checklist_items",
                columns: new[] { "id", "category_id", "created_at", "created_by", "description", "display_order", "engagement_checklist_id", "is_applicable", "source_master_item_id", "status", "title", "updated_at", "updated_by" },
                values: new object[] { 36, 5, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, 16, 2, true, 16, "Incomplete", "Identify backup/on-call resources", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null });

            migrationBuilder.InsertData(
                table: "engagement_checklist_items",
                columns: new[] { "id", "category_id", "created_at", "created_by", "description", "display_order", "engagement_checklist_id", "is_applicable", "is_mandatory", "source_master_item_id", "status", "title", "updated_at", "updated_by" },
                values: new object[,]
                {
                    { 37, 6, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, 17, 2, true, true, 17, "Incomplete", "Provision environment access", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 38, 6, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, 18, 2, true, true, 18, "Incomplete", "Verify technical prerequisites", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 39, 6, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, 19, 2, true, true, 19, "Incomplete", "Set up source control and CI/CD access", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null }
                });

            migrationBuilder.InsertData(
                table: "engagement_checklist_items",
                columns: new[] { "id", "category_id", "created_at", "created_by", "description", "display_order", "engagement_checklist_id", "is_applicable", "source_master_item_id", "status", "title", "updated_at", "updated_by" },
                values: new object[] { 40, 6, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, 20, 2, true, 20, "Incomplete", "Confirm security and compliance requirements", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null });

            migrationBuilder.CreateIndex(
                name: "IX_engagement_checklist_items_category_id",
                table: "engagement_checklist_items",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "IX_engagement_checklist_items_engagement_checklist_id",
                table: "engagement_checklist_items",
                column: "engagement_checklist_id");

            migrationBuilder.CreateIndex(
                name: "IX_engagement_checklists_engagement_id",
                table: "engagement_checklists",
                column: "engagement_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "engagement_checklist_items");

            migrationBuilder.DropTable(
                name: "engagement_checklists");

            migrationBuilder.RenameColumn(
                name: "engagement_name",
                table: "engagements",
                newName: "EngagementName");

            migrationBuilder.AddColumn<int>(
                name: "checklist_version_id",
                table: "engagements",
                type: "integer",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "engagements",
                keyColumn: "engagement_id",
                keyValue: 1,
                columns: new[] { "checklist_version_id", "EngagementName" },
                values: new object[] { null, "" });

            migrationBuilder.UpdateData(
                table: "engagements",
                keyColumn: "engagement_id",
                keyValue: 2,
                columns: new[] { "checklist_version_id", "EngagementName" },
                values: new object[] { null, "" });

            migrationBuilder.CreateIndex(
                name: "IX_engagements_checklist_version_id",
                table: "engagements",
                column: "checklist_version_id");

            migrationBuilder.AddForeignKey(
                name: "FK_engagements_master_checklist_versions_checklist_version_id",
                table: "engagements",
                column: "checklist_version_id",
                principalTable: "master_checklist_versions",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
