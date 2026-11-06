using Microsoft.EntityFrameworkCore;
using Proyecto2026.BD.Datos;
using Proyecto2026.BD.Datos.Entity;

namespace Proyecto2026.Repositorio.Repositorios;

public class EmpleadoRepositorio : Repositorio<Empleado>, IEmpleadoRepositorio
{
    private readonly AppDbContext context;

    public EmpleadoRepositorio(AppDbContext context) : base(context)
    {
        this.context = context;
    }

    // Trae los empleados ordenados por apellido y nombre
    public async Task<List<Empleado>> ObtenerListaEmpleadosAsync()
    {
        return await context.Set<Empleado>()
            .OrderBy(e => e.Apellido)
            .ThenBy(e => e.Nombre)
            .ToListAsync();
    }

    // Trae un empleado por ID
    public async Task<Empleado?> ObtenerPorIdAsync(int id)
    {
        return await context.Set<Empleado>()
            .FirstOrDefaultAsync(e => e.Id == id);
    }


}