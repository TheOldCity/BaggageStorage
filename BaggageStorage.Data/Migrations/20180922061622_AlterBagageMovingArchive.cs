using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace BaggageStorage.Data.Migrations
{
    public partial class AlterBaggageMovingArchive : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BaggageMovingArchives_Clients_ClientId",
                table: "BaggageMovingArchives");

            migrationBuilder.DropForeignKey(
                name: "FK_BaggageMovingArchives_StoragePlaces_StoragePlaceId",
                table: "BaggageMovingArchives");

            migrationBuilder.DropForeignKey(
                name: "FK_BaggageMovingArchives_Users_UserInId",
                table: "BaggageMovingArchives");

            migrationBuilder.DropForeignKey(
                name: "FK_BaggageMovingArchives_Users_UserOutId",
                table: "BaggageMovingArchives");

            migrationBuilder.DropForeignKey(
                name: "FK_BaggageRegisters_BaggageMovingArchives_BaggageMovingArchiveId",
                table: "BaggageRegisters");

            migrationBuilder.DropForeignKey(
                name: "FK_CashOperations_BaggageMovingArchives_BaggageMovingArchiveId",
                table: "CashOperations");

            migrationBuilder.DropIndex(
                name: "IX_CashOperations_BaggageMovingArchiveId",
                table: "CashOperations");

            migrationBuilder.DropIndex(
                name: "IX_BaggageRegisters_BaggageMovingArchiveId",
                table: "BaggageRegisters");

            migrationBuilder.DropIndex(
                name: "IX_BaggageMovingArchives_ClientId",
                table: "BaggageMovingArchives");

            migrationBuilder.DropIndex(
                name: "IX_BaggageMovingArchives_StoragePlaceId",
                table: "BaggageMovingArchives");

            migrationBuilder.DropIndex(
                name: "IX_BaggageMovingArchives_UserInId",
                table: "BaggageMovingArchives");

            migrationBuilder.DropIndex(
                name: "IX_BaggageMovingArchives_UserOutId",
                table: "BaggageMovingArchives");

            migrationBuilder.DropColumn(
                name: "BaggageMovingArchiveId",
                table: "CashOperations");

            migrationBuilder.DropColumn(
                name: "BaggageMovingArchiveId",
                table: "BaggageRegisters");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BaggageMovingArchiveId",
                table: "CashOperations",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BaggageMovingArchiveId",
                table: "BaggageRegisters",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CashOperations_BaggageMovingArchiveId",
                table: "CashOperations",
                column: "BaggageMovingArchiveId");

            migrationBuilder.CreateIndex(
                name: "IX_BaggageRegisters_BaggageMovingArchiveId",
                table: "BaggageRegisters",
                column: "BaggageMovingArchiveId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_BaggageMovingArchives_Clients_ClientId",
                table: "BaggageMovingArchives",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BaggageMovingArchives_StoragePlaces_StoragePlaceId",
                table: "BaggageMovingArchives",
                column: "StoragePlaceId",
                principalTable: "StoragePlaces",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BaggageMovingArchives_Users_UserInId",
                table: "BaggageMovingArchives",
                column: "UserInId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BaggageMovingArchives_Users_UserOutId",
                table: "BaggageMovingArchives",
                column: "UserOutId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

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
    }
}
