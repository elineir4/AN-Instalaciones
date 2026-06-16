using System;
using System.Collections.Generic;
using System.Text;

namespace Proyecto2026.BD.Datos.Entity;

public class Cliente : EntityBase
{
    public required string Nombre { get; set; }
    public required string Apellido { get; set; }
    public int Telefono { get; set; }
    public string? Email { get; set; }
    public decimal SaldoPendiente { get; set; }
}
