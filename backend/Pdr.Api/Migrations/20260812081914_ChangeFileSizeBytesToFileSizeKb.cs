using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pdr.Api.Migrations
{
    /// <inheritdoc />
    public partial class ChangeFileSizeBytesToFileSizeKb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "file_size_bytes",
                table: "documents");

            migrationBuilder.DropColumn(
                name: "file_size_bytes",
                table: "document_versions");

            migrationBuilder.AddColumn<int>(
                name: "file_size_kb",
                table: "documents",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "file_size_kb",
                table: "document_versions",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "file_size_kb",
                table: "documents");

            migrationBuilder.DropColumn(
                name: "file_size_kb",
                table: "document_versions");

            migrationBuilder.AddColumn<long>(
                name: "file_size_bytes",
                table: "documents",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "file_size_bytes",
                table: "document_versions",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
