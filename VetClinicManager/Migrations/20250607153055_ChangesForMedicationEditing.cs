using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VetClinicManager.Migrations
{
    /// <inheritdoc />
    public partial class ChangesForMedicationEditing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnimalMedications_VisitUpdates_VisitUpdateId",
                table: "AnimalMedications");

            migrationBuilder.DropForeignKey(
                name: "FK_VisitUpdates_AspNetUsers_UpdatedByVetId",
                table: "VisitUpdates");

            migrationBuilder.DropForeignKey(
                name: "FK_VisitUpdates_Visits_VisitId",
                table: "VisitUpdates");

            migrationBuilder.AddForeignKey(
                name: "FK_AnimalMedications_VisitUpdates_VisitUpdateId",
                table: "AnimalMedications",
                column: "VisitUpdateId",
                principalTable: "VisitUpdates",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_VisitUpdates_AspNetUsers_UpdatedByVetId",
                table: "VisitUpdates",
                column: "UpdatedByVetId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VisitUpdates_Visits_VisitId",
                table: "VisitUpdates",
                column: "VisitId",
                principalTable: "Visits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnimalMedications_VisitUpdates_VisitUpdateId",
                table: "AnimalMedications");

            migrationBuilder.DropForeignKey(
                name: "FK_VisitUpdates_AspNetUsers_UpdatedByVetId",
                table: "VisitUpdates");

            migrationBuilder.DropForeignKey(
                name: "FK_VisitUpdates_Visits_VisitId",
                table: "VisitUpdates");

            migrationBuilder.AddForeignKey(
                name: "FK_AnimalMedications_VisitUpdates_VisitUpdateId",
                table: "AnimalMedications",
                column: "VisitUpdateId",
                principalTable: "VisitUpdates",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_VisitUpdates_AspNetUsers_UpdatedByVetId",
                table: "VisitUpdates",
                column: "UpdatedByVetId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VisitUpdates_Visits_VisitId",
                table: "VisitUpdates",
                column: "VisitId",
                principalTable: "Visits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
