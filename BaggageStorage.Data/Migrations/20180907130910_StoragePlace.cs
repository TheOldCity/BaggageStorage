using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace BaggageStorage.Data.Migrations
{
    public partial class StoragePlace : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StoragePlaces",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    StorageId = table.Column<string>(nullable: false),
                    CustomerId = table.Column<string>(nullable: false),
                    Place = table.Column<string>(nullable: false),
                    Price = table.Column<int>(nullable: false),
                    UserConnectionSessionId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoragePlaces", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoragePlaces_Storages_StorageId",
                        column: x => x.StorageId,
                        principalTable: "Storages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StoragePlaces_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StoragePlaces_UserConnection_UserConnectionSessionId",
                        column: x => x.UserConnectionSessionId,
                        principalTable: "UserConnection",
                        principalColumn: "SessionId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StoragePlaces_StorageId",
                table: "StoragePlaces",
                column: "StorageId");

            migrationBuilder.CreateIndex(
                name: "IX_StoragePlaces_CustomerId",
                table: "StoragePlaces",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_StoragePlaces_UserConnectionSessionId",
                table: "StoragePlaces",
                column: "UserConnectionSessionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StoragePlaces");
        }
    }
}
