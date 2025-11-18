using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repository.Migrations
{
    /// <inheritdoc />
    public partial class ChangedConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SubGroups_Name",
                table: "SubGroups");

            migrationBuilder.DropIndex(
                name: "IX_Specialisations_Name",
                table: "Specialisations");

            migrationBuilder.DropIndex(
                name: "IX_GroupYears_Year",
                table: "GroupYears");

            migrationBuilder.DropIndex(
                name: "IX_Groups_Name",
                table: "Groups");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_SubGroups_Name",
                table: "SubGroups",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Specialisations_Name",
                table: "Specialisations",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GroupYears_Year",
                table: "GroupYears",
                column: "Year",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Groups_Name",
                table: "Groups",
                column: "Name",
                unique: true);
        }
    }
}
