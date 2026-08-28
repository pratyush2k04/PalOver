using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pdr.Api.Migrations
{
    /// <inheritdoc />
    public partial class RenameHandoverQueryPrimaryKeysToId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "handover_query_field_section_id",
                table: "handover_query_field_sections",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "query_id",
                table: "handover_queries",
                newName: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "id",
                table: "handover_query_field_sections",
                newName: "handover_query_field_section_id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "handover_queries",
                newName: "query_id");
        }
    }
}
