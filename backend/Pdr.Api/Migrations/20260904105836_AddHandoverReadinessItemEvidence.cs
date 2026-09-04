using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pdr.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddHandoverReadinessItemEvidence : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EvidenceDocumentId",
                table: "HandoverReadinessItems",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EvidenceSubmittedAt",
                table: "HandoverReadinessItems",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EvidenceSubmittedById",
                table: "HandoverReadinessItems",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_HandoverReadinessItems_EvidenceDocumentId",
                table: "HandoverReadinessItems",
                column: "EvidenceDocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_HandoverReadinessItems_EvidenceSubmittedById",
                table: "HandoverReadinessItems",
                column: "EvidenceSubmittedById");

            migrationBuilder.AddForeignKey(
                name: "FK_HandoverReadinessItems_documents_EvidenceDocumentId",
                table: "HandoverReadinessItems",
                column: "EvidenceDocumentId",
                principalTable: "documents",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_HandoverReadinessItems_users_EvidenceSubmittedById",
                table: "HandoverReadinessItems",
                column: "EvidenceSubmittedById",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HandoverReadinessItems_documents_EvidenceDocumentId",
                table: "HandoverReadinessItems");

            migrationBuilder.DropForeignKey(
                name: "FK_HandoverReadinessItems_users_EvidenceSubmittedById",
                table: "HandoverReadinessItems");

            migrationBuilder.DropIndex(
                name: "IX_HandoverReadinessItems_EvidenceDocumentId",
                table: "HandoverReadinessItems");

            migrationBuilder.DropIndex(
                name: "IX_HandoverReadinessItems_EvidenceSubmittedById",
                table: "HandoverReadinessItems");

            migrationBuilder.DropColumn(
                name: "EvidenceDocumentId",
                table: "HandoverReadinessItems");

            migrationBuilder.DropColumn(
                name: "EvidenceSubmittedAt",
                table: "HandoverReadinessItems");

            migrationBuilder.DropColumn(
                name: "EvidenceSubmittedById",
                table: "HandoverReadinessItems");
        }
    }
}
