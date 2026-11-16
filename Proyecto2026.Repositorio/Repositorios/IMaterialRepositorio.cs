using Proyecto2026.BD.Datos.Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Proyecto2026.Repositorio.Repositorios
{
    public interface IMaterialRepositorio : IRepositorio<Material>
    {
        Task<List<Material>> ObtenerPorTrabajoIdAsync(int trabajoId);
        Task<decimal> ObtenerCostoTotalPorTrabajoIdAsync(int trabajoId);
        Task<List<Material>> BuscarPorNombreAsync(string nombreMaterial);
        Task<bool> ActualizarPrecioUnitarioAsync(int materialId, decimal nuevoPrecio);
        Task<bool> ActualizarCantidadAsync(int materialId, decimal nuevaCantidad); // O int, según el tipo de Cantidad
        Task<bool> ExisteMaterialEnTrabajoAsync(int trabajoId, string nombreMaterial);
    }
}