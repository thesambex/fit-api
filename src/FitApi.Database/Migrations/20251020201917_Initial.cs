using System;
using FitApi.Core.Domain.Patients.Enums;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FitApi.Database.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "assessments");

            migrationBuilder.EnsureSchema(
                name: "patients");

            migrationBuilder.EnsureSchema(
                name: "professionals");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:patients.birth_genres", "male,female")
                .Annotation("Npgsql:PostgresExtension:uuid-ossp", ",,");

            migrationBuilder.CreateTable(
                name: "patients",
                schema: "patients",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    name = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    birth_date = table.Column<DateOnly>(type: "date", nullable: false),
                    birth_genre = table.Column<BirthGenres>(type: "patients.birth_genres", nullable: false),
                    external_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_patients", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "professionals",
                schema: "professionals",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    name = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    external_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_professionals", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "body_assessments",
                schema: "assessments",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    age = table.Column<int>(type: "integer", nullable: false),
                    birth_genre = table.Column<BirthGenres>(type: "patients.birth_genres", nullable: false),
                    height = table.Column<decimal>(type: "numeric(3,2)", nullable: false),
                    weight = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    folds_sum = table.Column<decimal>(type: "numeric(10,9)", nullable: false),
                    external_id = table.Column<Guid>(type: "uuid", nullable: false),
                    patient_id = table.Column<long>(type: "bigint", nullable: false),
                    professional_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_body_assessments", x => x.id);
                    table.ForeignKey(
                        name: "FK_body_assessments_patients",
                        column: x => x.patient_id,
                        principalSchema: "patients",
                        principalTable: "patients",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_body_assessments_professionals",
                        column: x => x.professional_id,
                        principalSchema: "professionals",
                        principalTable: "professionals",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "body_assessment_skin_folds",
                schema: "assessments",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    triceps = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    biceps = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    subscapular = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    suprailiac = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    median_axillary = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    thoracic = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    supraspinal = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    thigh = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    calf = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    assessment_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_body_assessment_skin_folds", x => x.id);
                    table.ForeignKey(
                        name: "FK_body_assessment_skin_folds",
                        column: x => x.assessment_id,
                        principalSchema: "assessments",
                        principalTable: "body_assessments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_body_assessment_skin_folds_assessment_id",
                schema: "assessments",
                table: "body_assessment_skin_folds",
                column: "assessment_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_body_assessments_patient_id",
                schema: "assessments",
                table: "body_assessments",
                column: "patient_id");

            migrationBuilder.CreateIndex(
                name: "IX_body_assessments_professional_id",
                schema: "assessments",
                table: "body_assessments",
                column: "professional_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "body_assessment_skin_folds",
                schema: "assessments");

            migrationBuilder.DropTable(
                name: "body_assessments",
                schema: "assessments");

            migrationBuilder.DropTable(
                name: "patients",
                schema: "patients");

            migrationBuilder.DropTable(
                name: "professionals",
                schema: "professionals");
        }
    }
}
