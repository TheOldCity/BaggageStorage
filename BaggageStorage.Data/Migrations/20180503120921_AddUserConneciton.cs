using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace BaggageStorage.Data.Migrations
{
    public partial class AddUserConneciton : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserConnection",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    IsOnline = table.Column<bool>(nullable: false),
                    IsRemember = table.Column<bool>(nullable: false),
                    LastRequestAction = table.Column<string>(maxLength: 50, nullable: true),
                    LastRequestController = table.Column<string>(maxLength: 50, nullable: true),
                    LastRequestDate = table.Column<DateTime>(nullable: false),
                    LastRequestPostParams = table.Column<string>(nullable: true),
                    LastRequestRawUrl = table.Column<string>(maxLength: 1024, nullable: true),
                    SessionEnd = table.Column<DateTime>(nullable: false),
                    SessionId = table.Column<string>(maxLength: 50, nullable: false),
                    UserAgent = table.Column<string>(maxLength: 1024, nullable: true),
                    UserId = table.Column<string>(nullable: false),
                    UserIp = table.Column<string>(maxLength: 39, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserConnection", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserConnection_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserConnection_UserId",
                table: "UserConnection",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserConnection");
        }
    }
}
