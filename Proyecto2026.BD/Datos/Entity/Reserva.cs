using System;
using System.Collections.Generic;
using System.Text;

namespace Proyecto2026.BD.Datos.Entity;

public class Reserva : EntityBase
{

    public string Descripcion { get; set; }
    public string Estado { get; set; }
    public DateTime Fecha { get; set; }
    public DateTime FechaInicio { get; set; }
    public int ClienteId { get; set; }
    public Cliente Cliente { get; set; }

}
