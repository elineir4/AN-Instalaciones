using System.ComponentModel.DataAnnotations;

namespace Proyecto2026.Shared.DTO;

public class ConvertirReservaATrabajoDTO
{
    [Required(ErrorMessage = "El nombre de la obra es obligatorio.")]
    public string NombreObra { get; set; } = string.Empty;

    [Range(0, double.MaxValue, ErrorMessage = "El presupuesto debe ser mayor o igual a 0.")]
    public decimal Presupuesto { get; set; }

    [Required(ErrorMessage = "La fecha de inicio es obligatoria.")]
    public DateTime FechaInicio { get; set; } = DateTime.Today;
}
