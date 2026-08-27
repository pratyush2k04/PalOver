using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pdr.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddHandoverQueryActorColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "raised_by_user_id",
                table: "handover_queries",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "responded_by_user_id",
                table: "handover_queries",
                type: "integer",
                nullable: true);

            migrationBuilder.Sql(
                "UPDATE handover_queries SET raised_by_user_id = CAST(created_by AS integer) " +
                "WHERE raised_by_user_id = 0 AND created_by ~ '^[0-9]+$';");

            migrationBuilder.CreateIndex(
                name: "IX_handover_queries_raised_by_user_id",
                table: "handover_queries",
                column: "raised_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_handover_queries_responded_by_user_id",
                table: "handover_queries",
                column: "responded_by_user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_handover_queries_users_raised_by_user_id",
                table: "handover_queries",
                column: "raised_by_user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_handover_queries_users_responded_by_user_id",
                table: "handover_queries",
                column: "responded_by_user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_handover_queries_users_raised_by_user_id",
                table: "handover_queries");

            migrationBuilder.DropForeignKey(
                name: "FK_handover_queries_users_responded_by_user_id",
                table: "handover_queries");

            migrationBuilder.DropIndex(
                name: "IX_handover_queries_raised_by_user_id",
                table: "handover_queries");

            migrationBuilder.DropIndex(
                name: "IX_handover_queries_responded_by_user_id",
                table: "handover_queries");

            migrationBuilder.DropColumn(
                name: "raised_by_user_id",
                table: "handover_queries");

            migrationBuilder.DropColumn(
                name: "responded_by_user_id",
                table: "handover_queries");
        }
    }
}
