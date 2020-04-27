using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace UpsideAPI.Migrations
{
    public partial class AddedDemoAccountfieldstoUsertable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "AccountCreatedTime",
                table: "Users",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsDemoAccount",
                table: "Users",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountCreatedTime",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsDemoAccount",
                table: "Users");
        }
    }
}
