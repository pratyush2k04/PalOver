using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pdr.Api.Migrations
{
    /// <inheritdoc />
    public partial class RealisticCommitments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.UpdateData(
                table: "commitments",
                keyColumn: "commitment_id",
                keyValue: 1,
                columns: new[] { "timeline", "timeline_unit", "title" },
                values: new object[] { 3, "Months", "MVP Banking Module Delivery" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "realistic_commitments");

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
        }
    }
}
