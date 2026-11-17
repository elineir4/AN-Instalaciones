using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Proyecto2026.BD.Migrations
{
    /// <inheritdoc />
    public partial class TrabajoIdOpcionalPagoEmpleado : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PagosEmpleados_Trabajos_TrabajoId",
                table: "PagosEmpleados");

            migrationBuilder.AlterColumn<int>(
                name: "TrabajoId",
                table: "PagosEmpleados",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_PagosEmpleados_Trabajos_TrabajoId",
                table: "PagosEmpleados",
                column: "TrabajoId",
                principalTable: "Trabajos",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PagosEmpleados_Trabajos_TrabajoId",
                table: "PagosEmpleados");

            migrationBuilder.AlterColumn<int>(
                name: "TrabajoId",
                table: "PagosEmpleados",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PagosEmpleados_Trabajos_TrabajoId",
                table: "PagosEmpleados",
                column: "TrabajoId",
                principalTable: "Trabajos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
