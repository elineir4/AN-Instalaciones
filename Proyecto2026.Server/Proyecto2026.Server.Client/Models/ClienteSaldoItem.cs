using System.Text.Json.Serialization;

namespace Proyecto2026.Server.Client.Models;

/// <summary>
/// Modelo de cliente con saldo pendiente para la pantalla de Pagos y Saldos.
/// Resuelve la compatibilidad entre la entidad de BD y DTOs del backend.
/// </summary>
public class ClienteSaldoItem
{
    // TODO: En el backend actual, PagoController.SaldosPendientes y ObtenerCliente retornan directamente
    // la entidad Cliente de Proyecto2026.BD (con propiedades Id y Nombre) en vez de un DTO de Shared
    // (donde ClienteDTO usa ClienteId y NombreCliente).
    // Este modelo en el frontend mapea tanto 'id'/'clienteId' como 'nombre'/'nombreCliente' de forma transparente.

    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("clienteId")]
    public int ClienteId
    {
        get => Id;
        set { if (Id == 0) Id = value; }
    }

    [JsonPropertyName("nombre")]
    public string Nombre { get; set; } = string.Empty;

    [JsonPropertyName("nombreCliente")]
    public string NombreCliente
    {
        get => Nombre;
        set { if (string.IsNullOrEmpty(Nombre)) Nombre = value; }
    }

    [JsonPropertyName("apellido")]
    public string Apellido { get; set; } = string.Empty;

    [JsonPropertyName("telefono")]
    public string? Telefono { get; set; }

    [JsonPropertyName("email")]
    public string? Email { get; set; }

    [JsonPropertyName("saldoPendiente")]
    public decimal SaldoPendiente { get; set; }

    [JsonIgnore]
    public string NombreCompleto
    {
        get
        {
            var completo = $"{Nombre} {Apellido}".Trim();
            return string.IsNullOrWhiteSpace(completo) ? "Cliente" : completo;
        }
    }
}
