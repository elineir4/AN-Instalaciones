using System;

namespace Proyecto2026.Shared.DTO
{
    public class MaterialDTO
    {
        public int TrabajoId { get; set; }
        public int? ClienteId { get; set; }
        public string NombreMaterial { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public string UnidadMedida { get; set; } = string.Empty;
        public decimal PrecioUnitario { get; set; }
        public DateTime FechaUso { get; set; }
    }
}