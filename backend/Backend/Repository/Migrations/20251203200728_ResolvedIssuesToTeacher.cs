using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrackForUBB.Repository.Migrations
{
    /// <inheritdoc />
    public partial class ResolvedIssuesToTeacher : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Teachers_Subjects_HeldSubjectId",
                table: "Teachers");

            migrationBuilder.DropIndex(
                name: "IX_Teachers_HeldSubjectId",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "HeldSubjectId",
                table: "Teachers");

            migrationBuilder.AddColumn<int>(
                name: "HolderTeacherId",
                table: "Subjects",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Subjects_HolderTeacherId",
                table: "Subjects",
                column: "HolderTeacherId");

            migrationBuilder.AddForeignKey(
                name: "FK_Subjects_Teachers_HolderTeacherId",
                table: "Subjects",
                column: "HolderTeacherId",
                principalTable: "Teachers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subjects_Teachers_HolderTeacherId",
                table: "Subjects");

            migrationBuilder.DropIndex(
                name: "IX_Subjects_HolderTeacherId",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "HolderTeacherId",
                table: "Subjects");

            migrationBuilder.AddColumn<int>(
                name: "HeldSubjectId",
                table: "Teachers",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Teachers_HeldSubjectId",
                table: "Teachers",
                column: "HeldSubjectId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Teachers_Subjects_HeldSubjectId",
                table: "Teachers",
                column: "HeldSubjectId",
                principalTable: "Subjects",
                principalColumn: "Id");
        }
    }
}
