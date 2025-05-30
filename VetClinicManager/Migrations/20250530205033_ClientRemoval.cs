using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VetClinicManager.Migrations
{
    /// <inheritdoc />
    public partial class ClientRemoval : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Animals_Clients_ClientId",
                table: "Animals");

            migrationBuilder.DropTable(
                name: "Clients");

            migrationBuilder.DropIndex(
                name: "IX_Animals_ClientId",
                table: "Animals");

            migrationBuilder.RenameColumn(
                name: "ClientId",
                table: "Animals",
                newName: "UserId");

            migrationBuilder.AddColumn<string>(
                name: "UserId1",
                table: "Animals",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Animals_UserId1",
                table: "Animals",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Animals_AspNetUsers_UserId1",
                table: "Animals",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Animals_AspNetUsers_UserId1",
                table: "Animals");

            migrationBuilder.DropIndex(
                name: "IX_Animals_UserId1",
                table: "Animals");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Animals");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Animals",
                newName: "ClientId");

            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Animals_ClientId",
                table: "Animals",
                column: "ClientId");

            migrationBuilder.AddForeignKey(
                name: "FK_Animals_Clients_ClientId",
                table: "Animals",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
