using Microsoft.AspNetCore.Mvc;
using Proyecto2026.BD.Datos.Entity;
using Proyecto2026.Repositorio.Repositorios;
using Proyecto2026.Shared.DTO;

namespace Proyecto2026.Server.Controllers // Ajusta al namespace de tus Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrabajosController : ControllerBase
    {
        private readonly ITrabajoRepositorio _repositorio;
        private readonly IClienteRepositorio _clienteRepositorio;
        private readonly IWebHostEnvironment _env;

        public TrabajosController(
            ITrabajoRepositorio repositorio,
            IClienteRepositorio clienteRepositorio,
            IWebHostEnvironment env)
        {
            _repositorio = repositorio;
            _clienteRepositorio = clienteRepositorio;
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
        public async Task<ActionResult<Trabajo>> Post([FromBody] TrabajoFormDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (string.IsNullOrWhiteSpace(dto.NombreObra))
                return BadRequest("El nombre de la obra es obligatorio.");

            if (dto.FechaFin.HasValue && dto.FechaFin.Value.Date < dto.FechaInicio.Date)
                return BadRequest("La fecha de fin no puede ser anterior a la fecha de inicio.");

            var clienteExiste = await _clienteRepositorio.Existe(dto.ClienteId);
            if (!clienteExiste)
                return BadRequest($"El cliente con ID {dto.ClienteId} no existe.");

            var trabajo = new Trabajo
            {
                NombreObra = dto.NombreObra.Trim(),
                ClienteId = dto.ClienteId,
                FechaInicio = dto.FechaInicio,
                FechaFin = dto.FechaFin ?? DateTime.MinValue,
                Presupuesto = dto.Presupuesto,
                Planos = dto.Planos ?? string.Empty,
                ReservaId = dto.ReservaId
            };

            var creado = await _repositorio.Insert(trabajo);
            var completo = await _repositorio.ObtenerPorIdConRelacionesAsync(creado.Id) ?? creado;
            return CreatedAtAction(nameof(GetById), new { id = creado.Id }, completo);
        }

        // PUT: api/trabajos/5
        [HttpPut("{id:int}")]
        public async Task<ActionResult<Trabajo>> Put(int id, [FromBody] TrabajoFormDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (dto.Id != 0 && id != dto.Id)
                return BadRequest("El ID de la URL no coincide con el ID del trabajo.");

            if (string.IsNullOrWhiteSpace(dto.NombreObra))
                return BadRequest("El nombre de la obra es obligatorio.");

            if (dto.FechaFin.HasValue && dto.FechaFin.Value.Date < dto.FechaInicio.Date)
                return BadRequest("La fecha de fin no puede ser anterior a la fecha de inicio.");

            var trabajoExistente = await _repositorio.ObtenerPorIdAsync(id);
            if (trabajoExistente == null)
                return NotFound($"No se encontró el trabajo con ID {id}.");

            var clienteExiste = await _clienteRepositorio.Existe(dto.ClienteId);
            if (!clienteExiste)
                return BadRequest($"El cliente con ID {dto.ClienteId} no existe.");

            trabajoExistente.NombreObra = dto.NombreObra.Trim();
            trabajoExistente.ClienteId = dto.ClienteId;
            trabajoExistente.FechaInicio = dto.FechaInicio;
            trabajoExistente.FechaFin = dto.FechaFin ?? DateTime.MinValue;
            trabajoExistente.Presupuesto = dto.Presupuesto;
            trabajoExistente.ReservaId = dto.ReservaId;
            if (dto.Planos != null)
            {
                trabajoExistente.Planos = dto.Planos;
            }

            await _repositorio.Update(trabajoExistente);
            var completo = await _repositorio.ObtenerPorIdConRelacionesAsync(id) ?? trabajoExistente;
            return Ok(completo);
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

            var trabajo = await _repositorio.ObtenerPorIdAsync(id);
            if (trabajo == null)
                return NotFound($"No se encontró el trabajo con ID {id}.");

            // Guardar fuera de wwwroot para evitar reinicios de dotnet watch / Hot Reload al subir archivos
            var carpetaPlanos = Path.Combine(_env.ContentRootPath, "App_Data", "planos");
            if (!Directory.Exists(carpetaPlanos))
                Directory.CreateDirectory(carpetaPlanos);

            // Si ya tenía un plano cargado previamente, elimina el archivo anterior
            if (!string.IsNullOrEmpty(trabajo.Planos))
            {
                var rutaAnterior = ResolverRutaPlano(trabajo.Planos);
                if (rutaAnterior != null && System.IO.File.Exists(rutaAnterior))
                    System.IO.File.Delete(rutaAnterior);
            }

            var nombreArchivo = $"trabajo_{id}_{Guid.NewGuid()}{extension}";
            var rutaCompleta = Path.Combine(carpetaPlanos, nombreArchivo);

            using (var stream = new FileStream(rutaCompleta, FileMode.Create))
            {
                await archivo.CopyToAsync(stream);
            }

            trabajo.Planos = nombreArchivo;
            await _repositorio.Update(trabajo);

            return Ok(new
            {
                mensaje = "Plano subido correctamente.",
                ruta = trabajo.Planos
            });
        }

        // GET: api/trabajos/5/plano (Descargar archivo AutoCAD .dwg o .dxf)
        [HttpGet("{id:int}/plano")]
        public async Task<ActionResult> DescargarPlano(int id)
        {
            var trabajo = await _repositorio.ObtenerPorIdAsync(id);
            if (trabajo == null)
                return NotFound($"No se encontró el trabajo con ID {id}.");

            if (string.IsNullOrWhiteSpace(trabajo.Planos))
                return NotFound("Este trabajo no tiene un plano cargado.");

            var rutaCompleta = ResolverRutaPlano(trabajo.Planos);
            if (rutaCompleta == null || !System.IO.File.Exists(rutaCompleta))
                return NotFound("El archivo del plano no se encuentra en el servidor.");

            var extension = Path.GetExtension(rutaCompleta).ToLowerInvariant();
            var nombreLimpio = string.Concat(trabajo.NombreObra.Split(Path.GetInvalidFileNameChars())).Trim();
            if (string.IsNullOrWhiteSpace(nombreLimpio))
                nombreLimpio = $"trabajo_{id}";

            var nombreDescarga = $"{nombreLimpio}{extension}";
            return PhysicalFile(rutaCompleta, "application/octet-stream", nombreDescarga);
        }

        // DELETE: api/trabajos/5
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var trabajo = await _repositorio.ObtenerPorIdAsync(id);
            if (trabajo == null)
                return NotFound($"No se encontró el trabajo con ID {id}.");

            if (!string.IsNullOrEmpty(trabajo.Planos))
            {
                var rutaPlano = ResolverRutaPlano(trabajo.Planos);
                if (rutaPlano != null && System.IO.File.Exists(rutaPlano))
                    System.IO.File.Delete(rutaPlano);
            }

            var eliminado = await _repositorio.Delete(id);
            if (!eliminado)
                return NotFound($"No se encontró el trabajo con ID {id}.");

            return NoContent();
        }

        private string? ResolverRutaPlano(string valorPlanos)
        {
            var nombreArchivo = Path.GetFileName(valorPlanos.TrimStart('/'));
            if (string.IsNullOrWhiteSpace(nombreArchivo))
                return null;

            var rutaAppData = Path.Combine(_env.ContentRootPath, "App_Data", "planos", nombreArchivo);
            if (System.IO.File.Exists(rutaAppData))
                return rutaAppData;

            var webRoot = _env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot");
            var rutaWwwroot = Path.Combine(webRoot, valorPlanos.TrimStart('/'));
            if (System.IO.File.Exists(rutaWwwroot))
                return rutaWwwroot;

            return rutaAppData;
        }
    }
}
