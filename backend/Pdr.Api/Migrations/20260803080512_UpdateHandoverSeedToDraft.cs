using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pdr.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateHandoverSeedToDraft : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "handover_info",
                keyColumn: "handover_id",
                keyValue: 1L,
                column: "status_id",
                value: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "handover_info",
                keyColumn: "handover_id",
                keyValue: 1L,
                column: "status_id",
                value: 2);
        }
    }
}
