using Microsoft.AspNetCore.Mvc;
using Proyecto2026.BD.Datos.Entity;
using Proyecto2026.Repositorio.Repositorios;
using Proyecto2026.Shared.DTO;

namespace Proyecto2026.Server.Controllers;

[ApiController]
[Route("api/pagoempleado")]
public class PagoEmpleadoController : ControllerBase
{
    public readonly PagoEmpleadoRepositorio repositorio;

    public PagoEmpleadoController(PagoEmpleadoRepositorio repositorio)
    {
        this.repositorio = repositorio;
    }


    [HttpGet("lista/pendientes")]
    public async Task<ActionResult> ListaEmpleadosConPagosPendientes()
    {
        var lista = await repositorio.ObtenerEmpleadosConPagosPendientes();
        return Ok(lista);
    }

    [HttpGet("empleado/{id}")]
    public async Task<ActionResult> ObtenerEmpleadoPorId(int id)
    {
        var empleado = await repositorio.ObtenerEmpleadoPorId(id);
        if (empleado == null)
        {
            return NotFound();
        }
        return Ok(empleado);
    }

    [HttpPost("registrar")]
    public async Task<ActionResult> RegistrarPagoEmpleado(PagoEmpleadoDTO dto)
    {
        var empleado = await repositorio.ObtenerEmpleadoPorId(dto.empleadoId);

        if (empleado == null)
            return NotFound("No se encontró el empleado indicado.");

        if (dto.montoAbonado > empleado.Salario)
            return BadRequest("El monto ingresado supera el saldo pendiente.");

        var pago = new PagoEmpleado
        {
            EmpleadoId = dto.empleadoId,
            Monto = dto.montoAbonado,
            FechaPago = DateTime.Now,
            MedioPago = dto.medioPago
        };

        var pagoRegistrado = await repositorio.RegistrarPagoEmpleado(pago);

        return Ok(pagoRegistrado);
    }

}
