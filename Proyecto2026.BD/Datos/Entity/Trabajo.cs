using System;
using System.Collections.Generic;
using System.Text;

namespace Proyecto2026.BD.Datos.Entity;

public class Trabajo : EntityBase
{
    public required string NombreObra { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin {  get; set; }
    public decimal Presupuesto { get; set; }
    public string Planos { get; set; }
    public int? ReservaId { get; set; }
    public Reserva? Reserva { get; set; }
    public int ClienteId { get; set; }
    public Cliente Cliente{ get; set; }

}
