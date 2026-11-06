using Microsoft.EntityFrameworkCore;
using Proyecto2026.BD.Datos;
using Proyecto2026.BD.Datos.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Proyecto2026.Repositorio.Repositorios;

public class ReservaRepositorio : Repositorio<Reserva>, IReservaRepositorio
{
    private readonly AppDbContext context;

    public ReservaRepositorio(AppDbContext context) : base(context)
    {
        this.context = context;
    }

    // Obtener todas las reservas de un cliente en específico
    public async Task<List<Reserva>> ObtenerPorClienteIdAsync(int clienteId)
    {
        return await context.Set<Reserva>()
            .Include(r => r.Cliente)
            .Where(r => r.ClienteId == clienteId)
            .OrderByDescending(r => r.FechaInicio)
            .ToListAsync();
    }

    // Obtener reservas entre un rango de fechas (útil para el calendario/agenda)
    public async Task<List<Reserva>> ObtenerPorRangoFechasAsync(DateTime fechaInicio, DateTime fechaFin)
    {
        return await context.Set<Reserva>()
            .Include(r => r.Cliente)
            .Where(r => r.FechaInicio >= fechaInicio && r.FechaInicio <= fechaFin)
            .OrderBy(r => r.FechaInicio)
            .ToListAsync();
    }

    // Actualizar solo el estado de la reserva (ej: "Confirmada", "Cancelada", "Completada")
    public async Task<bool> CambiarEstadoAsync(int reservaId, string nuevoEstado)
    {
        var reserva = await context.Set<Reserva>().FindAsync(reservaId);
        if (reserva == null) return false;

        reserva.Estado = nuevoEstado;
        return await context.SaveChangesAsync() > 0;
    }
}