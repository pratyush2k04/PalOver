using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pdr.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddEvidenceSnapshotItemOwner : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "owner_id",
                table: "evidence_snapshot_items",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "owner_name",
                table: "evidence_snapshot_items",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "owner_id",
                table: "evidence_snapshot_items");

            migrationBuilder.DropColumn(
                name: "owner_name",
                table: "evidence_snapshot_items");
        }
    }
}
