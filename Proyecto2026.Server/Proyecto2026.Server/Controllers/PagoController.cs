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
    private readonly ITrabajoRepositorio trabajoRepositorio;

    public PagoController(IPagoRepositorio1 repositorio, ITrabajoRepositorio trabajoRepositorio)
    {
        this.repositorio = repositorio;
        this.trabajoRepositorio = trabajoRepositorio;
    }

    [HttpGet("saldospendientes")]
    public async Task<ActionResult> SaldosPendientes()
    {
        var clientes = await repositorio.ClientesSaldoPendiente();
        var pagos = await repositorio.Select();
        var trabajos = await trabajoRepositorio.Select();

        var ultimoPagoPorCliente = pagos
            .GroupBy(p => p.ClienteId)
            .ToDictionary(g => g.Key, g => g.Max(p => p.FechaPago));

        var primerTrabajoPorCliente = trabajos
            .Where(t => t.FechaInicio > DateTime.MinValue)
            .GroupBy(t => t.ClienteId)
            .ToDictionary(g => g.Key, g => g.Min(t => t.FechaInicio));

        var hoy = DateTime.Today;

        var resultado = clientes.Select(c =>
        {
            ultimoPagoPorCliente.TryGetValue(c.Id, out var ultimoPago);
            var tieneUltimoPago = ultimoPago != default;

            primerTrabajoPorCliente.TryGetValue(c.Id, out var primerTrabajo);
            var tienePrimerTrabajo = primerTrabajo != default;

            DateTime? fechaReferencia = tieneUltimoPago
                ? ultimoPago
                : (tienePrimerTrabajo ? primerTrabajo : null);

            DateTime? fechaProximoPago = fechaReferencia?.Date.AddDays(7);
            int diasDesdeRef = fechaReferencia.HasValue
                ? Math.Max(0, (hoy - fechaReferencia.Value.Date).Days)
                : 0;

            int semanasAtraso = diasDesdeRef > 7 ? (diasDesdeRef - 1) / 7 : 0;
            string estadoSemanal;
            if (semanasAtraso >= 1)
            {
                estadoSemanal = semanasAtraso == 1 ? "Se pasó 1 semana" : $"Se pasó {semanasAtraso} semanas";
            }
            else if (fechaReferencia.HasValue && diasDesdeRef >= 5)
            {
                estadoSemanal = "Vence esta semana";
            }
            else
            {
                estadoSemanal = "Al día";
            }

            return new
            {
                c.Id,
                c.Nombre,
                c.Apellido,
                c.Telefono,
                c.Email,
                c.SaldoPendiente,
                UltimoPagoFecha = tieneUltimoPago ? ultimoPago : (DateTime?)null,
                FechaProximoPago = fechaProximoPago,
                DiasSinPagar = diasDesdeRef,
                SemanasAtraso = semanasAtraso,
                EstadoSemanal = estadoSemanal
            };
        }).ToList();

        return Ok(resultado);
    }

    [HttpGet("cobrados-semana")]
    public async Task<ActionResult> CobradosSemana()
    {
        var hoy = DateTime.Today;
        var diff = (7 + (hoy.DayOfWeek - DayOfWeek.Monday)) % 7;
        var inicioSemana = hoy.AddDays(-diff).Date;

        var pagos = await repositorio.Select();
        var pagosSemana = pagos.Where(p => p.FechaPago >= inicioSemana).ToList();

        return Ok(new
        {
            Total = pagosSemana.Sum(p => p.MontoAbonado),
            Cantidad = pagosSemana.Count
        });
    }

    [HttpGet("resumen-trabajos")]
    public async Task<ActionResult<Dictionary<int, decimal>>> ResumenPorTrabajo()
    {
        var pagos = await repositorio.Select();
        var resumen = pagos
            .Where(p => p.TrabajoId.HasValue)
            .GroupBy(p => p.TrabajoId!.Value)
            .ToDictionary(g => g.Key, g => g.Sum(p => p.MontoAbonado));

        return Ok(resumen);
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
            TrabajoId = dto.TrabajoId > 0 ? dto.TrabajoId : null,
            MontoAbonado = dto.MontoAbonado,
            FechaPago = DateTime.Now,
            MedioPago = dto.MedioPago
        };

        var pagoRegistrado = await repositorio.RegistrarPago(pago, cliente);

        return Ok(pagoRegistrado);
    }



}
