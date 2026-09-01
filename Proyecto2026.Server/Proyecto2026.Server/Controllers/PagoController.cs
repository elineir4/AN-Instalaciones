using Microsoft.AspNetCore.Mvc;
using Proyecto2026.BD.Datos.Entity;
using Proyecto2026.Repositorio.Repositorios;
using Proyecto2026.Shared.DTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Components.Forms;
namespace Proyecto2026.Server.Controllers;

[ApiController]
[Route("api/pagos")]
public class PagoController : ControllerBase
{
    private readonly PagoRepositorio repositorio;

    public PagoController(PagoRepositorio repositorio)
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
        var cliente = await repositorio.ObtenerClienteId(dto.clienteId);

        if (cliente == null)
            return NotFound("No se encontró el cliente indicado.");

        if (dto.montoAbonado > cliente.SaldoPendiente)
            return BadRequest("El monto ingresado supera el saldo pendiente.");

        var pago = new Pago
        {
            ClienteId = dto.clienteId,
            MontoAbonado = dto.montoAbonado,
            FechaPago = DateTime.Now,
            MedioPago = dto.medioPago
        };

        var pagoRegistrado = await repositorio.RegistrarPago(pago, cliente);

        return Ok(pagoRegistrado);
    }



}
