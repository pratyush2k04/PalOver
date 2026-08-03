using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Pdr.Api.Migrations
{
    /// <inheritdoc />
    public partial class intialcreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "checklist_categories",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    category_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_checklist_categories", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "checklist_statuses",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    status_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_checklist_statuses", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "commercial_models",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_commercial_models", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "commitment_types",
                columns: table => new
                {
                    commitment_type_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_commitment_types", x => x.commitment_type_id);
                });

            migrationBuilder.CreateTable(
                name: "document_categories",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    category_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_document_categories", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "engagement_statuses",
                columns: table => new
                {
                    status_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    status_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_engagement_statuses", x => x.status_id);
                });

            migrationBuilder.CreateTable(
                name: "engagement_types",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_engagement_types", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "handover_info",
                columns: table => new
                {
                    handover_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    engagement_id = table.Column<long>(type: "bigint", nullable: false),
                    status_id = table.Column<int>(type: "integer", nullable: false),
                    client_background = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    business_problem = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    client_sentiment = table.Column<string>(type: "text", nullable: false),
                    deal_origin = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    has_no_commitments = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    created_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    updated_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_handover_info", x => x.handover_id);
                });

            migrationBuilder.CreateTable(
                name: "handover_statuses",
                columns: table => new
                {
                    handover_status_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    status_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_handover_statuses", x => x.handover_status_id);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    created_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    updated_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "master_checklist_versions",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    version_number = table.Column<int>(type: "integer", nullable: false),
                    status_id = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    created_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    updated_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_master_checklist_versions", x => x.id);
                    table.ForeignKey(
                        name: "FK_master_checklist_versions_checklist_statuses_status_id",
                        column: x => x.status_id,
                        principalTable: "checklist_statuses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "commitments",
                columns: table => new
                {
                    commitment_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    handover_id = table.Column<long>(type: "bigint", nullable: false),
                    description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    commitment_type_id = table.Column<int>(type: "integer", nullable: false),
                    quantity = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    created_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    updated_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_commitments", x => x.commitment_id);
                    table.ForeignKey(
                        name: "FK_commitments_handover_info_handover_id",
                        column: x => x.handover_id,
                        principalTable: "handover_info",
                        principalColumn: "handover_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "engagements",
                columns: table => new
                {
                    engagement_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    client_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    engagement_description = table.Column<string>(type: "text", nullable: true),
                    engagement_type_id = table.Column<int>(type: "integer", nullable: false),
                    commercial_model_id = table.Column<int>(type: "integer", nullable: false),
                    kickoff_date = table.Column<DateOnly>(type: "date", nullable: false),
                    billing_start_date = table.Column<DateOnly>(type: "date", nullable: true),
                    status_id = table.Column<int>(type: "integer", nullable: false),
                    HandoverInfoHandoverId = table.Column<long>(type: "bigint", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    created_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    updated_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_engagements", x => x.engagement_id);
                    table.ForeignKey(
                        name: "FK_engagements_commercial_models_commercial_model_id",
                        column: x => x.commercial_model_id,
                        principalTable: "commercial_models",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_engagements_engagement_statuses_status_id",
                        column: x => x.status_id,
                        principalTable: "engagement_statuses",
                        principalColumn: "status_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_engagements_engagement_types_engagement_type_id",
                        column: x => x.engagement_type_id,
                        principalTable: "engagement_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_engagements_handover_info_HandoverInfoHandoverId",
                        column: x => x.HandoverInfoHandoverId,
                        principalTable: "handover_info",
                        principalColumn: "handover_id");
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    full_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    password = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    role_id = table.Column<int>(type: "integer", nullable: false),
                    is_admin = table.Column<bool>(type: "boolean", nullable: false),
                    failed_attempt_count = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    lockout_end = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    created_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    updated_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                    table.ForeignKey(
                        name: "FK_users_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "master_checklist_items",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    version_id = table.Column<long>(type: "bigint", nullable: false),
                    title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    category_id = table.Column<long>(type: "bigint", nullable: false),
                    is_mandatory = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    display_order = table.Column<int>(type: "integer", nullable: false),
                    is_retired = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    created_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    updated_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_master_checklist_items", x => x.id);
                    table.ForeignKey(
                        name: "FK_master_checklist_items_checklist_categories_category_id",
                        column: x => x.category_id,
                        principalTable: "checklist_categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_master_checklist_items_master_checklist_versions_version_id",
                        column: x => x.version_id,
                        principalTable: "master_checklist_versions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "documents",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    engagement_id = table.Column<int>(type: "integer", nullable: false),
                    category_id = table.Column<int>(type: "integer", nullable: false),
                    file_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    file_path = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    file_size_bytes = table.Column<long>(type: "bigint", nullable: false),
                    content_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    created_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    updated_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_documents", x => x.id);
                    table.ForeignKey(
                        name: "FK_documents_document_categories_category_id",
                        column: x => x.category_id,
                        principalTable: "document_categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_documents_engagements_engagement_id",
                        column: x => x.engagement_id,
                        principalTable: "engagements",
                        principalColumn: "engagement_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "engagement_assignments",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    engagement_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_engagement_assignments", x => new { x.user_id, x.engagement_id });
                    table.ForeignKey(
                        name: "FK_engagement_assignments_engagements_engagement_id",
                        column: x => x.engagement_id,
                        principalTable: "engagements",
                        principalColumn: "engagement_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_engagement_assignments_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "checklist_categories",
                columns: new[] { "id", "category_name" },
                values: new object[,]
                {
                    { 1L, "Commercial" },
                    { 2L, "Handover" },
                    { 3L, "Scope" },
                    { 4L, "Governance" },
                    { 5L, "Resourcing" },
                    { 6L, "Technical Readiness" }
                });

            migrationBuilder.InsertData(
                table: "checklist_statuses",
                columns: new[] { "id", "status_name" },
                values: new object[,]
                {
                    { 1L, "Draft" },
                    { 2L, "Published" }
                });

            migrationBuilder.InsertData(
                table: "commercial_models",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 1, "Time and Material" },
                    { 2, "Fixed" },
                    { 3, "Capped T&M" }
                });

            migrationBuilder.InsertData(
                table: "commitment_types",
                columns: new[] { "commitment_type_id", "type" },
                values: new object[,]
                {
                    { 1, "Delivery Milestone" },
                    { 2, "Resource Allocation" },
                    { 3, "Custom Requirement" }
                });

            migrationBuilder.InsertData(
                table: "document_categories",
                columns: new[] { "id", "category_name" },
                values: new object[,]
                {
                    { 1, "RFP" },
                    { 2, "Proposal" },
                    { 3, "Signed SOW" },
                    { 4, "Email Thread" },
                    { 5, "Design or Approach" },
                    { 6, "Other" }
                });

            migrationBuilder.InsertData(
                table: "engagement_statuses",
                columns: new[] { "status_id", "status_name" },
                values: new object[,]
                {
                    { 1, "KickoffPending" },
                    { 2, "Active" },
                    { 3, "Inactive" }
                });

            migrationBuilder.InsertData(
                table: "engagement_types",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 1, "Greenfield" },
                    { 2, "Brownfield" }
                });

            migrationBuilder.InsertData(
                table: "handover_info",
                columns: new[] { "handover_id", "business_problem", "client_background", "client_sentiment", "created_at", "created_by", "deal_origin", "engagement_id", "status_id", "updated_at", "updated_by" },
                values: new object[] { 1L, "They need a modern core banking platform to support new digital initiatives and mobile apps.", "FinTech Solutions is a mid-sized financial institution expanding its digital footprint.", "Excited but cautious about delivery timelines due to past vendor failures.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Strategic Partner Referral", 1L, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null });

            migrationBuilder.InsertData(
                table: "handover_statuses",
                columns: new[] { "handover_status_id", "status_name" },
                values: new object[,]
                {
                    { 1, "Draft" },
                    { 2, "Submitted" }
                });

            migrationBuilder.InsertData(
                table: "roles",
                columns: new[] { "id", "created_at", "created_by", "name", "updated_at", "updated_by" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Pre Sales", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Project Management", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Tech Lead", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Leadership", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null }
                });

            migrationBuilder.InsertData(
                table: "commitments",
                columns: new[] { "commitment_id", "commitment_type_id", "created_at", "created_by", "description", "handover_id", "quantity", "updated_at", "updated_by" },
                values: new object[] { 1L, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Delivery of MVP banking module within 3 months of kickoff.", 1L, "1", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null });

            migrationBuilder.InsertData(
                table: "engagements",
                columns: new[] { "engagement_id", "billing_start_date", "client_name", "commercial_model_id", "created_at", "created_by", "engagement_description", "engagement_type_id", "HandoverInfoHandoverId", "kickoff_date", "status_id", "updated_at", "updated_by" },
                values: new object[,]
                {
                    { 1, null, "FinTech Solutions", 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Core Banking Greenfield Implementation", 1, null, new DateOnly(2026, 8, 15), 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 2, new DateOnly(2026, 7, 1), "HealthCare Plus", 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Legacy System Upgrade and Support", 2, null, new DateOnly(2026, 6, 1), 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null }
                });

            migrationBuilder.InsertData(
                table: "master_checklist_versions",
                columns: new[] { "id", "created_at", "created_by", "status_id", "updated_at", "updated_by", "version_number" },
                values: new object[] { 1L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1 });

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "id", "created_at", "created_by", "email", "full_name", "is_admin", "lockout_end", "password", "role_id", "updated_at", "updated_by" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "presales@company.com", "Rahul Verma", false, null, "Password@123", 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "pm@company.com", "Asha Rao", false, null, "Password@123", 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "techlead@company.com", "Vikram Singh", false, null, "Password@123", 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "leadership@company.com", "Neha Sharma", false, null, "Password@123", 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 5, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "admin@company.com", "Admin User", true, null, "Password@123", 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null }
                });

            migrationBuilder.InsertData(
                table: "engagement_assignments",
                columns: new[] { "engagement_id", "user_id" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 1, 2 },
                    { 2, 2 },
                    { 2, 3 }
                });

            migrationBuilder.InsertData(
                table: "master_checklist_items",
                columns: new[] { "id", "category_id", "created_at", "created_by", "description", "display_order", "is_mandatory", "title", "updated_at", "updated_by", "version_id" },
                values: new object[,]
                {
                    { 1L, 1L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Confirm the signed commercial contract is on file before kickoff.", 1, true, "Sign commercial contract", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1L },
                    { 2L, 1L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Verify the approved budget matches the statement of work.", 2, true, "Confirm engagement budget", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1L },
                    { 3L, 1L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Agree invoicing milestones and payment terms with the client.", 3, true, "Confirm billing schedule", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1L },
                    { 4L, 2L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Sales team walks delivery team through client history and expectations.", 4, true, "Complete sales-to-delivery handover call", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1L },
                    { 5L, 2L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Ensure delivery has access to all pre-sales notes and correspondence.", 5, true, "Share CRM opportunity notes", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1L },
                    { 6L, 2L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Confirm delivery team has contact details for all client stakeholders.", 6, true, "Transfer key client contacts", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1L },
                    { 7L, 3L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Capture in-scope and out-of-scope items in the kickoff scope document.", 7, true, "Document engagement scope", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1L },
                    { 8L, 3L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "List and confirm all deliverables committed to the client.", 8, true, "Confirm deliverables list", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1L }
                });

            migrationBuilder.InsertData(
                table: "master_checklist_items",
                columns: new[] { "id", "category_id", "created_at", "created_by", "description", "display_order", "title", "updated_at", "updated_by", "version_id" },
                values: new object[] { 9L, 3L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Record assumptions made during the sales cycle that affect scope.", 9, "Identify scope assumptions and exclusions", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1L });

            migrationBuilder.InsertData(
                table: "master_checklist_items",
                columns: new[] { "id", "category_id", "created_at", "created_by", "description", "display_order", "is_mandatory", "title", "updated_at", "updated_by", "version_id" },
                values: new object[,]
                {
                    { 10L, 4L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Agree recurring status meetings and escalation paths.", 10, true, "Set up engagement governance cadence", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1L },
                    { 11L, 4L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Identify decision-makers on both client and delivery sides.", 11, true, "Assign engagement steering committee", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1L },
                    { 12L, 4L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Agree how scope changes will be raised, approved, and tracked.", 12, true, "Define change control process", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1L }
                });

            migrationBuilder.InsertData(
                table: "master_checklist_items",
                columns: new[] { "id", "category_id", "created_at", "created_by", "description", "display_order", "title", "updated_at", "updated_by", "version_id" },
                values: new object[] { 13L, 4L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Assign an owner to maintain the Risks, Assumptions, Issues, and Dependencies log.", 13, "Confirm RAID log owner", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1L });

            migrationBuilder.InsertData(
                table: "master_checklist_items",
                columns: new[] { "id", "category_id", "created_at", "created_by", "description", "display_order", "is_mandatory", "title", "updated_at", "updated_by", "version_id" },
                values: new object[,]
                {
                    { 14L, 5L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Confirm named resources are assigned and available for the engagement.", 14, true, "Allocate delivery team members", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1L },
                    { 15L, 5L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Check assigned resources have capacity for the engagement duration.", 15, true, "Confirm resource availability calendar", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1L }
                });

            migrationBuilder.InsertData(
                table: "master_checklist_items",
                columns: new[] { "id", "category_id", "created_at", "created_by", "description", "display_order", "title", "updated_at", "updated_by", "version_id" },
                values: new object[] { 16L, 5L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Name backup resources in case of absence during the engagement.", 16, "Identify backup/on-call resources", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1L });

            migrationBuilder.InsertData(
                table: "master_checklist_items",
                columns: new[] { "id", "category_id", "created_at", "created_by", "description", "display_order", "is_mandatory", "title", "updated_at", "updated_by", "version_id" },
                values: new object[,]
                {
                    { 17L, 6L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Request and confirm access to required client systems and environments.", 17, true, "Provision environment access", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1L },
                    { 18L, 6L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Confirm client infrastructure and tooling meet engagement requirements.", 18, true, "Verify technical prerequisites", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1L },
                    { 19L, 6L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Ensure delivery team has access to repositories and pipelines.", 19, true, "Set up source control and CI/CD access", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1L }
                });

            migrationBuilder.InsertData(
                table: "master_checklist_items",
                columns: new[] { "id", "category_id", "created_at", "created_by", "description", "display_order", "title", "updated_at", "updated_by", "version_id" },
                values: new object[] { 20L, 6L, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Review any client-specific security or compliance obligations.", 20, "Confirm security and compliance requirements", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1L });

            migrationBuilder.CreateIndex(
                name: "IX_commitments_handover_id",
                table: "commitments",
                column: "handover_id");

            migrationBuilder.CreateIndex(
                name: "IX_documents_category_id",
                table: "documents",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "IX_documents_engagement_id",
                table: "documents",
                column: "engagement_id");

            migrationBuilder.CreateIndex(
                name: "IX_engagement_assignments_engagement_id",
                table: "engagement_assignments",
                column: "engagement_id");

            migrationBuilder.CreateIndex(
                name: "IX_engagements_commercial_model_id",
                table: "engagements",
                column: "commercial_model_id");

            migrationBuilder.CreateIndex(
                name: "IX_engagements_engagement_type_id",
                table: "engagements",
                column: "engagement_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_engagements_HandoverInfoHandoverId",
                table: "engagements",
                column: "HandoverInfoHandoverId");

            migrationBuilder.CreateIndex(
                name: "IX_engagements_status_id",
                table: "engagements",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "IX_master_checklist_items_category_id",
                table: "master_checklist_items",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "IX_master_checklist_items_version_id",
                table: "master_checklist_items",
                column: "version_id");

            migrationBuilder.CreateIndex(
                name: "IX_master_checklist_versions_status_id",
                table: "master_checklist_versions",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "IX_roles_name",
                table: "roles",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_users_email",
                table: "users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_users_role_id",
                table: "users",
                column: "role_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "commitment_types");

            migrationBuilder.DropTable(
                name: "commitments");

            migrationBuilder.DropTable(
                name: "documents");

            migrationBuilder.DropTable(
                name: "engagement_assignments");

            migrationBuilder.DropTable(
                name: "handover_statuses");

            migrationBuilder.DropTable(
                name: "master_checklist_items");

            migrationBuilder.DropTable(
                name: "document_categories");

            migrationBuilder.DropTable(
                name: "engagements");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "checklist_categories");

            migrationBuilder.DropTable(
                name: "master_checklist_versions");

            migrationBuilder.DropTable(
                name: "commercial_models");

            migrationBuilder.DropTable(
                name: "engagement_statuses");

            migrationBuilder.DropTable(
                name: "engagement_types");

            migrationBuilder.DropTable(
                name: "handover_info");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.DropTable(
                name: "checklist_statuses");
        }
    }
}
