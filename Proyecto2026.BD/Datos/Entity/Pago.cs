using System;
using System.Collections.Generic;
using System.Text;

namespace Proyecto2026.BD.Datos.Entity;

public class Pago: EntityBase
{
    public decimal MontoAbonado { get; set; }
    public DateTime FechaPago { get; set; }
    public string MedioPago { get; set; }
    public int? TrabajoId { get; set; }
    public Trabajo Trabajo { get; set; }
    public int ClienteId { get; set; }
    public Cliente Cliente{ get; set; }
}
