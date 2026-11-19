using Microsoft.AspNetCore.Mvc;
using Proyecto2026.BD.Datos.Entity;
using Proyecto2026.Repositorio.Repositorios;
using Proyecto2026.Shared.DTO;

namespace Proyecto2026.Server.Controllers;

[ApiController]
[Route("api/Reservas")]
public class ReservaController : ControllerBase
{
    private readonly IReservaRepositorio repositorio;
    private readonly IClienteRepositorio clienteRepositorio;
    private readonly ITrabajoRepositorio trabajoRepositorio;

    public ReservaController(
        IReservaRepositorio repositorio,
        IClienteRepositorio clienteRepositorio,
        ITrabajoRepositorio trabajoRepositorio)
    {
        this.repositorio = repositorio;
        this.clienteRepositorio = clienteRepositorio;
        this.trabajoRepositorio = trabajoRepositorio;
    }

    // GET api/Reservas
    [HttpGet]
    public async Task<ActionResult<List<ReservaDTO>>> Get()
    {
        var reservas = await repositorio.ObtenerTodasConClienteAsync();
        var trabajos = await trabajoRepositorio.Select();
        var trabajoPorReserva = trabajos
            .Where(t => t.ReservaId.HasValue)
            .GroupBy(t => t.ReservaId!.Value)
            .ToDictionary(g => g.Key, g => g.First().Id);

        var lista = reservas
            .Where(r => !trabajoPorReserva.ContainsKey(r.Id))
            .Select(r => ADto(r))
            .ToList();

        return Ok(lista);
    }

    // GET api/Reservas/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ReservaDTO>> Get(int id)
    {
        var reserva = await repositorio.ObtenerPorIdAsync(id);
        if (reserva == null) return NotFound($"No existe la reserva con id {id}.");

        return Ok(ADto(reserva));
    }

    // GET api/Reservas/cliente/3
    [HttpGet("cliente/{clienteId:int}")]
    public async Task<ActionResult<List<ReservaDTO>>> GetPorCliente(int clienteId)
    {
        var reservas = await repositorio.ObtenerPorClienteIdAsync(clienteId);
        return Ok(reservas.Select(ADto).ToList());
    }

    // GET api/Reservas/rango?desde=2026-01-01&hasta=2026-01-31
    [HttpGet("rango")]
    public async Task<ActionResult<List<ReservaDTO>>> GetPorRango([FromQuery] DateTime desde, [FromQuery] DateTime hasta)
    {
        if (hasta < desde)
            return BadRequest("La fecha 'hasta' no puede ser anterior a 'desde'.");

        var reservas = await repositorio.ObtenerPorRangoFechasAsync(desde, hasta);
        return Ok(reservas.Select(ADto).ToList());
    }

