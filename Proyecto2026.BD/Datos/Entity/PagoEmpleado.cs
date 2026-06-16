using System;
using System.Collections.Generic;
using System.Text;

namespace Proyecto2026.BD.Datos.Entity;

public class PagoEmpleado : EntityBase
{
    public decimal Monto { get; set; }
    public DateTime FechaPago { get; set; }
    public string MedioPago{ get; set; }

    public int EmpleadoId {get; set; }
    public int TrabajoId { get; set; }
    public Empleado Empleado { get; set; }
    public Trabajo Trabajo{ get; set; }



}
