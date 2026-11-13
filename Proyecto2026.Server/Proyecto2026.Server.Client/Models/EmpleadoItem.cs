using System.Text.Json.Serialization;

namespace Proyecto2026.Server.Client.Models;

/// <summary>
/// Modelo de empleado para la pantalla de Empleados (GET /api/empleados/lista).
/// </summary>
public class EmpleadoItem
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("nombre")]
    public string Nombre { get; set; } = string.Empty;

    [JsonPropertyName("apellido")]
    public string Apellido { get; set; } = string.Empty;

    [JsonPropertyName("telefono")]
    public string? Telefono { get; set; }

    [JsonPropertyName("salario")]
    public int Salario { get; set; }

    [JsonPropertyName("fechaIngreso")]
    public DateTime FechaIngreso { get; set; }

    [JsonIgnore]
    public string NombreCompleto
    {
        get
        {
            var completo = $"{Nombre} {Apellido}".Trim();
            return string.IsNullOrWhiteSpace(completo) ? "Empleado" : completo;
        }
    }
}
