using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddedUserConfigs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Enrollment_StudentSubGroup_SubGroupId",
                table: "Enrollment");

            migrationBuilder.DropForeignKey(
                name: "FK_Enrollment_Users_StudentId",
                table: "Enrollment");

            migrationBuilder.DropForeignKey(
                name: "FK_GroupYear_Specialisation_SpecialisationId",
                table: "GroupYear");

            migrationBuilder.DropForeignKey(
                name: "FK_Specialisation_Faculty_FacultyId",
                table: "Specialisation");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentGroup_GroupYear_GroupYearId",
                table: "StudentGroup");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentSubGroup_StudentGroup_GroupId",
                table: "StudentSubGroup");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StudentSubGroup",
                table: "StudentSubGroup");

            migrationBuilder.DropIndex(
                name: "IX_StudentSubGroup_Id",
                table: "StudentSubGroup");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StudentGroup",
                table: "StudentGroup");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Specialisation",
                table: "Specialisation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GroupYear",
                table: "GroupYear");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Faculty",
                table: "Faculty");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Enrollment",
                table: "Enrollment");

            migrationBuilder.RenameTable(
                name: "StudentSubGroup",
                newName: "SubGroups");

            migrationBuilder.RenameTable(
                name: "StudentGroup",
                newName: "Groups");

            migrationBuilder.RenameTable(
                name: "Specialisation",
                newName: "Specialisations");

            migrationBuilder.RenameTable(
                name: "GroupYear",
                newName: "GroupYears");

            migrationBuilder.RenameTable(
                name: "Faculty",
                newName: "Faculties");

            migrationBuilder.RenameTable(
                name: "Enrollment",
                newName: "Enrollments");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Users",
                newName: "LastName");

            migrationBuilder.RenameColumn(
                name: "GroupId",
                table: "SubGroups",
                newName: "StudentGroupId");

            migrationBuilder.RenameIndex(
                name: "IX_StudentSubGroup_GroupId",
                table: "SubGroups",
                newName: "IX_SubGroups_StudentGroupId");

            migrationBuilder.RenameIndex(
                name: "IX_StudentGroup_Name",
                table: "Groups",
                newName: "IX_Groups_Name");

            migrationBuilder.RenameIndex(
                name: "IX_StudentGroup_GroupYearId",
                table: "Groups",
                newName: "IX_Groups_GroupYearId");

            migrationBuilder.RenameIndex(
                name: "IX_Specialisation_Name",
                table: "Specialisations",
                newName: "IX_Specialisations_Name");

            migrationBuilder.RenameIndex(
                name: "IX_Specialisation_FacultyId",
                table: "Specialisations",
                newName: "IX_Specialisations_FacultyId");

            migrationBuilder.RenameIndex(
                name: "IX_GroupYear_Year",
                table: "GroupYears",
                newName: "IX_GroupYears_Year");

            migrationBuilder.RenameIndex(
                name: "IX_GroupYear_SpecialisationId",
                table: "GroupYears",
                newName: "IX_GroupYears_SpecialisationId");

            migrationBuilder.RenameIndex(
                name: "IX_Faculty_Name",
                table: "Faculties",
                newName: "IX_Faculties_Name");

            migrationBuilder.RenameIndex(
                name: "IX_Enrollment_SubGroupId",
                table: "Enrollments",
                newName: "IX_Enrollments_SubGroupId");

            migrationBuilder.RenameIndex(
                name: "IX_Enrollment_StudentId",
                table: "Enrollments",
                newName: "IX_Enrollments_StudentId");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Users",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SubGroups",
                table: "SubGroups",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Groups",
                table: "Groups",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Specialisations",
                table: "Specialisations",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GroupYears",
                table: "GroupYears",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Faculties",
                table: "Faculties",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Enrollments",
                table: "Enrollments",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_SubGroups_Name",
                table: "SubGroups",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Enrollments_SubGroups_SubGroupId",
                table: "Enrollments",
                column: "SubGroupId",
                principalTable: "SubGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Enrollments_Users_StudentId",
                table: "Enrollments",
                column: "StudentId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_GroupYears_GroupYearId",
                table: "Groups",
                column: "GroupYearId",
                principalTable: "GroupYears",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GroupYears_Specialisations_SpecialisationId",
                table: "GroupYears",
                column: "SpecialisationId",
                principalTable: "Specialisations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Specialisations_Faculties_FacultyId",
                table: "Specialisations",
                column: "FacultyId",
                principalTable: "Faculties",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SubGroups_Groups_StudentGroupId",
                table: "SubGroups",
                column: "StudentGroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Enrollments_SubGroups_SubGroupId",
                table: "Enrollments");

            migrationBuilder.DropForeignKey(
                name: "FK_Enrollments_Users_StudentId",
                table: "Enrollments");

            migrationBuilder.DropForeignKey(
                name: "FK_Groups_GroupYears_GroupYearId",
                table: "Groups");

            migrationBuilder.DropForeignKey(
                name: "FK_GroupYears_Specialisations_SpecialisationId",
                table: "GroupYears");

            migrationBuilder.DropForeignKey(
                name: "FK_Specialisations_Faculties_FacultyId",
                table: "Specialisations");

            migrationBuilder.DropForeignKey(
                name: "FK_SubGroups_Groups_StudentGroupId",
                table: "SubGroups");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SubGroups",
                table: "SubGroups");

            migrationBuilder.DropIndex(
                name: "IX_SubGroups_Name",
                table: "SubGroups");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Specialisations",
                table: "Specialisations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GroupYears",
                table: "GroupYears");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Groups",
                table: "Groups");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Faculties",
                table: "Faculties");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Enrollments",
                table: "Enrollments");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Users");

            migrationBuilder.RenameTable(
                name: "SubGroups",
                newName: "StudentSubGroup");

            migrationBuilder.RenameTable(
                name: "Specialisations",
                newName: "Specialisation");

            migrationBuilder.RenameTable(
                name: "GroupYears",
                newName: "GroupYear");

            migrationBuilder.RenameTable(
                name: "Groups",
                newName: "StudentGroup");

            migrationBuilder.RenameTable(
                name: "Faculties",
                newName: "Faculty");

            migrationBuilder.RenameTable(
                name: "Enrollments",
                newName: "Enrollment");

            migrationBuilder.RenameColumn(
                name: "LastName",
                table: "Users",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "StudentGroupId",
                table: "StudentSubGroup",
                newName: "GroupId");

            migrationBuilder.RenameIndex(
                name: "IX_SubGroups_StudentGroupId",
                table: "StudentSubGroup",
                newName: "IX_StudentSubGroup_GroupId");

            migrationBuilder.RenameIndex(
                name: "IX_Specialisations_Name",
                table: "Specialisation",
                newName: "IX_Specialisation_Name");

            migrationBuilder.RenameIndex(
                name: "IX_Specialisations_FacultyId",
                table: "Specialisation",
                newName: "IX_Specialisation_FacultyId");

            migrationBuilder.RenameIndex(
                name: "IX_GroupYears_Year",
                table: "GroupYear",
                newName: "IX_GroupYear_Year");

            migrationBuilder.RenameIndex(
                name: "IX_GroupYears_SpecialisationId",
                table: "GroupYear",
                newName: "IX_GroupYear_SpecialisationId");

            migrationBuilder.RenameIndex(
                name: "IX_Groups_Name",
                table: "StudentGroup",
                newName: "IX_StudentGroup_Name");

            migrationBuilder.RenameIndex(
                name: "IX_Groups_GroupYearId",
                table: "StudentGroup",
                newName: "IX_StudentGroup_GroupYearId");

            migrationBuilder.RenameIndex(
                name: "IX_Faculties_Name",
                table: "Faculty",
                newName: "IX_Faculty_Name");

            migrationBuilder.RenameIndex(
                name: "IX_Enrollments_SubGroupId",
                table: "Enrollment",
                newName: "IX_Enrollment_SubGroupId");

            migrationBuilder.RenameIndex(
                name: "IX_Enrollments_StudentId",
                table: "Enrollment",
                newName: "IX_Enrollment_StudentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StudentSubGroup",
                table: "StudentSubGroup",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Specialisation",
                table: "Specialisation",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GroupYear",
                table: "GroupYear",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StudentGroup",
                table: "StudentGroup",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Faculty",
                table: "Faculty",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Enrollment",
                table: "Enrollment",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_StudentSubGroup_Id",
                table: "StudentSubGroup",
                column: "Id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Enrollment_StudentSubGroup_SubGroupId",
                table: "Enrollment",
                column: "SubGroupId",
                principalTable: "StudentSubGroup",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Enrollment_Users_StudentId",
                table: "Enrollment",
                column: "StudentId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GroupYear_Specialisation_SpecialisationId",
                table: "GroupYear",
                column: "SpecialisationId",
                principalTable: "Specialisation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Specialisation_Faculty_FacultyId",
                table: "Specialisation",
                column: "FacultyId",
                principalTable: "Faculty",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentGroup_GroupYear_GroupYearId",
                table: "StudentGroup",
                column: "GroupYearId",
                principalTable: "GroupYear",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentSubGroup_StudentGroup_GroupId",
                table: "StudentSubGroup",
                column: "GroupId",
                principalTable: "StudentGroup",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
