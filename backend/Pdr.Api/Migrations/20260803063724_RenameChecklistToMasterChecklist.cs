using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Pdr.Api.Migrations
{
    /// <inheritdoc />
    public partial class RenameChecklistToMasterChecklist : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "checklist_items");

            migrationBuilder.DropTable(
                name: "checklist_versions");

            migrationBuilder.CreateTable(
                name: "master_checklist_versions",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    version_number = table.Column<int>(type: "integer", nullable: false),
                    status_id = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    created_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    updated_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_master_checklist_versions", x => x.id);
                    table.ForeignKey(
                        name: "FK_master_checklist_versions_checklist_statuses_status_id",
                        column: x => x.status_id,
                        principalTable: "checklist_statuses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "master_checklist_items",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    version_id = table.Column<long>(type: "bigint", nullable: false),
                    title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    category_id = table.Column<long>(type: "bigint", nullable: false),
                    is_mandatory = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    display_order = table.Column<int>(type: "integer", nullable: false),
                    is_retired = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    created_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    updated_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_master_checklist_items", x => x.id);
                    table.ForeignKey(
                        name: "FK_master_checklist_items_checklist_categories_category_id",
                        column: x => x.category_id,
                        principalTable: "checklist_categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_master_checklist_items_master_checklist_versions_version_id",
                        column: x => x.version_id,
                        principalTable: "master_checklist_versions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "master_checklist_versions",
                columns: new[] { "id", "created_at", "created_by", "status_id", "updated_at", "updated_by", "version_number" },
                values: new object[] { 1L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1 });

            migrationBuilder.InsertData(
                table: "master_checklist_items",
                columns: new[] { "id", "category_id", "created_at", "created_by", "description", "display_order", "is_mandatory", "title", "updated_at", "updated_by", "version_id" },
                values: new object[,]
                {
                    { 1L, 1L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Confirm the signed commercial contract is on file before kickoff.", 1, true, "Sign commercial contract", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1L },
                    { 2L, 1L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Verify the approved budget matches the statement of work.", 2, true, "Confirm engagement budget", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1L },
                    { 3L, 1L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Agree invoicing milestones and payment terms with the client.", 3, true, "Confirm billing schedule", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1L },
                    { 4L, 2L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Sales team walks delivery team through client history and expectations.", 4, true, "Complete sales-to-delivery handover call", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1L },
                    { 5L, 2L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Ensure delivery has access to all pre-sales notes and correspondence.", 5, true, "Share CRM opportunity notes", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1L },
                    { 6L, 2L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Confirm delivery team has contact details for all client stakeholders.", 6, true, "Transfer key client contacts", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1L },
                    { 7L, 3L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Capture in-scope and out-of-scope items in the kickoff scope document.", 7, true, "Document engagement scope", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1L },
                    { 8L, 3L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "List and confirm all deliverables committed to the client.", 8, true, "Confirm deliverables list", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1L }
                });

            migrationBuilder.InsertData(
                table: "master_checklist_items",
                columns: new[] { "id", "category_id", "created_at", "created_by", "description", "display_order", "title", "updated_at", "updated_by", "version_id" },
                values: new object[] { 9L, 3L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Record assumptions made during the sales cycle that affect scope.", 9, "Identify scope assumptions and exclusions", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1L });

            migrationBuilder.InsertData(
                table: "master_checklist_items",
                columns: new[] { "id", "category_id", "created_at", "created_by", "description", "display_order", "is_mandatory", "title", "updated_at", "updated_by", "version_id" },
                values: new object[,]
                {
                    { 10L, 4L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Agree recurring status meetings and escalation paths.", 10, true, "Set up engagement governance cadence", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1L },
                    { 11L, 4L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Identify decision-makers on both client and delivery sides.", 11, true, "Assign engagement steering committee", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1L },
                    { 12L, 4L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Agree how scope changes will be raised, approved, and tracked.", 12, true, "Define change control process", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1L }
                });

            migrationBuilder.InsertData(
                table: "master_checklist_items",
                columns: new[] { "id", "category_id", "created_at", "created_by", "description", "display_order", "title", "updated_at", "updated_by", "version_id" },
                values: new object[] { 13L, 4L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Assign an owner to maintain the Risks, Assumptions, Issues, and Dependencies log.", 13, "Confirm RAID log owner", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1L });

            migrationBuilder.InsertData(
                table: "master_checklist_items",
                columns: new[] { "id", "category_id", "created_at", "created_by", "description", "display_order", "is_mandatory", "title", "updated_at", "updated_by", "version_id" },
                values: new object[,]
                {
                    { 14L, 5L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Confirm named resources are assigned and available for the engagement.", 14, true, "Allocate delivery team members", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1L },
                    { 15L, 5L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Check assigned resources have capacity for the engagement duration.", 15, true, "Confirm resource availability calendar", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1L }
                });

            migrationBuilder.InsertData(
                table: "master_checklist_items",
                columns: new[] { "id", "category_id", "created_at", "created_by", "description", "display_order", "title", "updated_at", "updated_by", "version_id" },
                values: new object[] { 16L, 5L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Name backup resources in case of absence during the engagement.", 16, "Identify backup/on-call resources", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1L });

            migrationBuilder.InsertData(
                table: "master_checklist_items",
                columns: new[] { "id", "category_id", "created_at", "created_by", "description", "display_order", "is_mandatory", "title", "updated_at", "updated_by", "version_id" },
                values: new object[,]
                {
                    { 17L, 6L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Request and confirm access to required client systems and environments.", 17, true, "Provision environment access", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1L },
                    { 18L, 6L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Confirm client infrastructure and tooling meet engagement requirements.", 18, true, "Verify technical prerequisites", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1L },
                    { 19L, 6L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Ensure delivery team has access to repositories and pipelines.", 19, true, "Set up source control and CI/CD access", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1L }
                });

            migrationBuilder.InsertData(
                table: "master_checklist_items",
                columns: new[] { "id", "category_id", "created_at", "created_by", "description", "display_order", "title", "updated_at", "updated_by", "version_id" },
                values: new object[] { 20L, 6L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Review any client-specific security or compliance obligations.", 20, "Confirm security and compliance requirements", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1L });

            migrationBuilder.CreateIndex(
                name: "IX_master_checklist_items_category_id",
                table: "master_checklist_items",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "IX_master_checklist_items_version_id",
                table: "master_checklist_items",
                column: "version_id");

            migrationBuilder.CreateIndex(
                name: "IX_master_checklist_versions_status_id",
                table: "master_checklist_versions",
                column: "status_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "master_checklist_items");

            migrationBuilder.DropTable(
                name: "master_checklist_versions");

            migrationBuilder.CreateTable(
                name: "checklist_versions",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    status_id = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    created_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    version_number = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_checklist_versions", x => x.id);
                    table.ForeignKey(
                        name: "FK_checklist_versions_checklist_statuses_status_id",
                        column: x => x.status_id,
                        principalTable: "checklist_statuses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "checklist_items",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    category_id = table.Column<long>(type: "bigint", nullable: false),
                    version_id = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    created_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    display_order = table.Column<int>(type: "integer", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    is_mandatory = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    is_retired = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_checklist_items", x => x.id);
                    table.ForeignKey(
                        name: "FK_checklist_items_checklist_categories_category_id",
                        column: x => x.category_id,
                        principalTable: "checklist_categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_checklist_items_checklist_versions_version_id",
                        column: x => x.version_id,
                        principalTable: "checklist_versions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "checklist_versions",
                columns: new[] { "id", "created_at", "created_by", "status_id", "updated_at", "updated_by", "version_number" },
                values: new object[] { 1L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1 });

            migrationBuilder.InsertData(
                table: "checklist_items",
                columns: new[] { "id", "category_id", "created_at", "created_by", "description", "display_order", "is_mandatory", "title", "updated_at", "updated_by", "version_id" },
                values: new object[,]
                {
                    { 1L, 1L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Confirm the signed commercial contract is on file before kickoff.", 1, true, "Sign commercial contract", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1L },
                    { 2L, 1L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Verify the approved budget matches the statement of work.", 2, true, "Confirm engagement budget", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1L },
                    { 3L, 1L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Agree invoicing milestones and payment terms with the client.", 3, true, "Confirm billing schedule", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1L },
                    { 4L, 2L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Sales team walks delivery team through client history and expectations.", 4, true, "Complete sales-to-delivery handover call", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1L },
                    { 5L, 2L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Ensure delivery has access to all pre-sales notes and correspondence.", 5, true, "Share CRM opportunity notes", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1L },
                    { 6L, 2L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Confirm delivery team has contact details for all client stakeholders.", 6, true, "Transfer key client contacts", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1L },
                    { 7L, 3L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Capture in-scope and out-of-scope items in the kickoff scope document.", 7, true, "Document engagement scope", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1L },
                    { 8L, 3L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "List and confirm all deliverables committed to the client.", 8, true, "Confirm deliverables list", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1L }
                });

            migrationBuilder.InsertData(
                table: "checklist_items",
                columns: new[] { "id", "category_id", "created_at", "created_by", "description", "display_order", "title", "updated_at", "updated_by", "version_id" },
                values: new object[] { 9L, 3L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Record assumptions made during the sales cycle that affect scope.", 9, "Identify scope assumptions and exclusions", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1L });

            migrationBuilder.InsertData(
                table: "checklist_items",
                columns: new[] { "id", "category_id", "created_at", "created_by", "description", "display_order", "is_mandatory", "title", "updated_at", "updated_by", "version_id" },
                values: new object[,]
                {
                    { 10L, 4L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Agree recurring status meetings and escalation paths.", 10, true, "Set up engagement governance cadence", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1L },
                    { 11L, 4L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Identify decision-makers on both client and delivery sides.", 11, true, "Assign engagement steering committee", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1L },
                    { 12L, 4L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Agree how scope changes will be raised, approved, and tracked.", 12, true, "Define change control process", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1L }
                });

            migrationBuilder.InsertData(
                table: "checklist_items",
                columns: new[] { "id", "category_id", "created_at", "created_by", "description", "display_order", "title", "updated_at", "updated_by", "version_id" },
                values: new object[] { 13L, 4L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Assign an owner to maintain the Risks, Assumptions, Issues, and Dependencies log.", 13, "Confirm RAID log owner", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1L });

            migrationBuilder.InsertData(
                table: "checklist_items",
                columns: new[] { "id", "category_id", "created_at", "created_by", "description", "display_order", "is_mandatory", "title", "updated_at", "updated_by", "version_id" },
                values: new object[,]
                {
                    { 14L, 5L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Confirm named resources are assigned and available for the engagement.", 14, true, "Allocate delivery team members", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1L },
                    { 15L, 5L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Check assigned resources have capacity for the engagement duration.", 15, true, "Confirm resource availability calendar", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1L }
                });

            migrationBuilder.InsertData(
                table: "checklist_items",
                columns: new[] { "id", "category_id", "created_at", "created_by", "description", "display_order", "title", "updated_at", "updated_by", "version_id" },
                values: new object[] { 16L, 5L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Name backup resources in case of absence during the engagement.", 16, "Identify backup/on-call resources", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1L });

            migrationBuilder.InsertData(
                table: "checklist_items",
                columns: new[] { "id", "category_id", "created_at", "created_by", "description", "display_order", "is_mandatory", "title", "updated_at", "updated_by", "version_id" },
                values: new object[,]
                {
                    { 17L, 6L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Request and confirm access to required client systems and environments.", 17, true, "Provision environment access", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1L },
                    { 18L, 6L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Confirm client infrastructure and tooling meet engagement requirements.", 18, true, "Verify technical prerequisites", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1L },
                    { 19L, 6L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Ensure delivery team has access to repositories and pipelines.", 19, true, "Set up source control and CI/CD access", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1L }
                });

            migrationBuilder.InsertData(
                table: "checklist_items",
                columns: new[] { "id", "category_id", "created_at", "created_by", "description", "display_order", "title", "updated_at", "updated_by", "version_id" },
                values: new object[] { 20L, 6L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Review any client-specific security or compliance obligations.", 20, "Confirm security and compliance requirements", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1L });

            migrationBuilder.CreateIndex(
                name: "IX_checklist_items_category_id",
                table: "checklist_items",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "IX_checklist_items_version_id",
                table: "checklist_items",
                column: "version_id");

            migrationBuilder.CreateIndex(
                name: "IX_checklist_versions_status_id",
                table: "checklist_versions",
                column: "status_id");
        }
    }
}
