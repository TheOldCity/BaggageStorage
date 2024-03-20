using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace BaggageStorage.Data.Migrations
{
    public partial class AlterCashOperationsArchive : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CashOperationArchives_BaggageMovings_BaggageMovingId",
                table: "CashOperationArchives");

            migrationBuilder.DropForeignKey(
                name: "FK_CashOperationArchives_Storages_StorageId",
                table: "CashOperationArchives");

            migrationBuilder.DropForeignKey(
                name: "FK_CashOperationArchives_Users_UserId",
                table: "CashOperationArchives");

            migrationBuilder.DropIndex(
                name: "IX_CashOperationArchives_BaggageMovingId",
                table: "CashOperationArchives");

            migrationBuilder.DropIndex(
                name: "IX_CashOperationArchives_StorageId",
                table: "CashOperationArchives");

            migrationBuilder.DropIndex(
                name: "IX_CashOperationArchives_UserId",
                table: "CashOperationArchives");

            migrationBuilder.AddColumn<string>(
                name: "UserDeletedId",
                table: "CashOperationArchives",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserDeletedId",
                table: "CashOperationArchives");

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
                name: "FK_CashOperationArchives_BaggageMovings_BaggageMovingId",
                table: "CashOperationArchives",
                column: "BaggageMovingId",
                principalTable: "BaggageMovings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CashOperationArchives_Storages_StorageId",
                table: "CashOperationArchives",
                column: "StorageId",
                principalTable: "Storages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CashOperationArchives_Users_UserId",
                table: "CashOperationArchives",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
