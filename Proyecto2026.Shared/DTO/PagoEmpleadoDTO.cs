namespace Proyecto2026.Server.Controllers
{
    public class PagoEmpleadoDTO
    {
        public int empleadoId { get; set; }
        public decimal salario { get; set; }
        public DateTime fechaPago { get; set; }
        public decimal montoAbonado { get; set; }
        public string medioPago { get; set; }


    }
}
