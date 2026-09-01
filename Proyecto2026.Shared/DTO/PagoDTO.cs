using System;
using System.Collections.Generic;
using System.Text;

namespace Proyecto2026.Shared.DTO;

public class PagoDTO
{
    public int id { get; set; }
    public int clienteId { get; set; }
    public string nombreCliente { get; set; }
    public int saldoPendiente { get; set; } 
    public int montoAbonado { get; set; }

    public DateTime fechaPago { get; set; }

    public string ? medioPago { get; set; } 
}
