using System;
using System.Collections.Generic;

namespace Proyecto2026.Shared.DTO
{
    public class TrabajoDTO
    {
        public int ClienteId { get; set; } // Se mantiene para saber a qué cliente pertenece la obra
        public string NombreObra { get; set; } = string.Empty;
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public decimal Presupuesto { get; set; }
        public string Estado { get; set; } = "Pendiente";
    }
}