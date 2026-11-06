using System;
using System.Collections.Generic;
using System.Text;


namespace Proyecto2026.Shared.DTO
{
    public class EmpleadoDTO
    {
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string? Telefono { get; set; }
        public DateTime FechaIngreso { get; set; }
    }
}