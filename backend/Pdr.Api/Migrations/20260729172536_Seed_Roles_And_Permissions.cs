using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Pdr.Api.Migrations
{
    /// <inheritdoc />
    public partial class Seed_Roles_And_Permissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Notes");

            migrationBuilder.CreateTable(
                name: "checklist_categories",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    display_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_checklist_categories", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "version_statuses",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    display_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_version_statuses", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "role_permissions",
                columns: table => new
                {
                    role_id = table.Column<int>(type: "integer", nullable: false),
                    permission = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_role_permissions", x => new { x.role_id, x.permission });
                    table.ForeignKey(
                        name: "FK_role_permissions_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    full_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    password = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    role_id = table.Column<int>(type: "integer", nullable: false),
                    isAdmin = table.Column<bool>(type: "boolean", nullable: false),
                    failed_attempt_count = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    lockout_end = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                    table.ForeignKey(
                        name: "FK_users_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "checklist_versions",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    version_number = table.Column<int>(type: "integer", nullable: false),
                    status_id = table.Column<long>(type: "bigint", nullable: false),
                    created_by = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_checklist_versions", x => x.id);
                    table.ForeignKey(
                        name: "FK_checklist_versions_version_statuses_status_id",
                        column: x => x.status_id,
                        principalTable: "version_statuses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "engagement_assignments",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    engagement_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_engagement_assignments", x => new { x.user_id, x.engagement_id });
                    table.ForeignKey(
                        name: "FK_engagement_assignments_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "checklist_items",
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
                    is_retired = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
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
                table: "roles",
                columns: new[] { "id", "created_at", "name", "updated_at" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 7, 29, 17, 25, 36, 200, DateTimeKind.Utc).AddTicks(3146), "Pre Sales", new DateTime(2026, 7, 29, 17, 25, 36, 200, DateTimeKind.Utc).AddTicks(3149) },
                    { 2, new DateTime(2026, 7, 29, 17, 25, 36, 200, DateTimeKind.Utc).AddTicks(3502), "Project Management", new DateTime(2026, 7, 29, 17, 25, 36, 200, DateTimeKind.Utc).AddTicks(3502) },
                    { 3, new DateTime(2026, 7, 29, 17, 25, 36, 200, DateTimeKind.Utc).AddTicks(3503), "Tech Lead", new DateTime(2026, 7, 29, 17, 25, 36, 200, DateTimeKind.Utc).AddTicks(3503) },
                    { 4, new DateTime(2026, 7, 29, 17, 25, 36, 200, DateTimeKind.Utc).AddTicks(3504), "Leadership", new DateTime(2026, 7, 29, 17, 25, 36, 200, DateTimeKind.Utc).AddTicks(3504) }
                });

            migrationBuilder.InsertData(
                table: "role_permissions",
                columns: new[] { "permission", "role_id" },
                values: new object[,]
                {
                    { "discussion:comment", 1 },
                    { "document:upload", 1 },
                    { "engagement:complete_handover", 1 },
                    { "engagement:mark_sow_signed", 1 },
                    { "engagement:record_committed_scope", 1 },
                    { "risk:raise", 1 },
                    { "checklist:request_waiver", 2 },
                    { "checklist:tailor", 2 },
                    { "discussion:comment", 2 },
                    { "document:set_visibility", 2 },
                    { "document:upload", 2 },
                    { "engagement:query_handover", 2 },
                    { "engagement:record_realistic_scope", 2 },
                    { "engagement:set_commercial_model", 2 },
                    { "engagement:view_own", 2 },
                    { "governance:plan", 2 },
                    { "one_pager:generate", 2 },
                    { "resource:request", 2 },
                    { "risk:raise", 2 },
                    { "signoff:request", 2 },
                    { "stakeholder:manage", 2 },
                    { "checklist:request_waiver", 3 },
                    { "checklist:tailor_tech", 3 },
                    { "discussion:comment", 3 },
                    { "document:upload", 3 },
                    { "engagement:query_handover", 3 },
                    { "engagement:record_realistic_scope", 3 },
                    { "engagement:view_own", 3 },
                    { "risk:raise", 3 },
                    { "tech_access:track", 3 },
                    { "tech_decision:log", 3 },
                    { "audit:view_engagement", 4 },
                    { "change_request:approve", 4 },
                    { "checklist:approve_waiver", 4 },
                    { "dashboard:view_portfolio", 4 },
                    { "discussion:comment", 4 },
                    { "engagement:reassign_team", 4 },
                    { "report:run_kpi", 4 },
                    { "resource:confirm", 4 },
                    { "signoff:approve", 4 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_checklist_categories_code",
                table: "checklist_categories",
                column: "code",
                unique: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_roles_name",
                table: "roles",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_users_email",
                table: "users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_users_role_id",
                table: "users",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "IX_version_statuses_code",
                table: "version_statuses",
                column: "code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "checklist_items");

            migrationBuilder.DropTable(
                name: "engagement_assignments");

            migrationBuilder.DropTable(
                name: "role_permissions");

            migrationBuilder.DropTable(
                name: "checklist_categories");

            migrationBuilder.DropTable(
                name: "checklist_versions");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "version_statuses");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.CreateTable(
                name: "Notes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Content = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notes", x => x.Id);
                });
        }
    }
}
