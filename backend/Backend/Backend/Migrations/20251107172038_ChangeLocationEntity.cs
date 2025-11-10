using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class ChangeLocationEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GoogleMapsData_Id",
                table: "Locations",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "GoogleMapsData_Latitude",
                table: "Locations",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "GoogleMapsData_Longitude",
                table: "Locations",
                type: "double precision",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GoogleMapsData_Id",
                table: "Locations");

            migrationBuilder.DropColumn(
                name: "GoogleMapsData_Latitude",
                table: "Locations");

            migrationBuilder.DropColumn(
                name: "GoogleMapsData_Longitude",
                table: "Locations");
        }
    }
}
