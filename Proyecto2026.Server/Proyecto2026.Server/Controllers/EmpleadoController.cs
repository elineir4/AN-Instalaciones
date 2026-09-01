using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Proyecto2026.BD.Datos;
using Proyecto2026.BD.Datos.Entity;
using Proyecto2026.Repositorio.Repositorios;
using Microsoft.EntityFrameworkCore;

namespace Proyecto2026.Server.Controllers;

[ApiController]
[Route("api/empleados")]

public class EmpleadoController : ControllerBase
{

    private readonly Repositorio<Empleado> repositorio;

    public EmpleadoController(Repositorio<Empleado> repositorio)
    {
        this.repositorio = repositorio;
    }


    [HttpGet("lista")]
    public async Task<ActionResult<List<Empleado>>> ListaEmpleados()
    {
        var lista = await repositorio.Select();
        if (lista == null)
        {
            return NotFound("No se encontro ningun elemento en la lista");
        }
        else
        {
            return Ok(lista);
        }
    }

    [HttpGet("empleado/{id}")]
    public async Task<ActionResult<Empleado>> ObtenerEmpleado(int id)
    {
        var empleado = await repositorio.ObtenerPorId(id);
        if (empleado == null)
        {
            return NotFound("No se encontro el empleado con ese ID");
        }
        else
        {
            return Ok(empleado);
        }
    }

    [HttpPost("agregar")]
    public async Task<ActionResult<Empleado>> AgregarEmpleado(Empleado empleado)
    {
        var nuevoempleado = await repositorio.Insert(empleado);
        return Ok(nuevoempleado);

    }


    [HttpDelete("eliminar/{id}")]
    public async Task<ActionResult<bool>> EliminarEmpleado(int id)
    {
        var eliminado = await repositorio.Delete(id);
        if (!eliminado)
        {
            return NotFound("No se encontro el empleado con ese ID");
        }
        else
        {
            return Ok(true);
        }
    }

}