    // POST api/Reservas
    [HttpPost]
    public async Task<ActionResult<ReservaDTO>> Post([FromBody] CrearReservaDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var cliente = await clienteRepositorio.ObtenerPorIdAsync(dto.ClienteId);
        if (cliente == null)
            return BadRequest($"El cliente con ID {dto.ClienteId} no existe.");

        try
        {
            var estadoInicial = string.IsNullOrWhiteSpace(dto.Estado) ? "Pendiente" : dto.Estado.Trim();
            var debeSumarSaldo = EsEstadoCobrable(estadoInicial) && dto.Monto > 0;

            var reserva = new Reserva
            {
                ClienteId = dto.ClienteId,
                Descripcion = dto.Descripcion?.Trim() ?? string.Empty,
                Fecha = DateTime.Now,
                FechaInicio = dto.FechaInicio,
                Estado = estadoInicial,
                Monto = dto.Monto,
                MontoSumadoAlSaldo = debeSumarSaldo
            };

            var creada = await repositorio.Insert(reserva);

            if (debeSumarSaldo)
            {
                cliente.SaldoPendiente += dto.Monto;
                await clienteRepositorio.Update(cliente);
            }

            var completa = await repositorio.ObtenerPorIdAsync(creada.Id) ?? creada;
            return CreatedAtAction(nameof(Get), new { id = creada.Id }, ADto(completa));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // PUT api/Reservas/5
    [HttpPut("{id:int}")]
    public async Task<ActionResult<ReservaDTO>> Put(int id, [FromBody] ActualizarReservaDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (dto.Id != 0 && id != dto.Id)
            return BadRequest("El id de la URL no coincide con el del cuerpo.");

        var reserva = await repositorio.ObtenerPorIdAsync(id);
        if (reserva == null) return NotFound($"No existe la reserva con id {id}.");

        var clienteNuevo = await clienteRepositorio.ObtenerPorIdAsync(dto.ClienteId);
        if (clienteNuevo == null)
            return BadRequest($"El cliente con ID {dto.ClienteId} no existe.");

        try
        {
            var clienteAnteriorId = reserva.ClienteId;
            var montoAnterior = reserva.Monto;
            var estabaSumado = reserva.MontoSumadoAlSaldo;
            var nuevoEstado = string.IsNullOrWhiteSpace(dto.Estado) ? "Pendiente" : dto.Estado.Trim();
            var debeEstarSumado = EsEstadoCobrable(nuevoEstado) && dto.Monto > 0;

            reserva.ClienteId = dto.ClienteId;
            reserva.Descripcion = dto.Descripcion?.Trim() ?? string.Empty;
            reserva.FechaInicio = dto.FechaInicio;
            reserva.Estado = nuevoEstado;
            reserva.Monto = dto.Monto;
            reserva.MontoSumadoAlSaldo = debeEstarSumado;

            await repositorio.Update(reserva);

            if (clienteAnteriorId == dto.ClienteId)
            {
                var impactoAnterior = estabaSumado ? montoAnterior : 0m;
                var impactoNuevo = debeEstarSumado ? dto.Monto : 0m;
                var diferencia = impactoNuevo - impactoAnterior;
                if (diferencia != 0)
                {
                    clienteNuevo.SaldoPendiente = Math.Max(0m, clienteNuevo.SaldoPendiente + diferencia);
                    await clienteRepositorio.Update(clienteNuevo);
                }
            }
            else
            {
                if (estabaSumado && montoAnterior > 0)
                {
                    var clienteAnterior = await clienteRepositorio.ObtenerPorIdAsync(clienteAnteriorId);
                    if (clienteAnterior != null)
                    {
                        clienteAnterior.SaldoPendiente = Math.Max(0m, clienteAnterior.SaldoPendiente - montoAnterior);
                        await clienteRepositorio.Update(clienteAnterior);
                    }
                }
                if (debeEstarSumado)
                {
                    clienteNuevo.SaldoPendiente += dto.Monto;
                    await clienteRepositorio.Update(clienteNuevo);
                }
            }

            var actualizada = await repositorio.ObtenerPorIdAsync(id) ?? reserva;
            return Ok(ADto(actualizada));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // PATCH api/Reservas/5/estado
    [HttpPatch("{id:int}/estado")]
    public async Task<ActionResult> CambiarEstado(int id, [FromBody] CambiarEstadoDTO dto)
    {
        if (string.IsNullOrWhiteSpace(dto.NuevoEstado))
            return BadRequest("El estado es obligatorio.");

        var reserva = await repositorio.ObtenerPorIdAsync(id);
        if (reserva == null) return NotFound($"No existe la reserva con id {id}.");

        var nuevoEstado = dto.NuevoEstado.Trim();
        var debeEstarSumado = EsEstadoCobrable(nuevoEstado) && reserva.Monto > 0;

        if (debeEstarSumado != reserva.MontoSumadoAlSaldo && reserva.Monto > 0)
        {
            var cliente = await clienteRepositorio.ObtenerPorIdAsync(reserva.ClienteId);
            if (cliente != null)
            {
                if (debeEstarSumado)
                    cliente.SaldoPendiente += reserva.Monto;
                else
                    cliente.SaldoPendiente = Math.Max(0m, cliente.SaldoPendiente - reserva.Monto);

                await clienteRepositorio.Update(cliente);
            }
            reserva.MontoSumadoAlSaldo = debeEstarSumado;
        }

        reserva.Estado = nuevoEstado;
        await repositorio.Update(reserva);

        return NoContent();
    }

    // POST api/Reservas/5/convertir-trabajo
    [HttpPost("{id:int}/convertir-trabajo")]
    public async Task<ActionResult<object>> ConvertirATrabajo(int id, [FromBody] ConvertirReservaATrabajoDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var reserva = await repositorio.ObtenerPorIdAsync(id);
        if (reserva == null)
            return NotFound($"No existe la reserva con id {id}.");

        var trabajos = await trabajoRepositorio.Select();
        if (trabajos.Any(t => t.ReservaId == id))
            return BadRequest("Esta reserva ya tiene un trabajo asociado.");

        var cliente = await clienteRepositorio.ObtenerPorIdAsync(reserva.ClienteId);
        if (cliente == null)
            return BadRequest("No se encontró el cliente de la reserva.");

        var nuevoTrabajo = new Trabajo
        {
            NombreObra = string.IsNullOrWhiteSpace(dto.NombreObra)
                ? (string.IsNullOrWhiteSpace(reserva.Descripcion) ? $"Obra Reserva #{reserva.Id}" : reserva.Descripcion)
                : dto.NombreObra.Trim(),
            ClienteId = reserva.ClienteId,
            FechaInicio = dto.FechaInicio,
            FechaFin = DateTime.MinValue,
            Presupuesto = dto.Presupuesto,
            Planos = string.Empty,
            ReservaId = reserva.Id
        };

        var trabajoCreado = await trabajoRepositorio.Insert(nuevoTrabajo);

        if (reserva.MontoSumadoAlSaldo && reserva.Monto > 0)
        {
            cliente.SaldoPendiente = Math.Max(0m, cliente.SaldoPendiente - reserva.Monto);
            reserva.MontoSumadoAlSaldo = false;
            await repositorio.Update(reserva);
        }

        if (dto.Presupuesto > 0)
        {
            cliente.SaldoPendiente += dto.Presupuesto;
            await clienteRepositorio.Update(cliente);
        }
        else if (reserva.Monto > 0)
        {
            await clienteRepositorio.Update(cliente);
        }

        await repositorio.CambiarEstadoAsync(id, "Confirmada");

        return Ok(new
        {
            trabajoId = trabajoCreado.Id,
            mensaje = $"Reserva #{id} confirmada y convertida en trabajo #TRB-{trabajoCreado.Id}."
        });
    }

    // DELETE api/Reservas/5
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        var reserva = await repositorio.ObtenerPorIdAsync(id);
        if (reserva == null)
            return NotFound($"No existe la reserva con id {id}.");

        // Desvincular trabajos que apunten a esta reserva antes de eliminar
        var trabajosVinculados = (await trabajoRepositorio.Select())
            .Where(t => t.ReservaId == id)
            .ToList();
        foreach (var t in trabajosVinculados)
        {
            t.ReservaId = null;
            await trabajoRepositorio.Update(t);
        }

        if (reserva.MontoSumadoAlSaldo && reserva.Monto > 0)
        {
            var cliente = await clienteRepositorio.ObtenerPorIdAsync(reserva.ClienteId);
            if (cliente != null)
            {
                cliente.SaldoPendiente = Math.Max(0m, cliente.SaldoPendiente - reserva.Monto);
                await clienteRepositorio.Update(cliente);
            }
        }

        var eliminado = await repositorio.Delete(id);
        return eliminado ? NoContent() : BadRequest("No se pudo eliminar la reserva.");
    }

    private static bool EsEstadoCobrable(string? estado) =>
        string.Equals(estado?.Trim(), "Completada", StringComparison.OrdinalIgnoreCase);

    // ---------- Mapeo manual Entidad -> DTO ----------
    private static ReservaDTO ADto(Reserva r) => new()
    {
        Id = r.Id,
        ClienteId = r.ClienteId,
        NombreCliente = r.Cliente != null
            ? $"{r.Cliente.Nombre} {r.Cliente.Apellido}".Trim()
            : null,
        TelefonoCliente = r.Cliente?.Telefono,
        Descripcion = r.Descripcion ?? string.Empty,
        Fecha = r.Fecha,
        FechaInicio = r.FechaInicio,
        Estado = r.Estado ?? "Pendiente",
        Monto = r.Monto,
        MontoSumadoAlSaldo = r.MontoSumadoAlSaldo
    };
}
