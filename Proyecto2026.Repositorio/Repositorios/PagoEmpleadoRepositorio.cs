using Proyecto2026.BD.Datos;
using Proyecto2026.BD.Datos.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace Proyecto2026.Repositorio.Repositorios;

public class PagoEmpleadoRepositorio : Repositorio<PagoEmpleado>, IPagoEmpleadoRepositorio
{

    private readonly AppDbContext context;
    public PagoEmpleadoRepositorio(AppDbContext context) : base(context)
    {
        this.context = context;
    }

    public async Task<List<Empleado>> ObtenerEmpleadosConPagosPendientes()
    {
        return await context.Empleados
          .Where(e => e.Salario > 0)
          .OrderByDescending(e => e.Salario)
          .ToListAsync();
    }


    public async Task<Empleado> ObtenerEmpleadoPorId(int empleadoId)
    {
        return await context.Empleados.FirstOrDefaultAsync(e => e.Id == empleadoId);
    }

    public async Task<PagoEmpleado> RegistrarPagoEmpleado(PagoEmpleado pagoEmpleado)
    {
        context.PagosEmpleados.Add(pagoEmpleado);
        await context.SaveChangesAsync();
        return pagoEmpleado;
    }

    public async Task<List<PagoEmpleado>> ObtenerPagosPorEmpleado(int empleadoId)
    {
        return await context.PagosEmpleados
            .Where(p => p.EmpleadoId == empleadoId)
            .ToListAsync();
    }
}
