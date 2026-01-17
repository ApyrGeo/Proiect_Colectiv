using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrackForUBB.Repository.Migrations
{
    /// <inheritdoc />
    public partial class FullSubjectAgain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Subjects_Name",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "ForScholarship",
                table: "Subjects");

            migrationBuilder.RenameColumn(
                name: "NrCredits",
                table: "Subjects",
                newName: "NumberOfCredits");

            migrationBuilder.AddColumn<int>(
                name: "GroupYearId",
                table: "Subjects",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Subjects_GroupYearId",
                table: "Subjects",
                column: "GroupYearId");

            migrationBuilder.AddForeignKey(
                name: "FK_Subjects_GroupYears_GroupYearId",
                table: "Subjects",
                column: "GroupYearId",
                principalTable: "GroupYears",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subjects_GroupYears_GroupYearId",
                table: "Subjects");

            migrationBuilder.DropIndex(
                name: "IX_Subjects_GroupYearId",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "GroupYearId",
                table: "Subjects");

            migrationBuilder.RenameColumn(
                name: "NumberOfCredits",
                table: "Subjects",
                newName: "NrCredits");

            migrationBuilder.AddColumn<bool>(
                name: "ForScholarship",
                table: "Subjects",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Subjects_Name",
                table: "Subjects",
                column: "Name",
                unique: true);
        }
    }
}
