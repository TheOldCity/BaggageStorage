using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace BaggageStorage.Data.Migrations
{
    public partial class AlterBaggageStorage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Storages_Customers_CustomerId",
                table: "Storages");

            migrationBuilder.AlterColumn<string>(
                name: "CustomerId",
                table: "Storages",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddForeignKey(
                name: "FK_Storages_Customers_CustomerId",
                table: "Storages",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Storages_Customers_CustomerId",
                table: "Storages");

            migrationBuilder.AlterColumn<string>(
                name: "CustomerId",
                table: "Storages",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Storages_Customers_CustomerId",
                table: "Storages",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
