using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto2026.BD.Datos;
using Proyecto2026.BD.Datos.Entity;
using Proyecto2026.Shared.DTO;
using Proyecto2026.Repositorio.Repositorios;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Proyecto2026.Server.Controllers;

[ApiController] 
[Route("api/clientes")]
public class ClienteController : ControllerBase
{
    private readonly AppDbContext context; //variable privada para almacenar el contexto de la base de datos
    //esta se deja de usar ya que repositorio ya la adquiere
    private readonly Repositorio<Cliente> repositorio; //variable privada para almacenar el repositorio de clientes

    public ClienteController(AppDbContext context, Repositorio<Cliente> repositorio) //inyectar el contexto de la base de datos en el constructor del controlador
    {
        this.context = context;
        this.repositorio = repositorio;
    } 


    [HttpPost("agregar")]
    public async Task<ActionResult> AgregarCliente(Cliente cliente)
    {
        var nuevocliente = await repositorio.Insert(cliente);
        return Ok(nuevocliente);
    }

    [HttpDelete("eliminar")]
    public async Task<ActionResult> EliminarCliente(int id) 
    {
        var borrar = await repositorio.Delete(id);
         return Ok(borrar);

    }



    [HttpGet]
    public async Task<ActionResult<List<Cliente>>> Get() //método para obtener todos los clientes
    {
        //var lista = await context.Clientes.ToListAsync(); //obtener todos los clientes de la base de datos
          
        var lista = await repositorio.Select(); //obtener todos los clientes de la base de datos usando el repositorio
        if (lista == null)
        {
            return NotFound("No se encontro ningun elemento en la lista");
        }
        else
        {
            return Ok(lista);
        }
    }

    [HttpGet("listaclientes")]
    public async Task<ActionResult<List<ClienteDTO>>> listaClientes()
    {
      var lista = await context.Clientes.Select(p=> new ClienteDTO
      {
         id = p.Id,
         NombreCliente = $"{p.Nombre}"

      }).ToListAsync();

        if (lista == null)
        {
            return NotFound("No se encontro ningun elemento en la lista");

        }
        else 
        {
            return Ok(lista);
        }


        
    }
}
