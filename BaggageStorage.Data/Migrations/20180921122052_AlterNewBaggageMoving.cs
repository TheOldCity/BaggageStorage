using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace BaggageStorage.Data.Migrations
{
    public partial class AlterNewBaggageMoving : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Amount",
                table: "BaggageMovings",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "AmountOfDays",
                table: "BaggageMovings",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AmountOfPlaces",
                table: "BaggageMovings",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Amount",
                table: "BaggageMovings");

            migrationBuilder.DropColumn(
                name: "AmountOfDays",
                table: "BaggageMovings");

            migrationBuilder.DropColumn(
                name: "AmountOfPlaces",
                table: "BaggageMovings");
        }
    }
}
