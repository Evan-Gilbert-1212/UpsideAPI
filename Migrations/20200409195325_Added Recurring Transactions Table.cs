using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace UpsideAPI.Migrations
{
    public partial class AddedRecurringTransactionsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RecurringTransactions",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TransactionType = table.Column<string>(nullable: true),
                    TransactionCategory = table.Column<string>(nullable: true),
                    TransactionName = table.Column<string>(nullable: true),
                    TransactionDate = table.Column<DateTime>(nullable: false),
                    TransactionAmount = table.Column<double>(nullable: false),
                    RecurringFrequency = table.Column<string>(nullable: true),
                    UserID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecurringTransactions", x => x.ID);
                    table.ForeignKey(
                        name: "FK_RecurringTransactions_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RecurringTransactions_UserID",
                table: "RecurringTransactions",
                column: "UserID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RecurringTransactions");
        }
    }
}
