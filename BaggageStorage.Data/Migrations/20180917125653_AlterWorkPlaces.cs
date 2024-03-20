using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace BaggageStorage.Data.Migrations
{
    public partial class AlterWorkPlaces : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CustomerId",
                table: "WorkPlaces",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_WorkPlaces_CustomerId",
                table: "WorkPlaces",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkPlaces_Customers_CustomerId",
                table: "WorkPlaces",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkPlaces_Customers_CustomerId",
                table: "WorkPlaces");

            migrationBuilder.DropIndex(
                name: "IX_WorkPlaces_CustomerId",
                table: "WorkPlaces");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "WorkPlaces");
        }
    }
}
