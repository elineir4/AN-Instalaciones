namespace Proyecto2026.Shared.DTO
{
    public class PagoEmpleadoDTO
    {
        public int PagoEmpleadoId { get; set; }
        public int EmpleadoId { get; set; }
        public int? TrabajoId { get; set; }
        public decimal Monto { get; set; }
        public DateTime FechaPago { get; set; } = DateTime.Now;
        public string MedioPago { get; set; } = string.Empty;
    }
}