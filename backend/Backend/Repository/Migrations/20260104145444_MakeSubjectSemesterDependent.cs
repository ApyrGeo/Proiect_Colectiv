using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TrackForUBB.Repository.Migrations
{
    /// <inheritdoc />
    public partial class MakeSubjectSemesterDependent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Grades_PromotionSemesters_SemesterId",
                table: "Grades");

            migrationBuilder.DropForeignKey(
                name: "FK_Hours_PromotionSemesters_SemesterId",
                table: "Hours");

            migrationBuilder.DropForeignKey(
                name: "FK_PromotionSemesters_PromotionYears_PromotionYearId",
                table: "PromotionSemesters");

            migrationBuilder.DropForeignKey(
                name: "FK_Subjects_Teachers_HolderTeacherId",
                table: "Subjects");

            migrationBuilder.DropTable(
                name: "PromotionYears");

            migrationBuilder.DropIndex(
                name: "IX_Hours_SemesterId",
                table: "Hours");

            migrationBuilder.DropIndex(
                name: "IX_Grades_SemesterId",
                table: "Grades");

            migrationBuilder.DropColumn(
                name: "SemesterId",
                table: "Hours");

            migrationBuilder.DropColumn(
                name: "SemesterId",
                table: "Grades");

            migrationBuilder.Sql("""DELETE FROM "PromotionSemesters";""");

            migrationBuilder.Sql("""DELETE FROM "Subjects";""");

            migrationBuilder.RenameColumn(
                name: "PromotionYearId",
                table: "PromotionSemesters",
                newName: "PromotionId");

            migrationBuilder.RenameIndex(
                name: "IX_PromotionSemesters_PromotionYearId",
                table: "PromotionSemesters",
                newName: "IX_PromotionSemesters_PromotionId");

            migrationBuilder.AlterColumn<int>(
                name: "HolderTeacherId",
                table: "Subjects",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OptionalPackage",
                table: "Subjects",
                type: "integer",
                nullable: true);


            migrationBuilder.AddColumn<int>(
                name: "SemesterId",
                table: "Subjects",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "SubjectCode",
                table: "Subjects",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SubjectCodeId",
                table: "Subjects",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Subjects",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "SubjectCode",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubjectCode", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Subjects_SemesterId",
                table: "Subjects",
                column: "SemesterId");

            migrationBuilder.CreateIndex(
                name: "IX_Subjects_SubjectCodeId",
                table: "Subjects",
                column: "SubjectCodeId");

            migrationBuilder.AddForeignKey(
                name: "FK_PromotionSemesters_Promotions_PromotionId",
                table: "PromotionSemesters",
                column: "PromotionId",
                principalTable: "Promotions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Subjects_PromotionSemesters_SemesterId",
                table: "Subjects",
                column: "SemesterId",
                principalTable: "PromotionSemesters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Subjects_SubjectCode_SubjectCodeId",
                table: "Subjects",
                column: "SubjectCodeId",
                principalTable: "SubjectCode",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Subjects_Teachers_HolderTeacherId",
                table: "Subjects",
                column: "HolderTeacherId",
                principalTable: "Teachers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PromotionSemesters_Promotions_PromotionId",
                table: "PromotionSemesters");

            migrationBuilder.DropForeignKey(
                name: "FK_Subjects_PromotionSemesters_SemesterId",
                table: "Subjects");

            migrationBuilder.DropForeignKey(
                name: "FK_Subjects_SubjectCode_SubjectCodeId",
                table: "Subjects");

            migrationBuilder.DropForeignKey(
                name: "FK_Subjects_Teachers_HolderTeacherId",
                table: "Subjects");

            migrationBuilder.DropTable(
                name: "SubjectCode");

            migrationBuilder.DropIndex(
                name: "IX_Subjects_SemesterId",
                table: "Subjects");

            migrationBuilder.DropIndex(
                name: "IX_Subjects_SubjectCodeId",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "OptionalPackage",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "SemesterId",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "SubjectCode",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "SubjectCodeId",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Subjects");

            migrationBuilder.RenameColumn(
                name: "PromotionId",
                table: "PromotionSemesters",
                newName: "PromotionYearId");

            migrationBuilder.RenameIndex(
                name: "IX_PromotionSemesters_PromotionId",
                table: "PromotionSemesters",
                newName: "IX_PromotionSemesters_PromotionYearId");

            migrationBuilder.AlterColumn<int>(
                name: "HolderTeacherId",
                table: "Subjects",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "SemesterId",
                table: "Hours",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SemesterId",
                table: "Grades",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "PromotionYears",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PromotionId = table.Column<int>(type: "integer", nullable: false),
                    YearNumber = table.Column<int>(type: "integer", nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_Hours_SemesterId",
                table: "Hours",
                column: "SemesterId");

            migrationBuilder.CreateIndex(
                name: "IX_Grades_SemesterId",
                table: "Grades",
                column: "SemesterId");

            migrationBuilder.CreateIndex(
                name: "IX_PromotionYears_PromotionId",
                table: "PromotionYears",
                column: "PromotionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Grades_PromotionSemesters_SemesterId",
                table: "Grades",
                column: "SemesterId",
                principalTable: "PromotionSemesters",
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
                name: "FK_PromotionSemesters_PromotionYears_PromotionYearId",
                table: "PromotionSemesters",
                column: "PromotionYearId",
                principalTable: "PromotionYears",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Subjects_Teachers_HolderTeacherId",
                table: "Subjects",
                column: "HolderTeacherId",
                principalTable: "Teachers",
                principalColumn: "Id");
        }
    }
}
