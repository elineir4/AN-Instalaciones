namespace Proyecto2026.Shared.DTO;

public class ReservaDTO
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public string? NombreCliente { get; set; }
    public string? TelefonoCliente { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
    public DateTime FechaInicio { get; set; }
    public string Estado { get; set; } = string.Empty;
    public decimal Monto { get; set; }
    public bool MontoSumadoAlSaldo { get; set; }
    public int? TrabajoId { get; set; }
}
