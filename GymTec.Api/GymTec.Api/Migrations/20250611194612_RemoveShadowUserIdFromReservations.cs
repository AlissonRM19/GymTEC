using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymTec.Api.Migrations
{
    /// <inheritdoc />
    public partial class RemoveShadowUserIdFromReservations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClaseReservations_Users_UserId1",
                table: "ClaseReservations");

            migrationBuilder.DropForeignKey(
                name: "FK_SpaReservations_Users_UserId1",
                table: "SpaReservations");

            migrationBuilder.DropIndex(
                name: "IX_SpaReservations_UserId1",
                table: "SpaReservations");

            migrationBuilder.DropIndex(
                name: "IX_ClaseReservations_UserId1",
                table: "ClaseReservations");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "SpaReservations");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "ClaseReservations");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserId1",
                table: "SpaReservations",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId1",
                table: "ClaseReservations",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SpaReservations_UserId1",
                table: "SpaReservations",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_ClaseReservations_UserId1",
                table: "ClaseReservations",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_ClaseReservations_Users_UserId1",
                table: "ClaseReservations",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SpaReservations_Users_UserId1",
                table: "SpaReservations",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
