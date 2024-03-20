using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace BaggageStorage.Data.Migrations
{
    public partial class AlteWorkPlacwAddHarwareData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IpAdressHardwareService",
                table: "WorkPlaces",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PortHardwareService",
                table: "WorkPlaces",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IpAdressHardwareService",
                table: "WorkPlaces");

            migrationBuilder.DropColumn(
                name: "PortHardwareService",
                table: "WorkPlaces");
        }
    }
}
