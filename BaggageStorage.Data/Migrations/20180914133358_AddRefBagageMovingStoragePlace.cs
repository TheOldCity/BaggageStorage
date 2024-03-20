using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace BaggageStorage.Data.Migrations
{
    public partial class AddRefBaggageMovingStoragePlace : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StoragePlaceId",
                table: "BaggageMovings",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_BaggageMovings_StoragePlaceId",
                table: "BaggageMovings",
                column: "StoragePlaceId");

            migrationBuilder.AddForeignKey(
                name: "FK_BaggageMovings_StoragePlaces_StoragePlaceId",
                table: "BaggageMovings",
                column: "StoragePlaceId",
                principalTable: "StoragePlaces",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BaggageMovings_StoragePlaces_StoragePlaceId",
                table: "BaggageMovings");

            migrationBuilder.DropIndex(
                name: "IX_BaggageMovings_StoragePlaceId",
                table: "BaggageMovings");

            migrationBuilder.DropColumn(
                name: "StoragePlaceId",
                table: "BaggageMovings");
        }
    }
}
