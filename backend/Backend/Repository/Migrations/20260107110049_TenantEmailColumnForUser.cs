using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrackForUBB.Repository.Migrations
{
    /// <inheritdoc />
    public partial class TenantEmailColumnForUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TenantEmail",
                table: "Users",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TenantEmail",
                table: "Users");
        }
    }
}
