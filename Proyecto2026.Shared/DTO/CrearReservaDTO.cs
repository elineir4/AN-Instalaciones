using System.ComponentModel.DataAnnotations;

namespace Proyecto2026.Shared.DTO;

public class CrearReservaDTO
{
    [Range(1, int.MaxValue, ErrorMessage = "El cliente es obligatorio.")]
    public int ClienteId { get; set; }

    [Required(ErrorMessage = "La descripción o motivo es obligatoria.")]
    public string Descripcion { get; set; } = string.Empty;

    [Required(ErrorMessage = "La fecha de inicio es obligatoria.")]
    public DateTime FechaInicio { get; set; } = DateTime.Now;

    public string Estado { get; set; } = "Pendiente";

    [Range(0, double.MaxValue, ErrorMessage = "El monto debe ser mayor o igual a 0.")]
    public decimal Monto { get; set; }
}
