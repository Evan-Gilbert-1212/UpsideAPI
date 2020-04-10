using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace UpsideAPI.Migrations
{
    public partial class RenamedTransactionDatetoFirstPaymentDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TransactionDate",
                table: "RecurringTransactions");

            migrationBuilder.AddColumn<DateTime>(
                name: "FirstPaymentDate",
                table: "RecurringTransactions",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstPaymentDate",
                table: "RecurringTransactions");

            migrationBuilder.AddColumn<DateTime>(
                name: "TransactionDate",
                table: "RecurringTransactions",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
