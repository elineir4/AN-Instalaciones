using Proyecto2026.BD.Datos.Entity;

namespace Proyecto2026.Repositorio.Repositorios
{
    public interface IEmpleadoRepositorio : IRepositorio<Empleado>
    {
        Task<List<Empleado>> ObtenerListaEmpleadosAsync();
        Task<Empleado?> ObtenerPorIdAsync(int id);
    }
}