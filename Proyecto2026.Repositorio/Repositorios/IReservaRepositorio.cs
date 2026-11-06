using Proyecto2026.BD.Datos.Entity;

namespace Proyecto2026.Repositorio.Repositorios
{
    public interface IReservaRepositorio : IRepositorio<Reserva>
    {
        Task<bool> CambiarEstadoAsync(int reservaId, string nuevoEstado);
        Task<List<Reserva>> ObtenerPorClienteIdAsync(int clienteId);
        Task<List<Reserva>> ObtenerPorRangoFechasAsync(DateTime fechaInicio, DateTime fechaFin);
    }
}