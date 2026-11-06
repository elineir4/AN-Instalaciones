using Microsoft.EntityFrameworkCore;
using Proyecto2026.BD.Datos;
using Proyecto2026.BD.Datos.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Proyecto2026.Repositorio.Repositorios;

public class MaterialRepositorio : Repositorio<Material>, IMaterialRepositorio
{
    private readonly AppDbContext context;

    public MaterialRepositorio(AppDbContext context) : base(context)
    {
        this.context = context;
    }

    // Retorna la lista de materiales pertenecientes a un trabajo específico
    public async Task<List<Material>> ObtenerPorTrabajoIdAsync(int trabajoId)
    {
        return await context.Set<Material>()
            .Where(m => m.TrabajoId == trabajoId)
            .OrderBy(m => m.NombreMaterial)
            .ToListAsync();
    }

    // Calcula y devuelve la suma total ($) de dinero gastado en materiales para ese trabajo
    public async Task<decimal> ObtenerCostoTotalPorTrabajoIdAsync(int trabajoId)
    {
        return await context.Set<Material>()
            .Where(m => m.TrabajoId == trabajoId)
            .SumAsync(m => m.Cantidad * m.PrecioUnitario);
    }

    // Búsqueda de un insumo o material por palabra clave (ej: "Caño", "Bomba", "Cable")
    public async Task<List<Material>> BuscarPorNombreAsync(string nombreMaterial)
    {
        return await context.Set<Material>()
            .Where(m => m.NombreMaterial.Contains(nombreMaterial))
            .ToListAsync();
    }

    public async Task<bool> ActualizarPrecioUnitarioAsync(int materialId, decimal nuevoPrecio)
    {
        var material = await context.Set<Material>().FindAsync(materialId);
        if (material == null) return false;

        material.PrecioUnitario = nuevoPrecio;
        return await context.SaveChangesAsync() > 0;
    }
}