using Microsoft.EntityFrameworkCore;
using Proyecto2026.BD.Datos;
using Proyecto2026.BD.Datos.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Proyecto2026.Repositorio.Repositorios
{
    public class MaterialRepositorio : Repositorio<Material>, IMaterialRepositorio
    {
        private readonly AppDbContext context;

        public MaterialRepositorio(AppDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<List<Material>> ObtenerTodosConRelacionesAsync()
        {
            return await context.Set<Material>()
                .AsNoTracking()
                .Include(m => m.Trabajo)
                    .ThenInclude(t => t.Cliente)
                .OrderByDescending(m => m.FechaUso)
                .ThenByDescending(m => m.Id)
                .ToListAsync();
        }

        public async Task<Material?> ObtenerPorIdConRelacionesAsync(int id)
        {
            return await context.Set<Material>()
                .AsNoTracking()
                .Include(m => m.Trabajo)
                    .ThenInclude(t => t.Cliente)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<Material?> ObtenerPorIdAsync(int id)
        {
            return await context.Set<Material>()
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        // Retorna la lista de materiales pertenecientes a un trabajo específico
        public async Task<List<Material>> ObtenerPorTrabajoIdAsync(int trabajoId)
        {
            return await context.Set<Material>()
                .AsNoTracking()
                .Where(m => m.TrabajoId == trabajoId)
                .OrderBy(m => m.NombreMaterial)
                .ToListAsync();
        }

        // Calcula y devuelve la suma total ($) de dinero gastado en materiales para ese trabajo
        public async Task<decimal> ObtenerCostoTotalPorTrabajoIdAsync(int trabajoId)
        {
            return await context.Set<Material>()
                .AsNoTracking()
                .Where(m => m.TrabajoId == trabajoId)
                .SumAsync(m => (decimal?)(m.Cantidad * m.PrecioUnitario)) ?? 0m;
        }

        // Búsqueda de un insumo o material por palabra clave (insensible a mayúsculas/minúsculas)
        public async Task<List<Material>> BuscarPorNombreAsync(string nombreMaterial)
        {
            if (string.IsNullOrWhiteSpace(nombreMaterial))
                return new List<Material>();

            var termino = nombreMaterial.Trim().ToLower();

            return await context.Set<Material>()
                .AsNoTracking()
                .Where(m => m.NombreMaterial.ToLower().Contains(termino))
                .OrderBy(m => m.NombreMaterial)
                .ToListAsync();
        }

        // Actualiza únicamente el precio unitario de un material
        public async Task<bool> ActualizarPrecioUnitarioAsync(int materialId, decimal nuevoPrecio)
        {
            if (nuevoPrecio < 0) return false;

            var material = await context.Set<Material>().FindAsync(materialId);
            if (material == null) return false;

            material.PrecioUnitario = nuevoPrecio;
            material.CostoTotal = material.Cantidad * nuevoPrecio;
            return await context.SaveChangesAsync() > 0;
        }

        // Actualiza únicamente la cantidad usada de un material (cambia 'decimal' por 'int' si Cantidad es entero)
        public async Task<bool> ActualizarCantidadAsync(int materialId, decimal nuevaCantidad)
        {
            if (nuevaCantidad < 0) return false;

            var material = await context.Set<Material>().FindAsync(materialId);
            if (material == null) return false;

            material.Cantidad = nuevaCantidad;
            material.CostoTotal = nuevaCantidad * material.PrecioUnitario;
            return await context.SaveChangesAsync() > 0;
        }

        // Verifica si ya existe un material con el mismo nombre cargado en ese trabajo
        public async Task<bool> ExisteMaterialEnTrabajoAsync(int trabajoId, string nombreMaterial)
        {
            if (string.IsNullOrWhiteSpace(nombreMaterial)) return false;

            var nombreNormalizado = nombreMaterial.Trim().ToLower();

            return await context.Set<Material>()
                .AsNoTracking()
                .AnyAsync(m => m.TrabajoId == trabajoId && m.NombreMaterial.ToLower() == nombreNormalizado);
        }
    }
}