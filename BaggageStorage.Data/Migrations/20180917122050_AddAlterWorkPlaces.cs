using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace BaggageStorage.Data.Migrations
{
    public partial class AddAlterWorkPlaces : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StorageId",
                table: "WorkPlaces",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkPlaces_StorageId",
                table: "WorkPlaces",
                column: "StorageId");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkPlaces_Storages_StorageId",
                table: "WorkPlaces",
                column: "StorageId",
                principalTable: "Storages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkPlaces_Storages_StorageId",
                table: "WorkPlaces");

            migrationBuilder.DropIndex(
                name: "IX_WorkPlaces_StorageId",
                table: "WorkPlaces");

            migrationBuilder.DropColumn(
                name: "StorageId",
                table: "WorkPlaces");
        }
    }
}
