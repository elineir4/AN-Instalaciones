using System.ComponentModel.DataAnnotations;

namespace Proyecto2026.Shared.DTO;

public class MaterialFormDTO
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre del material es obligatorio.")]
    public string NombreMaterial { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0.")]
    public decimal Cantidad { get; set; } = 1;

    [Required(ErrorMessage = "La unidad de medida es obligatoria.")]
    public string UnidadMedida { get; set; } = "unidades";

    [Range(0, double.MaxValue, ErrorMessage = "El precio unitario debe ser mayor o igual a 0.")]
    public decimal PrecioUnitario { get; set; }

    [Required(ErrorMessage = "La fecha de uso es obligatoria.")]
    public DateTime FechaUso { get; set; } = DateTime.Today;

    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un trabajo u obra.")]
    public int TrabajoId { get; set; }
}
