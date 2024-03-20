using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace BaggageStorage.Data.Migrations
{
    public partial class Archives : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BaggageMovingArchiveId",
                table: "CashOperations",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BaggageMovingArchiveId",
                table: "BaggageRegisters",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BaggageMovingArchives",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Amount = table.Column<decimal>(nullable: false),
                    AmountOfDays = table.Column<int>(nullable: false),
                    AmountOfPlaces = table.Column<int>(nullable: false),
                    StorageId = table.Column<string>(nullable: false),
                    ClientId = table.Column<string>(nullable: false),
                    DateIn = table.Column<DateTime>(nullable: false),
                    DateOut = table.Column<DateTime>(nullable: false),
                    StoragePlaceId = table.Column<string>(nullable: false),
                    UserInId = table.Column<string>(nullable: false),
                    UserOutId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaggageMovingArchives", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BaggageMovingArchives_Storages_StorageId",
                        column: x => x.StorageId,
                        principalTable: "Storages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BaggageMovingArchives_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BaggageMovingArchives_StoragePlaces_StoragePlaceId",
                        column: x => x.StoragePlaceId,
                        principalTable: "StoragePlaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BaggageMovingArchives_Users_UserInId",
                        column: x => x.UserInId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BaggageMovingArchives_Users_UserOutId",
                        column: x => x.UserOutId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CashOperationArchives",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Amount = table.Column<decimal>(nullable: false),
                    BaggageMovingId = table.Column<string>(nullable: true),
                    StorageId = table.Column<string>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    Operation = table.Column<int>(nullable: false),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CashOperationArchives", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CashOperationArchives_BaggageMovings_BaggageMovingId",
                        column: x => x.BaggageMovingId,
                        principalTable: "BaggageMovings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CashOperationArchives_Storages_StorageId",
                        column: x => x.StorageId,
                        principalTable: "Storages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CashOperationArchives_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CashOperations_BaggageMovingArchiveId",
                table: "CashOperations",
                column: "BaggageMovingArchiveId");

            migrationBuilder.CreateIndex(
                name: "IX_BaggageRegisters_BaggageMovingArchiveId",
                table: "BaggageRegisters",
                column: "BaggageMovingArchiveId");

            migrationBuilder.CreateIndex(
                name: "IX_BaggageMovingArchives_StorageId",
                table: "BaggageMovingArchives",
                column: "StorageId");

            migrationBuilder.CreateIndex(
                name: "IX_BaggageMovingArchives_ClientId",
                table: "BaggageMovingArchives",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_BaggageMovingArchives_StoragePlaceId",
                table: "BaggageMovingArchives",
                column: "StoragePlaceId");

            migrationBuilder.CreateIndex(
                name: "IX_BaggageMovingArchives_UserInId",
                table: "BaggageMovingArchives",
                column: "UserInId");

            migrationBuilder.CreateIndex(
                name: "IX_BaggageMovingArchives_UserOutId",
                table: "BaggageMovingArchives",
                column: "UserOutId");

            migrationBuilder.CreateIndex(
                name: "IX_CashOperationArchives_BaggageMovingId",
                table: "CashOperationArchives",
                column: "BaggageMovingId");

            migrationBuilder.CreateIndex(
                name: "IX_CashOperationArchives_StorageId",
                table: "CashOperationArchives",
                column: "StorageId");

            migrationBuilder.CreateIndex(
                name: "IX_CashOperationArchives_UserId",
                table: "CashOperationArchives",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_BaggageRegisters_BaggageMovingArchives_BaggageMovingArchiveId",
                table: "BaggageRegisters",
                column: "BaggageMovingArchiveId",
                principalTable: "BaggageMovingArchives",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CashOperations_BaggageMovingArchives_BaggageMovingArchiveId",
                table: "CashOperations",
                column: "BaggageMovingArchiveId",
                principalTable: "BaggageMovingArchives",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BaggageRegisters_BaggageMovingArchives_BaggageMovingArchiveId",
                table: "BaggageRegisters");

            migrationBuilder.DropForeignKey(
                name: "FK_CashOperations_BaggageMovingArchives_BaggageMovingArchiveId",
                table: "CashOperations");

            migrationBuilder.DropTable(
                name: "BaggageMovingArchives");

            migrationBuilder.DropTable(
                name: "CashOperationArchives");

            migrationBuilder.DropIndex(
                name: "IX_CashOperations_BaggageMovingArchiveId",
                table: "CashOperations");

            migrationBuilder.DropIndex(
                name: "IX_BaggageRegisters_BaggageMovingArchiveId",
                table: "BaggageRegisters");

            migrationBuilder.DropColumn(
                name: "BaggageMovingArchiveId",
                table: "CashOperations");

            migrationBuilder.DropColumn(
                name: "BaggageMovingArchiveId",
                table: "BaggageRegisters");
        }
    }
}
