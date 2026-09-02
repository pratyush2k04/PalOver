using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Pdr.Api.Migrations
{
    /// <inheritdoc />
    public partial class HandoverReviewThread : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_handover_queries_users_responded_by_user_id",
                table: "handover_queries");

            migrationBuilder.DropTable(
                name: "handover_query_questions");

            migrationBuilder.DropTable(
                name: "handover_query_responses");

            migrationBuilder.DropIndex(
                name: "IX_handover_queries_responded_by_user_id",
                table: "handover_queries");

            migrationBuilder.DropColumn(
                name: "responded_by_user_id",
                table: "handover_queries");

            migrationBuilder.CreateTable(
                name: "handover_query_messages",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    query_id = table.Column<int>(type: "integer", nullable: false),
                    sender_user_id = table.Column<int>(type: "integer", nullable: false),
                    message_type = table.Column<int>(type: "integer", nullable: false),
                    message_text = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    sequence_number = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    created_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    updated_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_handover_query_messages", x => x.id);
                    table.ForeignKey(
                        name: "FK_handover_query_messages_handover_queries_query_id",
                        column: x => x.query_id,
                        principalTable: "handover_queries",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_handover_query_messages_users_sender_user_id",
                        column: x => x.sender_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_handover_query_messages_query_id_sequence_number",
                table: "handover_query_messages",
                columns: new[] { "query_id", "sequence_number" });

            migrationBuilder.CreateIndex(
                name: "IX_handover_query_messages_sender_user_id",
                table: "handover_query_messages",
                column: "sender_user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "handover_query_messages");

            migrationBuilder.AddColumn<int>(
                name: "responded_by_user_id",
                table: "handover_queries",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "handover_query_questions",
                columns: table => new
                {
                    query_id = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    created_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    question_text = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_handover_query_questions", x => x.query_id);
                    table.ForeignKey(
                        name: "FK_handover_query_questions_handover_queries_query_id",
                        column: x => x.query_id,
                        principalTable: "handover_queries",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "handover_query_responses",
                columns: table => new
                {
                    query_id = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    created_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    response_text = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_handover_query_responses", x => x.query_id);
                    table.ForeignKey(
                        name: "FK_handover_query_responses_handover_queries_query_id",
                        column: x => x.query_id,
                        principalTable: "handover_queries",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_handover_queries_responded_by_user_id",
                table: "handover_queries",
                column: "responded_by_user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_handover_queries_users_responded_by_user_id",
                table: "handover_queries",
                column: "responded_by_user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
