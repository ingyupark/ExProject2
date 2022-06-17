using Microsoft.EntityFrameworkCore.Migrations;

namespace Server.Migrations
{
    public partial class ItemSetCount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSetCount",
                table: "Item",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSetCount",
                table: "Item");
        }
    }
}
