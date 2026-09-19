using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Pdr.Api.Migrations
{
    /// <inheritdoc />
    public partial class SplitUserAndEngagementRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_users_roles_role_id",
                table: "users");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_engagement_assignments",
                table: "engagement_assignments");

            migrationBuilder.AddColumn<int>(
                name: "engagement_role_id",
                table: "engagement_assignments",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_engagement_assignments",
                table: "engagement_assignments",
                columns: new[] { "user_id", "engagement_id", "engagement_role_id" });

            migrationBuilder.CreateTable(
                name: "engagement_roles",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    created_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    updated_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_engagement_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "user_roles",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    created_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    updated_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_roles", x => x.id);
                });

            migrationBuilder.InsertData(
                table: "engagement_roles",
                columns: new[] { "id", "created_at", "created_by", "name", "updated_at", "updated_by" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Pre Sales", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Leadership", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Project Manager", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Tech Lead", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null }
                });

            migrationBuilder.InsertData(
                table: "user_roles",
                columns: new[] { "id", "created_at", "created_by", "name", "updated_at", "updated_by" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Pre Sales", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Leadership", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Delivery", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null }
                });

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: 2,
                column: "role_id",
                value: 3);

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: 4,
                column: "role_id",
                value: 2);

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: 5,
                column: "role_id",
                value: 2);

            migrationBuilder.CreateIndex(
                name: "IX_engagement_assignments_engagement_role_id",
                table: "engagement_assignments",
                column: "engagement_role_id");

            migrationBuilder.CreateIndex(
                name: "IX_engagement_roles_name",
                table: "engagement_roles",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_roles_name",
                table: "user_roles",
                column: "name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_engagement_assignments_engagement_roles_engagement_role_id",
                table: "engagement_assignments",
                column: "engagement_role_id",
                principalTable: "engagement_roles",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_users_user_roles_role_id",
                table: "users",
                column: "role_id",
                principalTable: "user_roles",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_engagement_assignments_engagement_roles_engagement_role_id",
                table: "engagement_assignments");

            migrationBuilder.DropForeignKey(
                name: "FK_users_user_roles_role_id",
                table: "users");

            migrationBuilder.DropTable(
                name: "engagement_roles");

            migrationBuilder.DropTable(
                name: "user_roles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_engagement_assignments",
                table: "engagement_assignments");

            migrationBuilder.DropIndex(
                name: "IX_engagement_assignments_engagement_role_id",
                table: "engagement_assignments");

            migrationBuilder.DropColumn(
                name: "engagement_role_id",
                table: "engagement_assignments");

            migrationBuilder.AddPrimaryKey(
                name: "PK_engagement_assignments",
                table: "engagement_assignments",
                columns: new[] { "user_id", "engagement_id" });

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    created_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.id);
                });

            migrationBuilder.InsertData(
                table: "roles",
                columns: new[] { "id", "created_at", "created_by", "name", "updated_at", "updated_by" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Pre Sales", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Project Management", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Tech Lead", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Leadership", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null }
                });

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: 2,
                column: "role_id",
                value: 2);

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: 4,
                column: "role_id",
                value: 4);

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: 5,
                column: "role_id",
                value: 4);

            migrationBuilder.CreateIndex(
                name: "IX_roles_name",
                table: "roles",
                column: "name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_users_roles_role_id",
                table: "users",
                column: "role_id",
                principalTable: "roles",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
