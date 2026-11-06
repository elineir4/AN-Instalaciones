using Microsoft.EntityFrameworkCore;
using Proyecto2026.BD.Datos;
using Proyecto2026.BD.Datos.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Proyecto2026.Repositorio.Repositorios;

public class TrabajoRepositorio : Repositorio<Trabajo>, ITrabajoRepositorio
{
    private readonly AppDbContext context;

    public TrabajoRepositorio(AppDbContext context) : base(context)
    {
        this.context = context;
    }

    // Trae todas las obras/trabajos incluyendo el Cliente y la Reserva
    public async Task<List<Trabajo>> ObtenerTodosConRelacionesAsync()
    {
        return await context.Set<Trabajo>()
            .Include(t => t.Cliente)
            .Include(t => t.Reserva)
            .OrderByDescending(t => t.FechaInicio)
            .ToListAsync();
    }

    // Trae un trabajo por ID con su Cliente y Reserva
    public async Task<Trabajo?> ObtenerPorIdConRelacionesAsync(int id)
    {
        return await context.Set<Trabajo>()
            .Include(t => t.Cliente)
            .Include(t => t.Reserva)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    // Obtener todos los trabajos de un cliente específico
    public async Task<List<Trabajo>> ObtenerPorClienteIdAsync(int clienteId)
    {
        return await context.Set<Trabajo>()
            .Include(t => t.Cliente)
            .Include(t => t.Reserva)
            .Where(t => t.ClienteId == clienteId)
            .OrderByDescending(t => t.FechaInicio)
            .ToListAsync();
    }

    // Trae únicamente los trabajos finalizados (aquellos con FechaFin asignada)
    public async Task<List<Trabajo>> ObtenerTrabajosFinalizadosAsync()
    {
        return await context.Set<Trabajo>()
            .Include(t => t.Cliente)
            .Include(t => t.Reserva)
            .Where(t => t.FechaFin != null)
            .OrderByDescending(t => t.FechaFin)
            .ToListAsync();
    }
}