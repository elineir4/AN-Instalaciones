using System;
using System.Collections.Generic;
using System.Text;

namespace Proyecto2026.BD.Datos.Entity;

public class TrabajoEmpleado : EntityBase
{
    public int TrabajoId { get; set; }
    public int EmpleadoId { get; set; }
    public Empleado Empleado{ get; set; }
    public Trabajo Trabajo{ get; set; }
    public DateTime FechaTrabajo { get; set; }

}
