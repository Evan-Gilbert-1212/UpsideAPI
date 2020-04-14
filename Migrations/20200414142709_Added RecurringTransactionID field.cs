using Microsoft.EntityFrameworkCore.Migrations;

namespace UpsideAPI.Migrations
{
    public partial class AddedRecurringTransactionIDfield : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RecurringTransactionID",
                table: "Revenues",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RecurringTransactionID",
                table: "Expenses",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RecurringTransactionID",
                table: "Revenues");

            migrationBuilder.DropColumn(
                name: "RecurringTransactionID",
                table: "Expenses");
        }
    }
}
