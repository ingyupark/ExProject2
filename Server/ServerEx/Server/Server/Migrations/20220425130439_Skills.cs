using Microsoft.EntityFrameworkCore.Migrations;

namespace Server.Migrations
{
    public partial class Skills : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Level",
                table: "Item",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Skill",
                columns: table => new
                {
                    SkillDbId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TemplateId = table.Column<int>(nullable: false),
                    Level = table.Column<int>(nullable: false),
                    Damage = table.Column<int>(nullable: false),
                    Spconsumption = table.Column<int>(nullable: false),
                    SkillSlot = table.Column<int>(nullable: false),
                    SkillEquipped = table.Column<bool>(nullable: false),
                    OwnerDbId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Skill", x => x.SkillDbId);
                    table.ForeignKey(
                        name: "FK_Skill_Player_OwnerDbId",
                        column: x => x.OwnerDbId,
                        principalTable: "Player",
                        principalColumn: "PlayerDbId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Skill_OwnerDbId",
                table: "Skill",
                column: "OwnerDbId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Skill");

            migrationBuilder.DropColumn(
                name: "Level",
                table: "Item");
        }
    }
}
