using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TrackForUBB.Repository.Migrations
{
    /// <inheritdoc />
    public partial class RefactoredDomain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Groups_GroupYears_GroupYearId",
                table: "Groups");

            migrationBuilder.DropForeignKey(
                name: "FK_Hours_GroupYears_GroupYearId",
                table: "Hours");

            migrationBuilder.DropForeignKey(
                name: "FK_Subjects_GroupYears_GroupYearId",
                table: "Subjects");

            migrationBuilder.DropTable(
                name: "GroupYears");

            migrationBuilder.RenameColumn(
                name: "GroupYearId",
                table: "Subjects",
                newName: "PromotionSemesterId");

            migrationBuilder.RenameIndex(
                name: "IX_Subjects_GroupYearId",
                table: "Subjects",
                newName: "IX_Subjects_PromotionSemesterId");

            migrationBuilder.RenameColumn(
                name: "GroupYearId",
                table: "Hours",
                newName: "PromotionId");

            migrationBuilder.RenameIndex(
                name: "IX_Hours_GroupYearId",
                table: "Hours",
                newName: "IX_Hours_PromotionId");

            migrationBuilder.RenameColumn(
                name: "GroupYearId",
                table: "Groups",
                newName: "PromotionId");

            migrationBuilder.RenameIndex(
                name: "IX_Groups_GroupYearId",
                table: "Groups",
                newName: "IX_Groups_PromotionId");

            migrationBuilder.AddColumn<int>(
                name: "SemesterId",
                table: "Hours",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PromotionSemesterId",
                table: "Enrollments",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Promotions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StartYear = table.Column<int>(type: "integer", nullable: false),
                    EndYear = table.Column<int>(type: "integer", nullable: false),
                    SpecialisationId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Promotions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Promotions_Specialisations_SpecialisationId",
                        column: x => x.SpecialisationId,
                        principalTable: "Specialisations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PromotionYears",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    YearNumber = table.Column<int>(type: "integer", nullable: false),
                    PromotionId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromotionYears", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PromotionYears_Promotions_PromotionId",
                        column: x => x.PromotionId,
                        principalTable: "Promotions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PromotionSemesters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SemesterNumber = table.Column<int>(type: "integer", nullable: false),
                    PromotionYearId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromotionSemesters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PromotionSemesters_PromotionYears_PromotionYearId",
                        column: x => x.PromotionYearId,
                        principalTable: "PromotionYears",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Grades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Value = table.Column<int>(type: "integer", nullable: false),
                    SubjectId = table.Column<int>(type: "integer", nullable: false),
                    SemesterId = table.Column<int>(type: "integer", nullable: false),
                    EnrollmentId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Grades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Grades_Enrollments_EnrollmentId",
                        column: x => x.EnrollmentId,
                        principalTable: "Enrollments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Grades_PromotionSemesters_SemesterId",
                        column: x => x.SemesterId,
                        principalTable: "PromotionSemesters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Grades_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Hours_SemesterId",
                table: "Hours",
                column: "SemesterId");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_PromotionSemesterId",
                table: "Enrollments",
                column: "PromotionSemesterId");

            migrationBuilder.CreateIndex(
                name: "IX_Grades_EnrollmentId",
                table: "Grades",
                column: "EnrollmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Grades_SemesterId",
                table: "Grades",
                column: "SemesterId");

            migrationBuilder.CreateIndex(
                name: "IX_Grades_SubjectId",
                table: "Grades",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Promotions_SpecialisationId",
                table: "Promotions",
                column: "SpecialisationId");

            migrationBuilder.CreateIndex(
                name: "IX_PromotionSemesters_PromotionYearId",
                table: "PromotionSemesters",
                column: "PromotionYearId");

            migrationBuilder.CreateIndex(
                name: "IX_PromotionYears_PromotionId",
                table: "PromotionYears",
                column: "PromotionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Enrollments_PromotionSemesters_PromotionSemesterId",
                table: "Enrollments",
                column: "PromotionSemesterId",
                principalTable: "PromotionSemesters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_Promotions_PromotionId",
                table: "Groups",
                column: "PromotionId",
                principalTable: "Promotions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Hours_PromotionSemesters_SemesterId",
                table: "Hours",
                column: "SemesterId",
                principalTable: "PromotionSemesters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Hours_Promotions_PromotionId",
                table: "Hours",
                column: "PromotionId",
                principalTable: "Promotions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Subjects_PromotionSemesters_PromotionSemesterId",
                table: "Subjects",
                column: "PromotionSemesterId",
                principalTable: "PromotionSemesters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Enrollments_PromotionSemesters_PromotionSemesterId",
                table: "Enrollments");

            migrationBuilder.DropForeignKey(
                name: "FK_Groups_Promotions_PromotionId",
                table: "Groups");

            migrationBuilder.DropForeignKey(
                name: "FK_Hours_PromotionSemesters_SemesterId",
                table: "Hours");

            migrationBuilder.DropForeignKey(
                name: "FK_Hours_Promotions_PromotionId",
                table: "Hours");

            migrationBuilder.DropForeignKey(
                name: "FK_Subjects_PromotionSemesters_PromotionSemesterId",
                table: "Subjects");

            migrationBuilder.DropTable(
                name: "Grades");

            migrationBuilder.DropTable(
                name: "PromotionSemesters");

            migrationBuilder.DropTable(
                name: "PromotionYears");

            migrationBuilder.DropTable(
                name: "Promotions");

            migrationBuilder.DropIndex(
                name: "IX_Hours_SemesterId",
                table: "Hours");

            migrationBuilder.DropIndex(
                name: "IX_Enrollments_PromotionSemesterId",
                table: "Enrollments");

            migrationBuilder.DropColumn(
                name: "SemesterId",
                table: "Hours");

            migrationBuilder.DropColumn(
                name: "PromotionSemesterId",
                table: "Enrollments");

            migrationBuilder.RenameColumn(
                name: "PromotionSemesterId",
                table: "Subjects",
                newName: "GroupYearId");

            migrationBuilder.RenameIndex(
                name: "IX_Subjects_PromotionSemesterId",
                table: "Subjects",
                newName: "IX_Subjects_GroupYearId");

            migrationBuilder.RenameColumn(
                name: "PromotionId",
                table: "Hours",
                newName: "GroupYearId");

            migrationBuilder.RenameIndex(
                name: "IX_Hours_PromotionId",
                table: "Hours",
                newName: "IX_Hours_GroupYearId");

            migrationBuilder.RenameColumn(
                name: "PromotionId",
                table: "Groups",
                newName: "GroupYearId");

            migrationBuilder.RenameIndex(
                name: "IX_Groups_PromotionId",
                table: "Groups",
                newName: "IX_Groups_GroupYearId");

            migrationBuilder.CreateTable(
                name: "GroupYears",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SpecialisationId = table.Column<int>(type: "integer", nullable: false),
                    Year = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupYears", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroupYears_Specialisations_SpecialisationId",
                        column: x => x.SpecialisationId,
                        principalTable: "Specialisations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GroupYears_SpecialisationId",
                table: "GroupYears",
                column: "SpecialisationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_GroupYears_GroupYearId",
                table: "Groups",
                column: "GroupYearId",
                principalTable: "GroupYears",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Hours_GroupYears_GroupYearId",
                table: "Hours",
                column: "GroupYearId",
                principalTable: "GroupYears",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Subjects_GroupYears_GroupYearId",
                table: "Subjects",
                column: "GroupYearId",
                principalTable: "GroupYears",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
