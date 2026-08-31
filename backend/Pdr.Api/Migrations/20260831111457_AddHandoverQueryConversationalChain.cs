using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Pdr.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddHandoverQueryConversationalChain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            // Migrate existing questions
            migrationBuilder.Sql(@"
                INSERT INTO handover_query_messages (query_id, sender_user_id, message_type, message_text, sequence_number, created_at, updated_at, created_by, updated_by, is_deleted)
                SELECT q.query_id, hq.raised_by_user_id, 1, q.question_text, 1, q.created_at, q.updated_at, q.created_by, q.updated_by, q.is_deleted
                FROM handover_query_questions q
                JOIN handover_queries hq ON hq.id = q.query_id;
            ");

            // Migrate existing responses
            migrationBuilder.Sql(@"
                INSERT INTO handover_query_messages (query_id, sender_user_id, message_type, message_text, sequence_number, created_at, updated_at, created_by, updated_by, is_deleted)
                SELECT r.query_id, COALESCE(hq.responded_by_user_id, hq.raised_by_user_id), 2, r.response_text, 2, r.created_at, r.updated_at, r.created_by, r.updated_by, r.is_deleted
                FROM handover_query_responses r
                JOIN handover_queries hq ON hq.id = r.query_id;
            ");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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

            // Backfill questions from messages
            migrationBuilder.Sql(@"
                INSERT INTO handover_query_questions (query_id, question_text, created_at, updated_at, created_by, updated_by, is_deleted)
                SELECT query_id, message_text, created_at, updated_at, created_by, updated_by, is_deleted
                FROM handover_query_messages
                WHERE message_type = 1 AND sequence_number = 1;
            ");

            // Backfill responses from messages
            migrationBuilder.Sql(@"
                INSERT INTO handover_query_responses (query_id, response_text, created_at, updated_at, created_by, updated_by, is_deleted)
                SELECT query_id, message_text, created_at, updated_at, created_by, updated_by, is_deleted
                FROM handover_query_messages
                WHERE message_type = 2
                ORDER BY sequence_number DESC
                ON CONFLICT DO NOTHING;
            ");

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

            migrationBuilder.DropTable(
                name: "handover_query_messages");
        }
    }
}
