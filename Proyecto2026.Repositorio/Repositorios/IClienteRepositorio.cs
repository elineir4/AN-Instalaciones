using Proyecto2026.BD.Datos.Entity;

namespace Proyecto2026.Repositorio.Repositorios
{
    public interface IClienteRepositorio : IRepositorio<Cliente>
    {
        Task<List<Cliente>> ObtenerListaClientesAsync();
        Task<Cliente?> ObtenerPorIdAsync(int id);
    }
}