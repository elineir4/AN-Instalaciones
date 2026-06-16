using System;
using System.Collections.Generic;
using System.Text;

namespace Proyecto2026.BD.Datos.Entity;

public class Material : EntityBase
{
    public string NombreMaterial { get; set; }
    public decimal Cantidad { get; set; }
    public string UnidadMedida { get; set; }
    public decimal PrecioUnitario { get; set; }

    public decimal CostoTotal { get; set; }
    public DateTime FechaUso { get; set; }

    public int TrabajoId { get; set; }
    public Trabajo Trabajo { get; set; }
}

