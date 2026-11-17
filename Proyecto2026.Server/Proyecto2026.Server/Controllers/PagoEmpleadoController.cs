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


    [HttpGet("lista")]
    public async Task<ActionResult> ListaPagos()
    {
        var lista = await repositorio.Select();
        return Ok(lista.OrderByDescending(p => p.FechaPago).ThenByDescending(p => p.Id));
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
        var empleado = await repositorio.ObtenerEmpleadoPorId(dto.EmpleadoId);

        if (empleado == null)
            return NotFound("No se encontró el empleado indicado.");

        if (dto.Monto   > empleado.Salario)
            return BadRequest("El monto ingresado supera el saldo pendiente.");

        var pago = new PagoEmpleado
        {
            EmpleadoId = dto.EmpleadoId,
            TrabajoId = (dto.TrabajoId.HasValue && dto.TrabajoId.Value > 0) ? dto.TrabajoId.Value : null,
            Monto = dto.Monto,
            FechaPago = dto.FechaPago == default ? DateTime.Now : dto.FechaPago,
            MedioPago = dto.MedioPago
        };

        var pagoRegistrado = await repositorio.RegistrarPagoEmpleado(pago);

        return Ok(pagoRegistrado);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> ActualizarPagoEmpleado(int id, PagoEmpleadoDTO dto)
    {
        var lista = await repositorio.Select();
        var pago = lista.FirstOrDefault(p => p.Id == id);
        if (pago == null)
            return NotFound("No se encontró el registro de pago.");

        if (dto.Monto <= 0)
            return BadRequest("El monto debe ser mayor a cero.");

        pago.EmpleadoId = dto.EmpleadoId > 0 ? dto.EmpleadoId : pago.EmpleadoId;
        pago.TrabajoId = (dto.TrabajoId.HasValue && dto.TrabajoId.Value > 0) ? dto.TrabajoId.Value : null;
        pago.Monto = dto.Monto;
        pago.FechaPago = dto.FechaPago == default ? pago.FechaPago : dto.FechaPago;
        pago.MedioPago = string.IsNullOrWhiteSpace(dto.MedioPago) ? pago.MedioPago : dto.MedioPago;

        var actualizado = await repositorio.Update(pago);
        return Ok(actualizado);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> EliminarPagoEmpleado(int id)
    {
        var eliminado = await repositorio.Delete(id);
        if (!eliminado)
            return NotFound("No se encontró el registro de pago.");

        return Ok();
    }

}
