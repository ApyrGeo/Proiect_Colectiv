using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TrackForUBB.Repository.Migrations
{
    /// <inheritdoc />
    public partial class ExamEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HeldSubjectId",
                table: "Teachers",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ExamEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ExamDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Duration = table.Column<int>(type: "integer", nullable: false),
                    ClassroomId = table.Column<int>(type: "integer", nullable: false),
                    SubjectId = table.Column<int>(type: "integer", nullable: false),
                    StudentGroupId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExamEntries_Classrooms_ClassroomId",
                        column: x => x.ClassroomId,
                        principalTable: "Classrooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExamEntries_Groups_StudentGroupId",
                        column: x => x.StudentGroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExamEntries_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Teachers_HeldSubjectId",
                table: "Teachers",
                column: "HeldSubjectId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExamEntries_ClassroomId",
                table: "ExamEntries",
                column: "ClassroomId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamEntries_StudentGroupId",
                table: "ExamEntries",
                column: "StudentGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamEntries_SubjectId",
                table: "ExamEntries",
                column: "SubjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Teachers_Subjects_HeldSubjectId",
                table: "Teachers",
                column: "HeldSubjectId",
                principalTable: "Subjects",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Teachers_Subjects_HeldSubjectId",
                table: "Teachers");

            migrationBuilder.DropTable(
                name: "ExamEntries");

            migrationBuilder.DropIndex(
                name: "IX_Teachers_HeldSubjectId",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "HeldSubjectId",
                table: "Teachers");
        }
    }
}
