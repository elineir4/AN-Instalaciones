using Proyecto2026.BD.Datos.Entity;

namespace Proyecto2026.Repositorio.Repositorios
{
    public interface IPagoEmpleadoRepositorio : IRepositorio<PagoEmpleado>
    {
        Task<Empleado> ObtenerEmpleadoPorId(int empleadoId);
        Task<List<Empleado>> ObtenerEmpleadosConPagosPendientes();
        Task<List<PagoEmpleado>> ObtenerPagosPorEmpleado(int empleadoId);
        Task<PagoEmpleado> RegistrarPagoEmpleado(PagoEmpleado pagoEmpleado);
    }
}