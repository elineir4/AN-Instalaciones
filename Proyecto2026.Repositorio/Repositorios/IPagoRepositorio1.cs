using Proyecto2026.BD.Datos.Entity;

namespace Proyecto2026.Repositorio.Repositorios
{
    public interface IPagoRepositorio1 : IRepositorio<Pago>
    {
        Task<List<Cliente>> ClientesSaldoPendiente();
        Task<Cliente> ObtenerClienteId(int clienteId);
        Task<Pago> RegistrarPago(Pago pago, Cliente cliente);
    }
}