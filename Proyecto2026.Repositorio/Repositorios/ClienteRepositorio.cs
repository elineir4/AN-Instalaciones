using Microsoft.EntityFrameworkCore;
using Proyecto2026.BD.Datos;
using Proyecto2026.BD.Datos.Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Proyecto2026.Repositorio.Repositorios;

public class ClienteRepositorio : Repositorio<Cliente>, IClienteRepositorio
{
    private readonly AppDbContext context;

    public ClienteRepositorio(AppDbContext context) : base(context)
    {
        this.context = context;
    }

    // Trae la lista de entidades Cliente ordenada por apellido y nombre
    public async Task<List<Cliente>> ObtenerListaClientesAsync()
    {
        return await context.Set<Cliente>()
            .OrderBy(c => c.Apellido)
            .ThenBy(c => c.Nombre)
            .ToListAsync();
    }

    // Trae un cliente por ID
    public async Task<Cliente?> ObtenerPorIdAsync(int id)
    {
        return await context.Set<Cliente>()
            .FirstOrDefaultAsync(c => c.Id == id);
    }
}