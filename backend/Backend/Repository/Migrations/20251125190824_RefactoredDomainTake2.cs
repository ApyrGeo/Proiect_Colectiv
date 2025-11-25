using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TrackForUBB.Repository.Migrations
{
    /// <inheritdoc />
    public partial class RefactoredDomainTake2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Enrollments_PromotionSemesters_PromotionSemesterId",
                table: "Enrollments");

            migrationBuilder.DropForeignKey(
                name: "FK_Subjects_PromotionSemesters_PromotionSemesterId",
                table: "Subjects");

            migrationBuilder.DropIndex(
                name: "IX_Subjects_PromotionSemesterId",
                table: "Subjects");

            migrationBuilder.DropIndex(
                name: "IX_Enrollments_PromotionSemesterId",
                table: "Enrollments");

            migrationBuilder.DropColumn(
                name: "PromotionSemesterId",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "PromotionSemesterId",
                table: "Enrollments");

            migrationBuilder.CreateTable(
                name: "Contracts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SemesterId = table.Column<int>(type: "integer", nullable: false),
                    EnrollmentId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contracts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Contracts_Enrollments_EnrollmentId",
                        column: x => x.EnrollmentId,
                        principalTable: "Enrollments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Contracts_PromotionSemesters_SemesterId",
                        column: x => x.SemesterId,
                        principalTable: "PromotionSemesters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContractSubject",
                columns: table => new
                {
                    ContractsId = table.Column<int>(type: "integer", nullable: false),
                    SubjectsId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractSubject", x => new { x.ContractsId, x.SubjectsId });
                    table.ForeignKey(
                        name: "FK_ContractSubject_Contracts_ContractsId",
                        column: x => x.ContractsId,
                        principalTable: "Contracts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContractSubject_Subjects_SubjectsId",
                        column: x => x.SubjectsId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_EnrollmentId",
                table: "Contracts",
                column: "EnrollmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_SemesterId",
                table: "Contracts",
                column: "SemesterId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractSubject_SubjectsId",
                table: "ContractSubject",
                column: "SubjectsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContractSubject");

            migrationBuilder.DropTable(
                name: "Contracts");

            migrationBuilder.AddColumn<int>(
                name: "PromotionSemesterId",
                table: "Subjects",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PromotionSemesterId",
                table: "Enrollments",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Subjects_PromotionSemesterId",
                table: "Subjects",
                column: "PromotionSemesterId");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_PromotionSemesterId",
                table: "Enrollments",
                column: "PromotionSemesterId");

            migrationBuilder.AddForeignKey(
                name: "FK_Enrollments_PromotionSemesters_PromotionSemesterId",
                table: "Enrollments",
                column: "PromotionSemesterId",
                principalTable: "PromotionSemesters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Subjects_PromotionSemesters_PromotionSemesterId",
                table: "Subjects",
                column: "PromotionSemesterId",
                principalTable: "PromotionSemesters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
