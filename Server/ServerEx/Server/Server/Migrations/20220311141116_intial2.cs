using Microsoft.EntityFrameworkCore.Migrations;

namespace Server.Migrations
{
    public partial class intial2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Hrecovery",
                table: "Player",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaxSp",
                table: "Player",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Sp",
                table: "Player",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Sprecovery",
                table: "Player",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Hrecovery",
                table: "Player");

            migrationBuilder.DropColumn(
                name: "MaxSp",
                table: "Player");

            migrationBuilder.DropColumn(
                name: "Sp",
                table: "Player");

            migrationBuilder.DropColumn(
                name: "Sprecovery",
                table: "Player");
        }
    }
}
