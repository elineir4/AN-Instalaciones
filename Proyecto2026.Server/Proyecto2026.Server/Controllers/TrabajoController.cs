using Microsoft.AspNetCore.Mvc;
using Proyecto2026.BD.Datos.Entity;
using Proyecto2026.Repositorio.Repositorios;

namespace Proyecto2026.Server.Controllers // Ajusta al namespace de tus Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrabajosController : ControllerBase
    {
        private readonly ITrabajoRepositorio _repositorio;
        private readonly IWebHostEnvironment _env;

        public TrabajosController(ITrabajoRepositorio repositorio, IWebHostEnvironment env)
        {
            _repositorio = repositorio;
            _env = env;
        }

        // GET: api/trabajos
        [HttpGet]
        public async Task<ActionResult<List<Trabajo>>> GetAll()
        {
            var trabajos = await _repositorio.ObtenerTodosConRelacionesAsync();
            return Ok(trabajos);
        }

        // GET: api/trabajos/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Trabajo>> GetById(int id)
        {
            var trabajo = await _repositorio.ObtenerPorIdConRelacionesAsync(id);
            if (trabajo == null)
                return NotFound($"No se encontró el trabajo con ID {id}.");

            return Ok(trabajo);
        }

        // GET: api/trabajos/cliente/2
        [HttpGet("cliente/{clienteId:int}")]
        public async Task<ActionResult<List<Trabajo>>> GetPorCliente(int clienteId)
        {
            var trabajos = await _repositorio.ObtenerPorClienteIdAsync(clienteId);
            return Ok(trabajos);
        }

        // GET: api/trabajos/finalizados
        [HttpGet("finalizados")]
        public async Task<ActionResult<List<Trabajo>>> GetFinalizados()
        {
            var trabajos = await _repositorio.ObtenerTrabajosFinalizadosAsync();
            return Ok(trabajos);
        }

        // POST: api/trabajos
        [HttpPost]
        public async Task<ActionResult<Trabajo>> Post([FromBody] Trabajo trabajo)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var creado = await _repositorio.Insert(trabajo);
            return CreatedAtAction(nameof(GetById), new { id = creado.Id }, creado);
        }

        // PUT: api/trabajos/5
        [HttpPut("{id:int}")]
        public async Task<ActionResult<Trabajo>> Put(int id, [FromBody] Trabajo trabajo)
        {
            if (id != trabajo.Id)
                return BadRequest("El ID de la URL no coincide con el ID del trabajo.");

            var existe = await _repositorio.Existe(id);
            if (!existe)
                return NotFound($"No se encontró el trabajo con ID {id}.");

            var actualizado = await _repositorio.Update(trabajo);
            return Ok(actualizado);
        }

        // POST: api/trabajos/5/plano (Subir archivo AutoCAD .dwg o .dxf)
        [HttpPost("{id:int}/plano")]
        [RequestSizeLimit(100_000_000)] // Permite planos de hasta 100 MB
        public async Task<ActionResult> SubirPlano(int id, IFormFile archivo)
        {
            if (archivo == null || archivo.Length == 0)
                return BadRequest("Debe seleccionar un archivo de plano.");

            var extension = Path.GetExtension(archivo.FileName).ToLowerInvariant();
            if (extension != ".dwg" && extension != ".dxf")
                return BadRequest("Formato no válido. Solo se permiten archivos de AutoCAD (.dwg o .dxf).");

            var trabajos = await _repositorio.Select();
            var trabajo = trabajos.FirstOrDefault(t => t.Id == id);
            if (trabajo == null)
                return NotFound($"No se encontró el trabajo con ID {id}.");

            var carpetaPlanos = Path.Combine(_env.WebRootPath, "planos");
            if (!Directory.Exists(carpetaPlanos))
                Directory.CreateDirectory(carpetaPlanos);

            // Si ya tenía un plano cargado previamente, elimina el archivo anterior
            if (!string.IsNullOrEmpty(trabajo.Planos))
            {
                var rutaAnterior = Path.Combine(_env.WebRootPath, trabajo.Planos.TrimStart('/'));
                if (System.IO.File.Exists(rutaAnterior))
                    System.IO.File.Delete(rutaAnterior);
            }

            var nombreArchivo = $"trabajo_{id}_{Guid.NewGuid()}{extension}";
            var rutaCompleta = Path.Combine(carpetaPlanos, nombreArchivo);

            using (var stream = new FileStream(rutaCompleta, FileMode.Create))
            {
                await archivo.CopyToAsync(stream);
            }

            trabajo.Planos = $"/planos/{nombreArchivo}";
            await _repositorio.Update(trabajo);

            return Ok(new
            {
                mensaje = "Plano subido correctamente.",
                ruta = trabajo.Planos
            });
        }

        // DELETE: api/trabajos/5
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            // Opcional: borrar también el archivo físico del plano si existe antes de eliminar el registro
            var trabajos = await _repositorio.Select();
            var trabajo = trabajos.FirstOrDefault(t => t.Id == id);
            if (trabajo != null && !string.IsNullOrEmpty(trabajo.Planos))
            {
                var rutaPlano = Path.Combine(_env.WebRootPath, trabajo.Planos.TrimStart('/'));
                if (System.IO.File.Exists(rutaPlano))
                    System.IO.File.Delete(rutaPlano);
            }

            var eliminado = await _repositorio.Delete(id);
            if (!eliminado)
                return NotFound($"No se encontró el trabajo con ID {id}.");

            return NoContent();
        }
    }
}