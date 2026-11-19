using Microsoft.AspNetCore.Mvc;
using Proyecto2026.BD.Datos.Entity;
using Proyecto2026.Repositorio.Repositorios;
using Proyecto2026.Shared.DTO;

namespace Proyecto2026.Server.Controllers 
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaterialesController : ControllerBase
    {
        private readonly IMaterialRepositorio _repositorio;
        private readonly ITrabajoRepositorio _trabajoRepositorio;

        public MaterialesController(
            IMaterialRepositorio repositorio,
            ITrabajoRepositorio trabajoRepositorio)
        {
            _repositorio = repositorio;
            _trabajoRepositorio = trabajoRepositorio;
        }

        // GET: api/materiales
        [HttpGet]
        public async Task<ActionResult<List<Material>>> GetAll()
        {
            var materiales = await _repositorio.ObtenerTodosConRelacionesAsync();
            return Ok(materiales);
        }

        // GET: api/materiales/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Material>> GetById(int id)
        {
            var material = await _repositorio.ObtenerPorIdConRelacionesAsync(id);
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
        public async Task<ActionResult<Material>> Post([FromBody] MaterialFormDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (string.IsNullOrWhiteSpace(dto.NombreMaterial))
                return BadRequest("El nombre del material es obligatorio.");

            var trabajoExiste = await _trabajoRepositorio.Existe(dto.TrabajoId);
            if (!trabajoExiste)
                return BadRequest($"El trabajo con ID {dto.TrabajoId} no existe.");

            var material = new Material
            {
                NombreMaterial = dto.NombreMaterial.Trim(),
                Cantidad = dto.Cantidad,
                UnidadMedida = string.IsNullOrWhiteSpace(dto.UnidadMedida) ? "unidades" : dto.UnidadMedida.Trim(),
                PrecioUnitario = dto.PrecioUnitario,
                CostoTotal = dto.Cantidad * dto.PrecioUnitario,
                FechaUso = dto.FechaUso,
                TrabajoId = dto.TrabajoId
            };

            var creado = await _repositorio.Insert(material);
            var completo = await _repositorio.ObtenerPorIdConRelacionesAsync(creado.Id) ?? creado;
            return CreatedAtAction(nameof(GetById), new { id = creado.Id }, completo);
        }

        // PUT: api/materiales/5
        [HttpPut("{id:int}")]
        public async Task<ActionResult<Material>> Put(int id, [FromBody] MaterialFormDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (dto.Id != 0 && id != dto.Id)
                return BadRequest("El ID de la URL no coincide con el ID del material.");

            if (string.IsNullOrWhiteSpace(dto.NombreMaterial))
                return BadRequest("El nombre del material es obligatorio.");

            var materialExistente = await _repositorio.ObtenerPorIdAsync(id);
            if (materialExistente == null)
                return NotFound($"No se encontró el material con ID {id}.");

            var trabajoExiste = await _trabajoRepositorio.Existe(dto.TrabajoId);
            if (!trabajoExiste)
                return BadRequest($"El trabajo con ID {dto.TrabajoId} no existe.");

            materialExistente.NombreMaterial = dto.NombreMaterial.Trim();
            materialExistente.Cantidad = dto.Cantidad;
            materialExistente.UnidadMedida = string.IsNullOrWhiteSpace(dto.UnidadMedida) ? "unidades" : dto.UnidadMedida.Trim();
            materialExistente.PrecioUnitario = dto.PrecioUnitario;
            materialExistente.CostoTotal = dto.Cantidad * dto.PrecioUnitario;
            materialExistente.FechaUso = dto.FechaUso;
            materialExistente.TrabajoId = dto.TrabajoId;

            await _repositorio.Update(materialExistente);
            var completo = await _repositorio.ObtenerPorIdConRelacionesAsync(id) ?? materialExistente;
            return Ok(completo);
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
