using Microsoft.AspNetCore.Mvc;
using Proyecto2026.BD.Datos.Entity;
using Proyecto2026.Repositorio.Repositorios;

namespace Proyecto2026.Server.Controllers;

[ApiController]
[Route("api/empleados")]
public class EmpleadoController : ControllerBase
{
    private readonly IEmpleadoRepositorio repositorio;

    public EmpleadoController(IEmpleadoRepositorio repositorio)
    {
        this.repositorio = repositorio;
    }

    [HttpGet("lista")]
    public async Task<ActionResult<List<Empleado>>> ListaEmpleados()
    {
        var lista = await repositorio.Select();
        return Ok(lista ?? new List<Empleado>());
    }

    [HttpGet("empleado/{id:int}")]
    public async Task<ActionResult<Empleado>> ObtenerEmpleado(int id)
    {
        var empleado = await repositorio.ObtenerPorIdAsync(id);
        if (empleado == null)
        {
            return NotFound("No se encontro el empleado con ese ID");
        }
        return Ok(empleado);
    }

    [HttpPost("agregar")]
    public async Task<ActionResult<Empleado>> AgregarEmpleado([FromBody] Empleado empleado)
    {
        try
        {
            var nuevoEmpleado = await repositorio.Insert(empleado);
            return Ok(nuevoEmpleado);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al agregar el empleado: {ex.Message}");
        }
    }

    [HttpDelete("eliminar/{id:int}")]
    public async Task<ActionResult<bool>> EliminarEmpleado(int id)
    {
        var eliminado = await repositorio.Delete(id);
        if (!eliminado)
        {
            return NotFound("No se encontro el empleado con ese ID");
        }
        return Ok(true);
    }
}