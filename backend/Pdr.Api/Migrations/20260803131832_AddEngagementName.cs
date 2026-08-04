using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pdr.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddEngagementName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EngagementName",
                table: "engagements",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "engagements",
                keyColumn: "engagement_id",
                keyValue: 1,
                column: "EngagementName",
                value: "");

            migrationBuilder.UpdateData(
                table: "engagements",
                keyColumn: "engagement_id",
                keyValue: 2,
                column: "EngagementName",
                value: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EngagementName",
                table: "engagements");
        }
    }
}
