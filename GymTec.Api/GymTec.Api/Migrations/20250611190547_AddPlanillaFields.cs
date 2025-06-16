using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace GymTec.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddPlanillaFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Role",
                table: "Users",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<decimal>(
                name: "Salario",
                table: "Users",
                type: "numeric(12,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SucursalId",
                table: "Users",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TipoPlanillaId",
                table: "Users",
                type: "integer",
                nullable: true);

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

            migrationBuilder.CreateTable(
                name: "clases_impartidas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Detalle = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_clases_impartidas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_clases_impartidas_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "horas_trabajadas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    HoraEntrada = table.Column<TimeSpan>(type: "interval", nullable: false),
                    HoraSalida = table.Column<TimeSpan>(type: "interval", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_horas_trabajadas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_horas_trabajadas_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TiposPlanilla",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Descripcion = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposPlanilla", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_SucursalId",
                table: "Users",
                column: "SucursalId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_TipoPlanillaId",
                table: "Users",
                column: "TipoPlanillaId");

            migrationBuilder.CreateIndex(
                name: "IX_SpaReservations_UserId1",
                table: "SpaReservations",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_ClaseReservations_UserId1",
                table: "ClaseReservations",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_clases_impartidas_UserId",
                table: "clases_impartidas",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_horas_trabajadas_UserId",
                table: "horas_trabajadas",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TiposPlanilla_Descripcion",
                table: "TiposPlanilla",
                column: "Descripcion",
                unique: true);

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

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Sucursales_SucursalId",
                table: "Users",
                column: "SucursalId",
                principalTable: "Sucursales",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_TiposPlanilla_TipoPlanillaId",
                table: "Users",
                column: "TipoPlanillaId",
                principalTable: "TiposPlanilla",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClaseReservations_Users_UserId1",
                table: "ClaseReservations");

            migrationBuilder.DropForeignKey(
                name: "FK_SpaReservations_Users_UserId1",
                table: "SpaReservations");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Sucursales_SucursalId",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_TiposPlanilla_TipoPlanillaId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "clases_impartidas");

            migrationBuilder.DropTable(
                name: "horas_trabajadas");

            migrationBuilder.DropTable(
                name: "TiposPlanilla");

            migrationBuilder.DropIndex(
                name: "IX_Users_SucursalId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_TipoPlanillaId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_SpaReservations_UserId1",
                table: "SpaReservations");

            migrationBuilder.DropIndex(
                name: "IX_ClaseReservations_UserId1",
                table: "ClaseReservations");

            migrationBuilder.DropColumn(
                name: "Salario",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SucursalId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TipoPlanillaId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "SpaReservations");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "ClaseReservations");

            migrationBuilder.AlterColumn<int>(
                name: "Role",
                table: "Users",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);
        }
    }
}
