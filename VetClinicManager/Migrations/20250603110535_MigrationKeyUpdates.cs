using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VetClinicManager.Migrations
{
    /// <inheritdoc />
    public partial class MigrationKeyUpdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VisitUpdates_AspNetUsers_UpdatedById",
                table: "VisitUpdates");

            migrationBuilder.DropIndex(
                name: "IX_VisitUpdates_UpdatedById",
                table: "VisitUpdates");

            migrationBuilder.DropColumn(
                name: "UpdatedById",
                table: "VisitUpdates");

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedByVetId",
                table: "VisitUpdates",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.CreateIndex(
                name: "IX_VisitUpdates_UpdatedByVetId",
                table: "VisitUpdates",
                column: "UpdatedByVetId");

            migrationBuilder.AddForeignKey(
                name: "FK_VisitUpdates_AspNetUsers_UpdatedByVetId",
                table: "VisitUpdates",
                column: "UpdatedByVetId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VisitUpdates_AspNetUsers_UpdatedByVetId",
                table: "VisitUpdates");

            migrationBuilder.DropIndex(
                name: "IX_VisitUpdates_UpdatedByVetId",
                table: "VisitUpdates");

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedByVetId",
                table: "VisitUpdates",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "UpdatedById",
                table: "VisitUpdates",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_VisitUpdates_UpdatedById",
                table: "VisitUpdates",
                column: "UpdatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_VisitUpdates_AspNetUsers_UpdatedById",
                table: "VisitUpdates",
                column: "UpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
