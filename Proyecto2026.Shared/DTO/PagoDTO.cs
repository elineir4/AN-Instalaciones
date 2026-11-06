namespace Proyecto2026.Shared.DTO
{
    public class PagoDTO
    {
        public int PagoId { get; set; }
        public int ClienteId { get; set; }
        public decimal MontoAbonado { get; set; }
        public DateTime FechaPago { get; set; } = DateTime.Now;
        public string MedioPago { get; set; } = "Efectivo";
    }
}