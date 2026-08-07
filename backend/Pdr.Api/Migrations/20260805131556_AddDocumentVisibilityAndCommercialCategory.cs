using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pdr.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddDocumentVisibilityAndCommercialCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "visibility",
                table: "documents",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "Team-wide");

            migrationBuilder.InsertData(
                table: "document_categories",
                columns: new[] { "id", "category_name" },
                values: new object[] { 7, "Commercial" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "document_categories",
                keyColumn: "id",
                keyValue: 7);

            migrationBuilder.DropColumn(
                name: "visibility",
                table: "documents");
        }
    }
}
