using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Proyecto2026.BD.Migrations
{
    /// <inheritdoc />
    public partial class MontoEnReserva : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Monto",
                table: "Reservas",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "MontoSumadoAlSaldo",
                table: "Reservas",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Monto",
                table: "Reservas");

            migrationBuilder.DropColumn(
                name: "MontoSumadoAlSaldo",
                table: "Reservas");
        }
    }
}
