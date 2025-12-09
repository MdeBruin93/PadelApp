using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PadelApp.Migrations
{
    /// <inheritdoc />
    public partial class AddsExtraKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BracketMatch_Brackets_BracketId",
                table: "BracketMatch");

            migrationBuilder.AlterColumn<Guid>(
                name: "BracketId",
                table: "BracketMatch",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BracketMatch_Brackets_BracketId",
                table: "BracketMatch",
                column: "BracketId",
                principalTable: "Brackets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BracketMatch_Brackets_BracketId",
                table: "BracketMatch");

            migrationBuilder.AlterColumn<Guid>(
                name: "BracketId",
                table: "BracketMatch",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_BracketMatch_Brackets_BracketId",
                table: "BracketMatch",
                column: "BracketId",
                principalTable: "Brackets",
                principalColumn: "Id");
        }
    }
}
