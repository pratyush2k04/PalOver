using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Pdr.Api.Migrations
{
    /// <inheritdoc />
    public partial class Schema_Redesign : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_checklist_versions_version_statuses_status_id",
                table: "checklist_versions");

            migrationBuilder.DropTable(
                name: "role_permissions");

            migrationBuilder.DropTable(
                name: "version_statuses");

            migrationBuilder.DropIndex(
                name: "IX_checklist_categories_code",
                table: "checklist_categories");

            migrationBuilder.DropColumn(
                name: "code",
                table: "checklist_categories");

            migrationBuilder.RenameColumn(
                name: "isAdmin",
                table: "users",
                newName: "is_admin");

            migrationBuilder.RenameColumn(
                name: "display_name",
                table: "checklist_categories",
                newName: "category_name");

            migrationBuilder.AddColumn<string>(
                name: "created_by",
                table: "users",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "updated_by",
                table: "users",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "roles",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "roles",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "NOW()");

            migrationBuilder.AddColumn<string>(
                name: "created_by",
                table: "roles",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "roles",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_at",
                table: "roles",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "NOW()");

            migrationBuilder.AddColumn<string>(
                name: "updated_by",
                table: "roles",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserId1",
                table: "engagement_assignments",
                type: "integer",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "created_by",
                table: "checklist_versions",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "checklist_versions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "NOW()");

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "checklist_versions",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_at",
                table: "checklist_versions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "NOW()");

            migrationBuilder.AddColumn<string>(
                name: "updated_by",
                table: "checklist_versions",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "checklist_items",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "NOW()");

            migrationBuilder.AddColumn<string>(
                name: "created_by",
                table: "checklist_items",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "checklist_items",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_at",
                table: "checklist_items",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "NOW()");

            migrationBuilder.AddColumn<string>(
                name: "updated_by",
                table: "checklist_items",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "checklist_statuses",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    status_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_checklist_statuses", x => x.id);
                });

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "id",
                keyValue: 1,
                columns: new[] { "created_at", "created_by", "updated_at", "updated_by" },
                values: new object[] { new DateTime(2026, 7, 30, 12, 51, 18, 901, DateTimeKind.Utc).AddTicks(1005), "System", new DateTime(2026, 7, 30, 12, 51, 18, 901, DateTimeKind.Utc).AddTicks(1008), null });

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "id",
                keyValue: 2,
                columns: new[] { "created_at", "created_by", "updated_at", "updated_by" },
                values: new object[] { new DateTime(2026, 7, 30, 12, 51, 18, 901, DateTimeKind.Utc).AddTicks(1903), "System", new DateTime(2026, 7, 30, 12, 51, 18, 901, DateTimeKind.Utc).AddTicks(1904), null });

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "id",
                keyValue: 3,
                columns: new[] { "created_at", "created_by", "updated_at", "updated_by" },
                values: new object[] { new DateTime(2026, 7, 30, 12, 51, 18, 901, DateTimeKind.Utc).AddTicks(1905), "System", new DateTime(2026, 7, 30, 12, 51, 18, 901, DateTimeKind.Utc).AddTicks(1906), null });

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "id",
                keyValue: 4,
                columns: new[] { "created_at", "created_by", "updated_at", "updated_by" },
                values: new object[] { new DateTime(2026, 7, 30, 12, 51, 18, 901, DateTimeKind.Utc).AddTicks(1907), "System", new DateTime(2026, 7, 30, 12, 51, 18, 901, DateTimeKind.Utc).AddTicks(1907), null });

            migrationBuilder.CreateIndex(
                name: "IX_engagement_assignments_UserId1",
                table: "engagement_assignments",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_checklist_versions_checklist_statuses_status_id",
                table: "checklist_versions",
                column: "status_id",
                principalTable: "checklist_statuses",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_engagement_assignments_users_UserId1",
                table: "engagement_assignments",
                column: "UserId1",
                principalTable: "users",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_checklist_versions_checklist_statuses_status_id",
                table: "checklist_versions");

            migrationBuilder.DropForeignKey(
                name: "FK_engagement_assignments_users_UserId1",
                table: "engagement_assignments");

            migrationBuilder.DropTable(
                name: "checklist_statuses");

            migrationBuilder.DropIndex(
                name: "IX_engagement_assignments_UserId1",
                table: "engagement_assignments");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "users");

            migrationBuilder.DropColumn(
                name: "is_deleted",
                table: "users");

            migrationBuilder.DropColumn(
                name: "updated_by",
                table: "users");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "roles");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "roles");

            migrationBuilder.DropColumn(
                name: "is_deleted",
                table: "roles");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "roles");

            migrationBuilder.DropColumn(
                name: "updated_by",
                table: "roles");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "engagement_assignments");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "checklist_versions");

            migrationBuilder.DropColumn(
                name: "is_deleted",
                table: "checklist_versions");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "checklist_versions");

            migrationBuilder.DropColumn(
                name: "updated_by",
                table: "checklist_versions");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "checklist_items");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "checklist_items");

            migrationBuilder.DropColumn(
                name: "is_deleted",
                table: "checklist_items");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "checklist_items");

            migrationBuilder.DropColumn(
                name: "updated_by",
                table: "checklist_items");

            migrationBuilder.RenameColumn(
                name: "is_admin",
                table: "users",
                newName: "isAdmin");

            migrationBuilder.RenameColumn(
                name: "category_name",
                table: "checklist_categories",
                newName: "display_name");

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "roles",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<long>(
                name: "created_by",
                table: "checklist_versions",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "code",
                table: "checklist_categories",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

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
                name: "IX_version_statuses_code",
                table: "version_statuses",
                column: "code",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_checklist_versions_version_statuses_status_id",
                table: "checklist_versions",
                column: "status_id",
                principalTable: "version_statuses",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
