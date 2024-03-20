using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace BaggageStorage.Data.Migrations
{
    public partial class BaggageMoving : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BaggageMovingId",
                table: "StoragePlaces",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BaggageMovings",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    StorageId = table.Column<string>(nullable: true),
                    DateIn = table.Column<DateTime>(nullable: false),
                    DateOut = table.Column<DateTime>(nullable: false),
                    UserConnectionSessionId = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false),
                    UserIn = table.Column<string>(nullable: false),
                    UserOut = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaggageMovings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BaggageMovings_Storages_StorageId",
                        column: x => x.StorageId,
                        principalTable: "Storages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BaggageMovings_UserConnection_UserConnectionSessionId",
                        column: x => x.UserConnectionSessionId,
                        principalTable: "UserConnection",
                        principalColumn: "SessionId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StoragePlaces_BaggageMovingId",
                table: "StoragePlaces",
                column: "BaggageMovingId");

            migrationBuilder.CreateIndex(
                name: "IX_BaggageMovings_StorageId",
                table: "BaggageMovings",
                column: "StorageId");

            migrationBuilder.CreateIndex(
                name: "IX_BaggageMovings_UserConnectionSessionId",
                table: "BaggageMovings",
                column: "UserConnectionSessionId");

            migrationBuilder.AddForeignKey(
                name: "FK_StoragePlaces_BaggageMovings_BaggageMovingId",
                table: "StoragePlaces",
                column: "BaggageMovingId",
                principalTable: "BaggageMovings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StoragePlaces_BaggageMovings_BaggageMovingId",
                table: "StoragePlaces");

            migrationBuilder.DropTable(
                name: "BaggageMovings");

            migrationBuilder.DropIndex(
                name: "IX_StoragePlaces_BaggageMovingId",
                table: "StoragePlaces");

            migrationBuilder.DropColumn(
                name: "BaggageMovingId",
                table: "StoragePlaces");
        }
    }
}
