using System;
using System.Collections.Generic;
using System.Text;

namespace Proyecto2026.BD.Datos.Entity;

public class Empleado : EntityBase
{
    public string Nombre { get; set; }
    public string Apellido { get; set; }
    public string? Telefono { get; set; }

    public int Salario { get; set; } 
    public DateTime FechaIngreso { get; set; }


}
