namespace Proyecto2026.Server.Client.Models;

public class MaterialItem
{
    public int Id { get; set; }
    public string NombreMaterial { get; set; } = string.Empty;
    public decimal Cantidad { get; set; }
    public string UnidadMedida { get; set; } = string.Empty;
    public decimal PrecioUnitario { get; set; }
    public decimal CostoTotal { get; set; }
    public DateTime FechaUso { get; set; }
    public int TrabajoId { get; set; }
    public TrabajoItem? Trabajo { get; set; }

    public decimal TotalCalculado => CostoTotal > 0 ? CostoTotal : Cantidad * PrecioUnitario;

    public string NombreObra =>
        Trabajo != null && !string.IsNullOrWhiteSpace(Trabajo.NombreObra)
            ? Trabajo.NombreObra
            : $"Trabajo #{TrabajoId}";

    public string NombreCliente =>
        Trabajo != null
            ? Trabajo.NombreClienteCompleto
            : "—";

    public string CantidadFormateada
    {
        get
        {
            var cant = Cantidad % 1 == 0
                ? ((long)Cantidad).ToString()
                : Cantidad.ToString("0.##");
            return string.IsNullOrWhiteSpace(UnidadMedida) ? cant : $"{cant} {UnidadMedida}";
        }
    }
}
