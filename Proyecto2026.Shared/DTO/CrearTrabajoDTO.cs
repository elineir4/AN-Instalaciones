
using System.ComponentModel.DataAnnotations;

public class CrearTrabajoDTO
{
    [Required(ErrorMessage = "El cliente es obligatorio.")]
    public int ClienteId { get; set; }

    public int? ReservaId { get; set; }

    [Required(ErrorMessage = "La fecha de inicio es obligatoria.")]
    public DateTime FechaInicio { get; set; }

    public DateTime? FechaFin { get; set; }

    
}