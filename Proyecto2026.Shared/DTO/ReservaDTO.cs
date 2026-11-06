using System;

namespace Proyecto2026.Shared.DTO
{
    public class ReservaDTO
    {
        public string Descripcion { get; set; } = string.Empty;
        public string Estados { get; set; }
        public DateTime Fecha { get; set; }
        public DateTime FechaInicio { get; set; }
    }
}