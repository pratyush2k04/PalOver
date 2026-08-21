using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Pdr.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddEvidenceSnapshot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "evidence_snapshots",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    engagement_id = table.Column<int>(type: "integer", nullable: false),
                    signed_off_by_user_id = table.Column<int>(type: "integer", nullable: false),
                    signed_off_by_user_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    signed_off_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    billing_start_date = table.Column<DateOnly>(type: "date", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    created_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    updated_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_evidence_snapshots", x => x.id);
                    table.ForeignKey(
                        name: "FK_evidence_snapshots_engagements_engagement_id",
                        column: x => x.engagement_id,
                        principalTable: "engagements",
                        principalColumn: "engagement_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_evidence_snapshots_users_signed_off_by_user_id",
                        column: x => x.signed_off_by_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "evidence_snapshot_items",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    evidence_snapshot_id = table.Column<int>(type: "integer", nullable: false),
                    source_checklist_item_id = table.Column<int>(type: "integer", nullable: true),
                    title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    category_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    display_order = table.Column<int>(type: "integer", nullable: false),
                    status_at_sign_off = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    evidence_note = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    evidence_link = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    evidence_submitted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    evidence_submitted_by_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    created_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    updated_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_evidence_snapshot_items", x => x.id);
                    table.ForeignKey(
                        name: "FK_evidence_snapshot_items_evidence_snapshots_evidence_snapsho~",
                        column: x => x.evidence_snapshot_id,
                        principalTable: "evidence_snapshots",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_evidence_snapshot_items_evidence_snapshot_id",
                table: "evidence_snapshot_items",
                column: "evidence_snapshot_id");

            migrationBuilder.CreateIndex(
                name: "IX_evidence_snapshots_engagement_id",
                table: "evidence_snapshots",
                column: "engagement_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_evidence_snapshots_signed_off_by_user_id",
                table: "evidence_snapshots",
                column: "signed_off_by_user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "evidence_snapshot_items");

            migrationBuilder.DropTable(
                name: "evidence_snapshots");
        }
    }
}
