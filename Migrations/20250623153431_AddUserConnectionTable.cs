using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Graduation.Migrations
{
    public partial class AddUserConnectionTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserConnections",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BlindId = table.Column<int>(type: "int", nullable: false),
                    SightId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserConnections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserConnections_AspNetUsers_BlindId",
                        column: x => x.BlindId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserConnections_AspNetUsers_SightId",
                        column: x => x.SightId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserConnections_BlindId",
                table: "UserConnections",
                column: "BlindId");

            migrationBuilder.CreateIndex(
                name: "IX_UserConnections_SightId",
                table: "UserConnections",
                column: "SightId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserConnections");
        }
    }
}
