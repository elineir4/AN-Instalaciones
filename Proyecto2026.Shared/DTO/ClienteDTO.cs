using System;
using System.Collections.Generic;
using System.Text;

namespace Proyecto2026.Shared.DTO
{
    public class ClienteDTO
    {
        public int ClienteId { get; set; }
        public string NombreCliente { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string? Telefono { get; set; }
        public string? Email { get; set; }
        public string NombreObra { get; set; } = string.Empty;
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public decimal Presupuesto { get; set; }
        public decimal SaldoPendiente { get; set; }
        public string? Planos { get; set; }
    }
}