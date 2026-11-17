
using Proyecto2026.BD.Datos.Entity;
using Proyecto2026.Repositorio.Repositorios;

public interface ITrabajoRepositorio : IRepositorio<Trabajo>
{
    Task<List<Trabajo>> ObtenerTodosConRelacionesAsync();
    Task<Trabajo?> ObtenerPorIdConRelacionesAsync(int id);
    Task<List<Trabajo>> ObtenerPorClienteIdAsync(int clienteId);
    Task<List<Trabajo>> ObtenerTrabajosFinalizadosAsync();
    Task<Trabajo?> ObtenerPorIdAsync(int id);  
}