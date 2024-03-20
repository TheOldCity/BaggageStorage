using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace BaggageStorage.Data.Migrations
{
    public partial class AlterBaggageMoving : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Amount",
                table: "BaggageMovings",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AmountOfDays",
                table: "BaggageMovings",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AmountOfPlaces",
                table: "BaggageMovings",
                nullable: false,
                defaultValue: "");
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
