using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace BaggageStorage.Data.Migrations
{
    public partial class AlterUserConnection1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserConnection",
                table: "UserConnection");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "UserConnection");

            migrationBuilder.DropColumn(
                name: "SessionEnd",
                table: "UserConnection");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserConnection",
                table: "UserConnection",
                column: "SessionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserConnection",
                table: "UserConnection");

            migrationBuilder.AddColumn<string>(
                name: "Id",
                table: "UserConnection",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "SessionEnd",
                table: "UserConnection",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserConnection",
                table: "UserConnection",
                column: "Id");
        }
    }
}
