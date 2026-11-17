namespace Proyecto2026.Server.Client.Models;

public class TrabajoItem
{
    public int Id { get; set; }
    public string NombreObra { get; set; } = string.Empty;
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public decimal Presupuesto { get; set; }
    public string Planos { get; set; } = string.Empty;
    public int? ReservaId { get; set; }
    public int ClienteId { get; set; }
    public TrabajoClienteItem? Cliente { get; set; }

    public bool EstaFinalizado => FechaFin.Year > 1;
    public bool TienePlano => !string.IsNullOrWhiteSpace(Planos);

    public string NombreClienteCompleto =>
        Cliente != null && !string.IsNullOrWhiteSpace(Cliente.NombreCompleto)
            ? Cliente.NombreCompleto
            : $"Cliente #{ClienteId}";

    public string NombreArchivoPlano =>
        string.IsNullOrWhiteSpace(Planos)
            ? string.Empty
            : Path.GetFileName(Planos);

    public string ExtensionPlano =>
        string.IsNullOrWhiteSpace(Planos)
            ? string.Empty
            : Path.GetExtension(Planos).TrimStart('.').ToUpperInvariant();
}

public class TrabajoClienteItem
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public string? Email { get; set; }

    public string NombreCompleto => $"{Nombre} {Apellido}".Trim();
}
