using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace BaggageStorage.Data.Migrations
{
    public partial class GlobalChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BaggageMovings_Storages_StorageId",
                table: "BaggageMovings");

            migrationBuilder.DropForeignKey(
                name: "FK_BaggageMovings_UserConnection_UserConnectionSessionId",
                table: "BaggageMovings");

            migrationBuilder.DropForeignKey(
                name: "FK_Storages_UserConnection_UserConnectionSessionId",
                table: "Storages");

            migrationBuilder.DropForeignKey(
                name: "FK_StoragePlaces_BaggageMovings_BaggageMovingId",
                table: "StoragePlaces");

            migrationBuilder.DropForeignKey(
                name: "FK_StoragePlaces_Customers_CustomerId",
                table: "StoragePlaces");

            migrationBuilder.DropForeignKey(
                name: "FK_StoragePlaces_UserConnection_UserConnectionSessionId",
                table: "StoragePlaces");

            migrationBuilder.DropIndex(
                name: "IX_StoragePlaces_BaggageMovingId",
                table: "StoragePlaces");

            migrationBuilder.DropIndex(
                name: "IX_StoragePlaces_CustomerId",
                table: "StoragePlaces");

            migrationBuilder.DropIndex(
                name: "IX_StoragePlaces_UserConnectionSessionId",
                table: "StoragePlaces");

            migrationBuilder.DropIndex(
                name: "IX_Storages_UserConnectionSessionId",
                table: "Storages");

            migrationBuilder.DropColumn(
                name: "BaggageMovingId",
                table: "StoragePlaces");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "StoragePlaces");

            migrationBuilder.DropColumn(
                name: "UserConnectionSessionId",
                table: "StoragePlaces");

            migrationBuilder.DropColumn(
                name: "UserConnectionSessionId",
                table: "Storages");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "BaggageMovings");

            migrationBuilder.DropColumn(
                name: "UserIn",
                table: "BaggageMovings");

            migrationBuilder.RenameColumn(
                name: "UserOut",
                table: "BaggageMovings",
                newName: "UserInId");

            migrationBuilder.RenameColumn(
                name: "UserConnectionSessionId",
                table: "BaggageMovings",
                newName: "UserOutId");

            migrationBuilder.RenameIndex(
                name: "IX_BaggageMovings_UserConnectionSessionId",
                table: "BaggageMovings",
                newName: "IX_BaggageMovings_UserOutId");

            migrationBuilder.AlterColumn<string>(
                name: "StorageId",
                table: "BaggageMovings",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UserOutId",
                table: "BaggageMovings",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "CashOperations",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Amount = table.Column<decimal>(nullable: false),
                    BaggageMovingId = table.Column<string>(nullable: true),
                    StorageId = table.Column<string>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    Operation = table.Column<int>(nullable: false),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CashOperations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CashOperations_BaggageMovings_BaggageMovingId",
                        column: x => x.BaggageMovingId,
                        principalTable: "BaggageMovings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CashOperations_Storages_StorageId",
                        column: x => x.StorageId,
                        principalTable: "Storages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CashOperations_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BaggageMovings_UserInId",
                table: "BaggageMovings",
                column: "UserInId");

            migrationBuilder.CreateIndex(
                name: "IX_CashOperations_BaggageMovingId",
                table: "CashOperations",
                column: "BaggageMovingId");

            migrationBuilder.CreateIndex(
                name: "IX_CashOperations_StorageId",
                table: "CashOperations",
                column: "StorageId");

            migrationBuilder.CreateIndex(
                name: "IX_CashOperations_UserId",
                table: "CashOperations",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_BaggageMovings_Storages_StorageId",
                table: "BaggageMovings",
                column: "StorageId",
                principalTable: "Storages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BaggageMovings_Users_UserInId",
                table: "BaggageMovings",
                column: "UserInId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BaggageMovings_Users_UserOutId",
                table: "BaggageMovings",
                column: "UserOutId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BaggageMovings_Storages_StorageId",
                table: "BaggageMovings");

            migrationBuilder.DropForeignKey(
                name: "FK_BaggageMovings_Users_UserInId",
                table: "BaggageMovings");

            migrationBuilder.DropForeignKey(
                name: "FK_BaggageMovings_Users_UserOutId",
                table: "BaggageMovings");

            migrationBuilder.DropTable(
                name: "CashOperations");

            migrationBuilder.DropIndex(
                name: "IX_BaggageMovings_UserInId",
                table: "BaggageMovings");

            migrationBuilder.RenameColumn(
                name: "UserOutId",
                table: "BaggageMovings",
                newName: "UserConnectionSessionId");

            migrationBuilder.RenameColumn(
                name: "UserInId",
                table: "BaggageMovings",
                newName: "UserOut");

            migrationBuilder.RenameIndex(
                name: "IX_BaggageMovings_UserOutId",
                table: "BaggageMovings",
                newName: "IX_BaggageMovings_UserConnectionSessionId");

            migrationBuilder.AddColumn<string>(
                name: "BaggageMovingId",
                table: "StoragePlaces",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerId",
                table: "StoragePlaces",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserConnectionSessionId",
                table: "StoragePlaces",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserConnectionSessionId",
                table: "Storages",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "StorageId",
                table: "BaggageMovings",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "UserConnectionSessionId",
                table: "BaggageMovings",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "BaggageMovings",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserIn",
                table: "BaggageMovings",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_StoragePlaces_BaggageMovingId",
                table: "StoragePlaces",
                column: "BaggageMovingId");

            migrationBuilder.CreateIndex(
                name: "IX_StoragePlaces_CustomerId",
                table: "StoragePlaces",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_StoragePlaces_UserConnectionSessionId",
                table: "StoragePlaces",
                column: "UserConnectionSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_Storages_UserConnectionSessionId",
                table: "Storages",
                column: "UserConnectionSessionId");

            migrationBuilder.AddForeignKey(
                name: "FK_BaggageMovings_Storages_StorageId",
                table: "BaggageMovings",
                column: "StorageId",
                principalTable: "Storages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BaggageMovings_UserConnection_UserConnectionSessionId",
                table: "BaggageMovings",
                column: "UserConnectionSessionId",
                principalTable: "UserConnection",
                principalColumn: "SessionId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Storages_UserConnection_UserConnectionSessionId",
                table: "Storages",
                column: "UserConnectionSessionId",
                principalTable: "UserConnection",
                principalColumn: "SessionId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StoragePlaces_BaggageMovings_BaggageMovingId",
                table: "StoragePlaces",
                column: "BaggageMovingId",
                principalTable: "BaggageMovings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StoragePlaces_Customers_CustomerId",
                table: "StoragePlaces",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StoragePlaces_UserConnection_UserConnectionSessionId",
                table: "StoragePlaces",
                column: "UserConnectionSessionId",
                principalTable: "UserConnection",
                principalColumn: "SessionId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
