using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Proyecto2026.BD.Migrations
{
    /// <inheritdoc />
    public partial class ActualizacionTablas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Estados",
                table: "Reservas",
                newName: "Estado");

            migrationBuilder.AddColumn<int>(
                name: "ReservaId",
                table: "Trabajos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TrabajoId",
                table: "Pagos",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Trabajos_ReservaId",
                table: "Trabajos",
                column: "ReservaId");

            migrationBuilder.CreateIndex(
                name: "IX_Pagos_TrabajoId",
                table: "Pagos",
                column: "TrabajoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Pagos_Trabajos_TrabajoId",
                table: "Pagos",
                column: "TrabajoId",
                principalTable: "Trabajos",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Trabajos_Reservas_ReservaId",
                table: "Trabajos",
                column: "ReservaId",
                principalTable: "Reservas",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pagos_Trabajos_TrabajoId",
                table: "Pagos");

            migrationBuilder.DropForeignKey(
                name: "FK_Trabajos_Reservas_ReservaId",
                table: "Trabajos");

            migrationBuilder.DropIndex(
                name: "IX_Trabajos_ReservaId",
                table: "Trabajos");

            migrationBuilder.DropIndex(
                name: "IX_Pagos_TrabajoId",
                table: "Pagos");

            migrationBuilder.DropColumn(
                name: "ReservaId",
                table: "Trabajos");

            migrationBuilder.DropColumn(
                name: "TrabajoId",
                table: "Pagos");

            migrationBuilder.RenameColumn(
                name: "Estado",
                table: "Reservas",
                newName: "Estados");
        }
    }
}
