using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repository.Migrations
{
    /// <inheritdoc />
    public partial class ConnectedSubjectWithHour : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Subject",
                table: "Hours");

            migrationBuilder.AddColumn<int>(
                name: "SubjectId",
                table: "Hours",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Hours_SubjectId",
                table: "Hours",
                column: "SubjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Hours_Subjects_SubjectId",
                table: "Hours",
                column: "SubjectId",
                principalTable: "Subjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Hours_Subjects_SubjectId",
                table: "Hours");

            migrationBuilder.DropIndex(
                name: "IX_Hours_SubjectId",
                table: "Hours");

            migrationBuilder.DropColumn(
                name: "SubjectId",
                table: "Hours");

            migrationBuilder.AddColumn<string>(
                name: "Subject",
                table: "Hours",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }
    }
}
