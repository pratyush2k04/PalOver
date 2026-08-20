using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Pdr.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddDayOneSignOffRecord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "day_one_sign_off_records",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    engagement_id = table.Column<int>(type: "integer", nullable: false),
                    signed_off_by_user_id = table.Column<int>(type: "integer", nullable: false),
                    billing_start_date = table.Column<DateOnly>(type: "date", nullable: false),
                    signed_off_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_exception = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    exception_justification = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    created_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    updated_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_day_one_sign_off_records", x => x.id);
                    table.ForeignKey(
                        name: "FK_day_one_sign_off_records_engagements_engagement_id",
                        column: x => x.engagement_id,
                        principalTable: "engagements",
                        principalColumn: "engagement_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_day_one_sign_off_records_users_signed_off_by_user_id",
                        column: x => x.signed_off_by_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_day_one_sign_off_records_engagement_id",
                table: "day_one_sign_off_records",
                column: "engagement_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_day_one_sign_off_records_signed_off_by_user_id",
                table: "day_one_sign_off_records",
                column: "signed_off_by_user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "day_one_sign_off_records");
        }
    }
}
