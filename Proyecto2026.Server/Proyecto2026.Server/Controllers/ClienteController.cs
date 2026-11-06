using Microsoft.AspNetCore.Mvc;
using Proyecto2026.BD.Datos.Entity;
using Proyecto2026.Repositorio.Repositorios;
using Proyecto2026.Shared.DTO;

namespace Proyecto2026.Server.Controllers;

[ApiController]
[Route("api/clientes")]
public class ClienteController : ControllerBase
{
    private readonly IClienteRepositorio repositorio;

    public ClienteController(IClienteRepositorio repositorio)
    {
        this.repositorio = repositorio;
    }

    [HttpPost("agregar")]
    public async Task<ActionResult<int>> AgregarCliente([FromBody] Cliente cliente)
    {
        try
        {
            var nuevoId = await repositorio.Insert(cliente);
            return Ok(nuevoId);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al agregar el cliente: {ex.Message}");
        }
    }

    [HttpDelete("eliminar/{id:int}")]
    public async Task<ActionResult> EliminarCliente(int id)
    {
        var eliminado = await repositorio.Delete(id);
        if (!eliminado)
        {
            return NotFound($"No existe el cliente con id {id}");
        }
        return Ok(eliminado);
    }

    [HttpGet("obtenerclientes")]
    public async Task<ActionResult<List<Cliente>>> Get()
    {
        var lista = await repositorio.Select();
        return Ok(lista ?? new List<Cliente>());
    }

    [HttpGet("listaclientes")]
    public async Task<ActionResult<List<ClienteDTO>>> ListaClientes()
    {
        var clientes = await repositorio.ObtenerListaClientesAsync();

        var listaDTO = (clientes ?? Enumerable.Empty<Cliente>())
            .Select(p => new ClienteDTO
            {
                ClienteId = p.Id,
                NombreCliente = $"{p.Nombre} {p.Apellido}".Trim(),
                Apellido = p.Apellido,
                Telefono = p.Telefono,
                Email = p.Email
            }).ToList();

        return Ok(listaDTO);
    }
}