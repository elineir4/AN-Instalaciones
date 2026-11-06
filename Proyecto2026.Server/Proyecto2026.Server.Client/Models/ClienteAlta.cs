namespace Proyecto2026.Server.Client.Models;

/// <summary>
/// Cuerpo del POST /api/clientes/agregar.
/// Reproduce los nombres reales de Proyecto2026.BD.Datos.Entity.Cliente
/// (Nombre, Apellido, Telefono, Email) sin enviar Id ni SaldoPendiente.
/// El enlace de ASP.NET Core lee el JSON de forma insensible a mayúsculas/minúsculas.
/// </summary>
public class ClienteAlta
{
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public string? Email { get; set; }
}
