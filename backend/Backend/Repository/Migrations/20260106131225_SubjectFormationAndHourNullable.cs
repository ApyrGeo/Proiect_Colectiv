using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrackForUBB.Repository.Migrations
{
    /// <inheritdoc />
    public partial class SubjectFormationAndHourNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Hours_Classrooms_ClassroomId",
                table: "Hours");

            migrationBuilder.DropForeignKey(
                name: "FK_Hours_Teachers_TeacherId",
                table: "Hours");

            migrationBuilder.DropForeignKey(
                name: "FK_Subjects_SubjectCode_SubjectCodeId",
                table: "Subjects");

            migrationBuilder.DropTable(
                name: "SubjectCode");

            migrationBuilder.DropIndex(
                name: "IX_Subjects_SubjectCodeId",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "SubjectCodeId",
                table: "Subjects");

            migrationBuilder.AddColumn<int>(
                name: "FormationType",
                table: "Subjects",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "TeacherId",
                table: "Hours",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "ClassroomId",
                table: "Hours",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_Hours_Classrooms_ClassroomId",
                table: "Hours",
                column: "ClassroomId",
                principalTable: "Classrooms",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Hours_Teachers_TeacherId",
                table: "Hours",
                column: "TeacherId",
                principalTable: "Teachers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Hours_Classrooms_ClassroomId",
                table: "Hours");

            migrationBuilder.DropForeignKey(
                name: "FK_Hours_Teachers_TeacherId",
                table: "Hours");

            migrationBuilder.DropColumn(
                name: "FormationType",
                table: "Subjects");

            migrationBuilder.AddColumn<string>(
                name: "SubjectCodeId",
                table: "Subjects",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TeacherId",
                table: "Hours",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ClassroomId",
                table: "Hours",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

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
                name: "IX_Subjects_SubjectCodeId",
                table: "Subjects",
                column: "SubjectCodeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Hours_Classrooms_ClassroomId",
                table: "Hours",
                column: "ClassroomId",
                principalTable: "Classrooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Hours_Teachers_TeacherId",
                table: "Hours",
                column: "TeacherId",
                principalTable: "Teachers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Subjects_SubjectCode_SubjectCodeId",
                table: "Subjects",
                column: "SubjectCodeId",
                principalTable: "SubjectCode",
                principalColumn: "Id");
        }
    }
}
