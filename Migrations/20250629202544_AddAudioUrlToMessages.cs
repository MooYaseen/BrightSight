using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Graduation.Migrations
{
    public partial class AddAudioUrlToMessages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AudioUrl",
                table: "Messages",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AudioUrl",
                table: "Messages");
        }
    }
}
