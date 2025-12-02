using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrackForUBB.Repository.Migrations
{
    /// <inheritdoc />
    public partial class AddUserSignature : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "Signature",
                table: "Users",
                type: "bytea",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Signature",
                table: "Users");
        }
    }
}
