using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace BaggageStorage.Data.Migrations
{
    public partial class NewBaggageRegister : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StorageId",
                table: "BaggageRegisters",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BaggageRegisters_StorageId",
                table: "BaggageRegisters",
                column: "StorageId");

            migrationBuilder.AddForeignKey(
                name: "FK_BaggageRegisters_Storages_StorageId",
                table: "BaggageRegisters",
                column: "StorageId",
                principalTable: "Storages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BaggageRegisters_Storages_StorageId",
                table: "BaggageRegisters");

            migrationBuilder.DropIndex(
                name: "IX_BaggageRegisters_StorageId",
                table: "BaggageRegisters");

            migrationBuilder.DropColumn(
                name: "StorageId",
                table: "BaggageRegisters");
        }
    }
}
