using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppForSEII2526.API.Models;
using AppForSEII2526.API.DTOs.ResenyaDTOs;

namespace AppForSEII2526.API.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResenyasControlador : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ResenyasControlador> _logger;

        public ResenyasControlador(ApplicationDbContext context, ILogger<ResenyasControlador> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Paso 7 - GET detalle
        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(ResenyaDetailDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> GetResenya(int id)
        {
            if (_context.Resenyas == null)
            {
                _logger.LogError("Error: Resenyas table does not exist");
                return NotFound();
            }

            var resenya = await _context.Resenyas
                .Where(r => r.Id == id)
                .Include(r => r.ResenyaBocadillo)
                    .ThenInclude(rb => rb.Bocadillo)
                        .ThenInclude(b => b.tipopan)
                .Select(r => new ResenyaDetailDTO(
                    r.Id,
                    r.FechaPublicacion,
                    r.NombreUsuario,
                    r.Titulo,
                    r.Descripcion,
                    r.Valoracion,
                    r.ResenyaBocadillo
                        .Select(rb => new ResenyaItemDTO(
                            rb.BocadilloId,
                            rb.Bocadillo.Nombre,
                            rb.Bocadillo.Pvp,
                            rb.Bocadillo.Tamano,
                            rb.Puntuacion   // <- importante: ahora pasamos la puntuación del item
                        ))
                        .ToList()
                ))
                .FirstOrDefaultAsync();

            if (resenya == null)
            {
                _logger.LogError($"Error: Resenya with id {id} does not exist");
                return NotFound();
            }

            return Ok(resenya);
        }

        // Paso 5 - POST crear
        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType(typeof(ResenyaDetailDTO), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult> CreateResenya(ResenyaForCreateDTO dto)
        {
            if (dto.ResenyaBocadillo == null || dto.ResenyaBocadillo.Count == 0)
                ModelState.AddModelError("ResenyaBocadillo", "Error: Debes incluir al menos un bocadillo con su puntuación (1..10)");

            if (dto.ResenyaBocadillo != null &&
                dto.ResenyaBocadillo.Any(i => i.Puntuacion < 1 || i.Puntuacion > 10))
                ModelState.AddModelError("Puntuacion", "Error: La puntuación de cada bocadillo debe estar entre 1 y 10");

            if (dto.ResenyaBocadillo != null)
            {
                var ids = dto.ResenyaBocadillo.Select(i => i.BocadilloId).ToList();
                if (ids.Distinct().Count() != ids.Count)
                    ModelState.AddModelError("ResenyaBocadillo", "Error: No se permiten bocadillos duplicados en la reseña");
            }

            if (!ModelState.IsValid)
                return BadRequest(new ValidationProblemDetails(ModelState));

            var idsSolicitados = dto.ResenyaBocadillo.Select(i => i.BocadilloId).ToList();

            var bocadillos = await _context.Bocadillos
                .Include(b => b.tipopan)
                .Where(b => idsSolicitados.Contains(b.Id))
                .Select(b => new
                {
                    b.Id,
                    b.Nombre,
                    b.Pvp,
                    b.Tamano
                })
                .ToListAsync();

            if (bocadillos.Count != idsSolicitados.Count)
            {
                ModelState.AddModelError("ResenyaBocadillo", "Error: Alguno de los bocadillos no existe en la base de datos");
                return BadRequest(new ValidationProblemDetails(ModelState));
            }

            // Crear entidad Resenya (tu constructor no pide Valoracion, se asigna después)
            var resenya = new Resenya(
                id: 0,
                descripcion: dto.Descripcion,
                fechaPublicacion: DateTime.Now,
                nombreUsuario: dto.NombreUsuario ?? "Anónimo",
                titulo: dto.Titulo
            )
            {
                ResenyaBocadillo = new List<ResenyaBocadillo>()
            };

            // Asignar la valoración general
            resenya.Valoracion = dto.Valoracion;

            // Crear items de la reseña y enlazar la navegación (EF rellena ResenyaId)
            foreach (var item in dto.ResenyaBocadillo)
            {
                resenya.ResenyaBocadillo.Add(new ResenyaBocadillo
                {
                    BocadilloId = item.BocadilloId,
                    Puntuacion = item.Puntuacion,
                    Resenya = resenya
                });
            }

            _context.Add(resenya);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error guardando la reseña");
                return Conflict("Error " + (ex.InnerException?.Message ?? ex.Message));
            }

            // Construir DTO de salida (detalle)
            var itemsDetalle = dto.ResenyaBocadillo
                .Join(bocadillos,
                      i => i.BocadilloId,
                      b => b.Id,
                      (i, b) => new ResenyaItemDTO(
                          b.Id,
                          b.Nombre,
                          b.Pvp,
                          b.Tamano,
                          i.Puntuacion  // <- importante: viene del item enviado por el cliente
                      ))
                .ToList();

            var resenyaDetail = new ResenyaDetailDTO(
                id: resenya.Id,
                fechaPublicacion: resenya.FechaPublicacion,
                nombreUsuario: resenya.NombreUsuario,
                titulo: resenya.Titulo,
                descripcion: resenya.Descripcion,
                valoracion: resenya.Valoracion,
                resenyaBocadillo: itemsDetalle
            );

            return CreatedAtAction(nameof(GetResenya), new { id = resenya.Id }, resenyaDetail);
        }
    }
}
