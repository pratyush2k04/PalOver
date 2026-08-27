using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pdr.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddReviewedHandoverStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "handover_statuses",
                columns: new[] { "handover_status_id", "status_name" },
                values: new object[] { 3, "Reviewed" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "handover_statuses",
                keyColumn: "handover_status_id",
                keyValue: 3);
        }
    }
}
