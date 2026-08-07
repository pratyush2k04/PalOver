using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pdr.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddDocumentSoftDeleteIndexAndQueryFilter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_documents_engagement_id",
                table: "documents");

            migrationBuilder.CreateIndex(
                name: "IX_documents_engagement_id_is_deleted",
                table: "documents",
                columns: new[] { "engagement_id", "is_deleted" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_documents_engagement_id_is_deleted",
                table: "documents");

            migrationBuilder.CreateIndex(
                name: "IX_documents_engagement_id",
                table: "documents",
                column: "engagement_id");
        }
    }
}
