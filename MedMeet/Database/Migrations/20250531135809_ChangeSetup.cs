using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class ChangeSetup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Records_Users_DoctorId",
                table: "Records");

            migrationBuilder.DropForeignKey(
                name: "FK_Records_Users_PatientId",
                table: "Records");

            migrationBuilder.AddForeignKey(
                name: "FK_Records_Users_DoctorId",
                table: "Records",
                column: "DoctorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Records_Users_PatientId",
                table: "Records",
                column: "PatientId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Records_Users_DoctorId",
                table: "Records");

            migrationBuilder.DropForeignKey(
                name: "FK_Records_Users_PatientId",
                table: "Records");

            migrationBuilder.AddForeignKey(
                name: "FK_Records_Users_DoctorId",
                table: "Records",
                column: "DoctorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Records_Users_PatientId",
                table: "Records",
                column: "PatientId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
