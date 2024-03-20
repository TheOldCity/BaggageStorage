using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace BaggageStorage.Data.Migrations
{
    public partial class AlterCashOperation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BaggageMovingId",
                table: "CashOperations",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CashOperations_BaggageMovingId",
                table: "CashOperations",
                column: "BaggageMovingId");

            migrationBuilder.AddForeignKey(
                name: "FK_CashOperations_BaggageMovings_BaggageMovingId",
                table: "CashOperations",
                column: "BaggageMovingId",
                principalTable: "BaggageMovings",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CashOperations_BaggageMovings_BaggageMovingId",
                table: "CashOperations");

            migrationBuilder.DropIndex(
                name: "IX_CashOperations_BaggageMovingId",
                table: "CashOperations");

            migrationBuilder.DropColumn(
                name: "BaggageMovingId",
                table: "CashOperations");
        }
    }
}
