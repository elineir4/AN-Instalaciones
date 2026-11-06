using Microsoft.AspNetCore.Mvc;
using Proyecto2026.BD.Datos.Entity;
using Proyecto2026.Repositorio.Repositorios;
using Proyecto2026.Shared.DTO;

namespace Proyecto2026.Server.Controllers;


[ApiController]
[Route("api/pagos")]
public class PagoController : ControllerBase
{
    private readonly IPagoRepositorio1 repositorio;

    public PagoController(IPagoRepositorio1 repositorio)
    {
        this.repositorio = repositorio;
    }

    [HttpGet("saldospendientes")]
    public async Task<ActionResult> SaldosPendientes()
    {
        var cliente = await repositorio.ClientesSaldoPendiente();
        return Ok(cliente);
    }

    [HttpGet("cliente/{clienteId}")]
    public async Task<ActionResult> ObtenerCliente(int clienteId)
    {
        var cliente = await repositorio.ObtenerClienteId(clienteId);

        if (cliente == null)
            return NotFound("No se encontro el cliente con ese ID");

        return Ok(cliente);
    }

    [HttpPost("registrar")]
    public async Task<ActionResult> RegistrarPago(PagoDTO dto)
    {
        var cliente = await repositorio.ObtenerClienteId(dto.ClienteId);

        if (cliente == null)
            return NotFound("No se encontró el cliente indicado.");

        if (dto.MontoAbonado     > cliente.SaldoPendiente)
            return BadRequest("El monto ingresado supera el saldo pendiente.");

        if (dto.MontoAbonado <= 0)
            return BadRequest("El monto debe ser mayor a cero.");

        var pago = new Pago
        {
            ClienteId = dto.ClienteId,
            MontoAbonado = dto.MontoAbonado,
            FechaPago = DateTime.Now,
            MedioPago = dto.MedioPago
        };

        var pagoRegistrado = await repositorio.RegistrarPago(pago, cliente);

        return Ok(pagoRegistrado);
    }



}
