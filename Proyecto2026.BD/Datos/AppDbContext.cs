using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Proyecto2026.BD.Datos;

public class AppDbContext : DbContext //representa una sesion de una base de datos en sql server
{
   public DbSet<Entity.Cliente> Clientes { get; set; }
    public DbSet<Entity.Trabajo> Trabajos { get; set; }

    public DbSet<Entity.Material> Materiales { get; set; }
    public DbSet<Entity.Empleado>Empleados { get; set; }

    public DbSet<Entity.TrabajoEmpleado>TrabajosEmpleados { get; set; }

    public DbSet<Entity.PagoEmpleado> PagosEmpleados { get; set; }

    public DbSet<Entity.Pago> Pagos {  get; set; }

    public DbSet<Entity.Reserva> Reservas { get; set; }





    public AppDbContext(DbContextOptions<AppDbContext> options)
    : base(options)
    {

    }


}
