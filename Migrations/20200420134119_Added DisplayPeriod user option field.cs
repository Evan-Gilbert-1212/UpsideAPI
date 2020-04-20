using Microsoft.EntityFrameworkCore.Migrations;

namespace UpsideAPI.Migrations
{
    public partial class AddedDisplayPerioduseroptionfield : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DisplayPeriod",
                table: "Users",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisplayPeriod",
                table: "Users");
        }
    }
}
