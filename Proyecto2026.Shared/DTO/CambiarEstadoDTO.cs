using System.ComponentModel.DataAnnotations;

namespace Proyecto2026.Shared.DTO;

public class CambiarEstadoDTO
{
    [Required(ErrorMessage = "El estado es obligatorio.")]
    public string NuevoEstado { get; set; } = string.Empty;
}