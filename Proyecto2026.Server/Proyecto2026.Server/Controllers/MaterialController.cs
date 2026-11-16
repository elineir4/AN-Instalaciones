using Microsoft.AspNetCore.Mvc;
using Proyecto2026.BD.Datos.Entity;
using Proyecto2026.Repositorio.Repositorios;

namespace Proyecto2026.Server.Controllers // Ajusta este namespace al de tu carpeta Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaterialesController : ControllerBase
    {
        private readonly IMaterialRepositorio _repositorio;

        public MaterialesController(IMaterialRepositorio repositorio)
        {
            _repositorio = repositorio;
        }

        // GET: api/materiales
        [HttpGet]
        public async Task<ActionResult<List<Material>>> GetAll()
        {
            var materiales = await _repositorio.Select();
            return Ok(materiales);
        }

        // GET: api/materiales/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Material>> GetById(int id)
        {
            var materiales = await _repositorio.Select();
            var material = materiales.FirstOrDefault(m => m.Id == id);

            if (material == null)
                return NotFound($"No se encontró el material con ID {id}.");

            return Ok(material);
        }

        // GET: api/materiales/trabajo/3
        [HttpGet("trabajo/{trabajoId:int}")]
        public async Task<ActionResult<List<Material>>> GetPorTrabajo(int trabajoId)
        {
            var materiales = await _repositorio.ObtenerPorTrabajoIdAsync(trabajoId);
            return Ok(materiales);
        }

        // GET: api/materiales/trabajo/3/costo-total
        [HttpGet("trabajo/{trabajoId:int}/costo-total")]
        public async Task<ActionResult<decimal>> GetCostoTotalPorTrabajo(int trabajoId)
        {
            var costoTotal = await _repositorio.ObtenerCostoTotalPorTrabajoIdAsync(trabajoId);
            return Ok(costoTotal);
        }

        // GET: api/materiales/buscar?nombre=Caño
        [HttpGet("buscar")]
        public async Task<ActionResult<List<Material>>> BuscarPorNombre([FromQuery] string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                return BadRequest("Debe ingresar un nombre o palabra clave para buscar.");

            var resultados = await _repositorio.BuscarPorNombreAsync(nombre);
            return Ok(resultados);
        }

        // POST: api/materiales
        [HttpPost]
        public async Task<ActionResult<Material>> Post([FromBody] Material material)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var creado = await _repositorio.Insert(material);
            return CreatedAtAction(nameof(GetById), new { id = creado.Id }, creado);
        }

        // PUT: api/materiales/5
        [HttpPut("{id:int}")]
        public async Task<ActionResult<Material>> Put(int id, [FromBody] Material material)
        {
            if (id != material.Id)
                return BadRequest("El ID de la URL no coincide con el ID del material.");

            var existe = await _repositorio.Existe(id);
            if (!existe)
                return NotFound($"No se encontró el material con ID {id}.");

            var actualizado = await _repositorio.Update(material);
            return Ok(actualizado);
        }

        // PATCH: api/materiales/5/precio
        [HttpPatch("{id:int}/precio")]
        public async Task<ActionResult> ActualizarPrecio(int id, [FromBody] decimal nuevoPrecio)
        {
            if (nuevoPrecio < 0)
                return BadRequest("El precio unitario no puede ser negativo.");

            var actualizado = await _repositorio.ActualizarPrecioUnitarioAsync(id, nuevoPrecio);
            if (!actualizado)
                return NotFound($"No se encontró el material con ID {id}.");

            return NoContent();
        }

        // DELETE: api/materiales/5
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var eliminado = await _repositorio.Delete(id);
            if (!eliminado)
                return NotFound($"No se encontró el material con ID {id}.");

            return NoContent();
        }
    }
}