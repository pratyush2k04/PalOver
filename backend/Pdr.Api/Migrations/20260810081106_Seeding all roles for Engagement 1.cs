using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Pdr.Api.Migrations
{
    /// <inheritdoc />
    public partial class SeedingallrolesforEngagement1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "engagement_assignments",
                columns: new[] { "engagement_id", "user_id" },
                values: new object[,]
                {
                    { 1, 3 },
                    { 1, 4 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "engagement_assignments",
                keyColumns: new[] { "engagement_id", "user_id" },
                keyValues: new object[] { 1, 3 });

            migrationBuilder.DeleteData(
                table: "engagement_assignments",
                keyColumns: new[] { "engagement_id", "user_id" },
                keyValues: new object[] { 1, 4 });
        }
    }
}
