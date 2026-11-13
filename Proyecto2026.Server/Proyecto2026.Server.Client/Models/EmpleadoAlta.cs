namespace Proyecto2026.Server.Client.Models;

/// <summary>
/// Cuerpo del POST /api/empleados/agregar.
/// Reproduce las propiedades de Proyecto2026.BD.Datos.Entity.Empleado.
/// </summary>
public class EmpleadoAlta
{
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public int Salario { get; set; } = 0;
    public DateTime FechaIngreso { get; set; } = DateTime.Today;
}
