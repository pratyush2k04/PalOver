using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pdr.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddChecklistItemEvidenceDocument : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "evidence_link",
                table: "evidence_snapshot_items");

            migrationBuilder.DropColumn(
                name: "evidence_note",
                table: "evidence_snapshot_items");

            migrationBuilder.DropColumn(
                name: "evidence_link",
                table: "engagement_checklist_items");

            migrationBuilder.DropColumn(
                name: "evidence_note",
                table: "engagement_checklist_items");

            migrationBuilder.AddColumn<string>(
                name: "evidence_document_file_name",
                table: "evidence_snapshot_items",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "evidence_document_id",
                table: "evidence_snapshot_items",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "evidence_document_id",
                table: "engagement_checklist_items",
                type: "integer",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 1,
                column: "evidence_document_id",
                value: null);

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 2,
                column: "evidence_document_id",
                value: null);

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 3,
                column: "evidence_document_id",
                value: null);

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 4,
                column: "evidence_document_id",
                value: null);

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 5,
                column: "evidence_document_id",
                value: null);

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 6,
                column: "evidence_document_id",
                value: null);

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 7,
                column: "evidence_document_id",
                value: null);

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 8,
                column: "evidence_document_id",
                value: null);

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 9,
                column: "evidence_document_id",
                value: null);

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 10,
                column: "evidence_document_id",
                value: null);

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 11,
                column: "evidence_document_id",
                value: null);

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 12,
                column: "evidence_document_id",
                value: null);

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 13,
                column: "evidence_document_id",
                value: null);

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 14,
                column: "evidence_document_id",
                value: null);

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 15,
                column: "evidence_document_id",
                value: null);

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 16,
                column: "evidence_document_id",
                value: null);

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 17,
                column: "evidence_document_id",
                value: null);

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 18,
                column: "evidence_document_id",
                value: null);

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 19,
                column: "evidence_document_id",
                value: null);

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 20,
                column: "evidence_document_id",
                value: null);

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 21,
                column: "evidence_document_id",
                value: null);

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 22,
                column: "evidence_document_id",
                value: null);

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 23,
                column: "evidence_document_id",
                value: null);

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 24,
                column: "evidence_document_id",
                value: null);

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 25,
                column: "evidence_document_id",
                value: null);

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 26,
                column: "evidence_document_id",
                value: null);

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 27,
                column: "evidence_document_id",
                value: null);

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 28,
                column: "evidence_document_id",
                value: null);

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 29,
                column: "evidence_document_id",
                value: null);

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 30,
                column: "evidence_document_id",
                value: null);

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 31,
                column: "evidence_document_id",
                value: null);

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 32,
                column: "evidence_document_id",
                value: null);

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 33,
                column: "evidence_document_id",
                value: null);

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 34,
                column: "evidence_document_id",
                value: null);

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 35,
                column: "evidence_document_id",
                value: null);

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 36,
                column: "evidence_document_id",
                value: null);

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 37,
                column: "evidence_document_id",
                value: null);

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 38,
                column: "evidence_document_id",
                value: null);

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 39,
                column: "evidence_document_id",
                value: null);

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 40,
                column: "evidence_document_id",
                value: null);

            migrationBuilder.CreateIndex(
                name: "IX_engagement_checklist_items_evidence_document_id",
                table: "engagement_checklist_items",
                column: "evidence_document_id");

            migrationBuilder.AddForeignKey(
                name: "FK_engagement_checklist_items_documents_evidence_document_id",
                table: "engagement_checklist_items",
                column: "evidence_document_id",
                principalTable: "documents",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_engagement_checklist_items_documents_evidence_document_id",
                table: "engagement_checklist_items");

            migrationBuilder.DropIndex(
                name: "IX_engagement_checklist_items_evidence_document_id",
                table: "engagement_checklist_items");

            migrationBuilder.DropColumn(
                name: "evidence_document_file_name",
                table: "evidence_snapshot_items");

            migrationBuilder.DropColumn(
                name: "evidence_document_id",
                table: "evidence_snapshot_items");

            migrationBuilder.DropColumn(
                name: "evidence_document_id",
                table: "engagement_checklist_items");

            migrationBuilder.AddColumn<string>(
                name: "evidence_link",
                table: "evidence_snapshot_items",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "evidence_note",
                table: "evidence_snapshot_items",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "evidence_link",
                table: "engagement_checklist_items",
                type: "character varying(2048)",
                maxLength: 2048,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "evidence_note",
                table: "engagement_checklist_items",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 1,
                columns: new[] { "evidence_link", "evidence_note" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 2,
                columns: new[] { "evidence_link", "evidence_note" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 3,
                columns: new[] { "evidence_link", "evidence_note" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 4,
                columns: new[] { "evidence_link", "evidence_note" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 5,
                columns: new[] { "evidence_link", "evidence_note" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 6,
                columns: new[] { "evidence_link", "evidence_note" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 7,
                columns: new[] { "evidence_link", "evidence_note" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 8,
                columns: new[] { "evidence_link", "evidence_note" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 9,
                columns: new[] { "evidence_link", "evidence_note" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 10,
                columns: new[] { "evidence_link", "evidence_note" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 11,
                columns: new[] { "evidence_link", "evidence_note" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 12,
                columns: new[] { "evidence_link", "evidence_note" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 13,
                columns: new[] { "evidence_link", "evidence_note" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 14,
                columns: new[] { "evidence_link", "evidence_note" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 15,
                columns: new[] { "evidence_link", "evidence_note" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 16,
                columns: new[] { "evidence_link", "evidence_note" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 17,
                columns: new[] { "evidence_link", "evidence_note" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 18,
                columns: new[] { "evidence_link", "evidence_note" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 19,
                columns: new[] { "evidence_link", "evidence_note" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 20,
                columns: new[] { "evidence_link", "evidence_note" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 21,
                columns: new[] { "evidence_link", "evidence_note" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 22,
                columns: new[] { "evidence_link", "evidence_note" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 23,
                columns: new[] { "evidence_link", "evidence_note" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 24,
                columns: new[] { "evidence_link", "evidence_note" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 25,
                columns: new[] { "evidence_link", "evidence_note" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 26,
                columns: new[] { "evidence_link", "evidence_note" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 27,
                columns: new[] { "evidence_link", "evidence_note" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 28,
                columns: new[] { "evidence_link", "evidence_note" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 29,
                columns: new[] { "evidence_link", "evidence_note" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 30,
                columns: new[] { "evidence_link", "evidence_note" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 31,
                columns: new[] { "evidence_link", "evidence_note" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 32,
                columns: new[] { "evidence_link", "evidence_note" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 33,
                columns: new[] { "evidence_link", "evidence_note" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 34,
                columns: new[] { "evidence_link", "evidence_note" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 35,
                columns: new[] { "evidence_link", "evidence_note" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 36,
                columns: new[] { "evidence_link", "evidence_note" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 37,
                columns: new[] { "evidence_link", "evidence_note" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 38,
                columns: new[] { "evidence_link", "evidence_note" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 39,
                columns: new[] { "evidence_link", "evidence_note" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "engagement_checklist_items",
                keyColumn: "id",
                keyValue: 40,
                columns: new[] { "evidence_link", "evidence_note" },
                values: new object[] { null, null });
        }
    }
}
