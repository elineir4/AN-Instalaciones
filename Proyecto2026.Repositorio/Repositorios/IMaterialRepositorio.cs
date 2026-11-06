using Proyecto2026.BD.Datos.Entity;

namespace Proyecto2026.Repositorio.Repositorios
{
    public interface IMaterialRepositorio : IRepositorio<Material>
    {
        Task<bool> ActualizarPrecioUnitarioAsync(int materialId, decimal nuevoPrecio);
        Task<List<Material>> BuscarPorNombreAsync(string nombreMaterial);
        Task<decimal> ObtenerCostoTotalPorTrabajoIdAsync(int trabajoId);
        Task<List<Material>> ObtenerPorTrabajoIdAsync(int trabajoId);
    }
}