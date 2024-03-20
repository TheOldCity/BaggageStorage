using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace BaggageStorage.Data.Migrations
{
    public partial class DropBaggageRegisters : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BaggageRegisters");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BaggageRegisters",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    AmountOfDays = table.Column<string>(nullable: false),
                    BaggageMovingId = table.Column<string>(nullable: true),
                    StorageId = table.Column<string>(nullable: true),
                    ClientId = table.Column<string>(nullable: false),
                    DateIn = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaggageRegisters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BaggageRegisters_BaggageMovings_BaggageMovingId",
                        column: x => x.BaggageMovingId,
                        principalTable: "BaggageMovings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BaggageRegisters_Storages_StorageId",
                        column: x => x.StorageId,
                        principalTable: "Storages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BaggageRegisters_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BaggageRegisters_BaggageMovingId",
                table: "BaggageRegisters",
                column: "BaggageMovingId");

            migrationBuilder.CreateIndex(
                name: "IX_BaggageRegisters_StorageId",
                table: "BaggageRegisters",
                column: "StorageId");

            migrationBuilder.CreateIndex(
                name: "IX_BaggageRegisters_ClientId",
                table: "BaggageRegisters",
                column: "ClientId");
        }
    }
}
