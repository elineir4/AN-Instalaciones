namespace Proyecto2026.Server.Client.Models;

/// <summary>
/// Cuerpo del POST /api/clientes/agregar.
/// Reproduce los nombres reales de Proyecto2026.BD.Datos.Entity.Cliente
/// (Nombre, Apellido, Telefono, Email, SaldoPendiente) con SaldoPendiente = 0 por defecto.
/// </summary>
public class ClienteAlta
{
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public string? Email { get; set; }
    public decimal SaldoPendiente { get; set; } = 0m;
}
