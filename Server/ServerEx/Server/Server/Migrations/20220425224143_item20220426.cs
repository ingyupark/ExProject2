using Microsoft.EntityFrameworkCore.Migrations;

namespace Server.Migrations
{
    public partial class item20220426 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Damage",
                table: "Item",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Defence",
                table: "Item",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Damage",
                table: "Item");

            migrationBuilder.DropColumn(
                name: "Defence",
                table: "Item");
        }
    }
}
