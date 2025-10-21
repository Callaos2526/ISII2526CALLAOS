using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppForSEII2526.API.Models;
using AppForSEII2526.API.DTOs.ResenyaDTOs;
using static AppForSEII2526.API.Models.Resenya;

namespace AppForSEII2526.API.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class Controlador : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<Controlador> _logger;

        public Controlador(ApplicationDbContext context, ILogger<Controlador> logger)
        {
            _context = context;
            _logger = logger;
        }

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
                    
                    ValoracionGeneral.Tres,
                    r.ResenyaBocadillo
                        .Select(rb => new ResenyaItemDTO(
                            rb.BocadilloId,
                            rb.Bocadillo.Nombre,
                            rb.Bocadillo.Pvp,
                            rb.Bocadillo.Tamano,
                            rb.Bocadillo.tipopan != null ? rb.Bocadillo.tipopan.Nombre : null,
                            rb.Puntuacion
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

        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType(typeof(ResenyaDetailDTO), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult> CreateResenya(ResenyaForCreateDTO resenyaForCreate)
        {
            
            if (resenyaForCreate.ResenyaBocadillo == null || resenyaForCreate.ResenyaBocadillo.Count == 0)
                ModelState.AddModelError("ResenyaBocadillo", "Error! Debes incluir al menos un bocadillo con su puntuación (1..10)");

            if (resenyaForCreate.ResenyaBocadillo != null &&
                resenyaForCreate.ResenyaBocadillo.Any(i => i.Puntuacion < 1 || i.Puntuacion > 10))
                ModelState.AddModelError("Puntuacion", "Error! La puntuación de cada bocadillo debe estar entre 1 y 10");

            if (resenyaForCreate.ResenyaBocadillo != null)
            {
                var ids = resenyaForCreate.ResenyaBocadillo.Select(i => i.BocadilloId).ToList();
                if (ids.Distinct().Count() != ids.Count)
                    ModelState.AddModelError("ResenyaBocadillo", "Error! No se permiten bocadillos duplicados en la reseña");
            }

            if (ModelState.ErrorCount > 0)
                return BadRequest(new ValidationProblemDetails(ModelState));

            
            var idsSolicitados = resenyaForCreate.ResenyaBocadillo.Select(i => i.BocadilloId).ToList();

            var bocadillos = await _context.Bocadillos
                .Include(b => b.tipopan)
                .Where(b => idsSolicitados.Contains(b.Id))
                .Select(b => new
                {
                    b.Id,
                    b.Nombre,
                    b.Pvp,
                    b.Tamano,
                    TipoPanNombre = b.tipopan != null ? b.tipopan.Nombre : null
                })
                .ToListAsync();

            if (bocadillos.Count != idsSolicitados.Count)
            {
                ModelState.AddModelError("ResenyaBocadillo", "Error! Alguno de los bocadillos no existe");
                return BadRequest(new ValidationProblemDetails(ModelState));
            }

            // === Construimos entidad Resenya (EF asignará Id) ===
            var resenya = new Resenya(
                id: 0,
                descripcion: resenyaForCreate.Descripcion,
                fechaPublicacion: DateTime.Now,
                nombreUsuario: resenyaForCreate.NombreUsuario,
                titulo: resenyaForCreate.Titulo
            )
            {
                ResenyaBocadillo = new List<ResenyaBocadillo>()
            };

            foreach (var item in resenyaForCreate.ResenyaBocadillo)
            {
                resenya.ResenyaBocadillo.Add(new ResenyaBocadillo(
                    id: 0,
                    bocadilloId: item.BocadilloId,
                    puntuacion: item.Puntuacion,
                    resenyaId: 0
                ));
            }

            _context.Add(resenya);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                ModelState.AddModelError("Resenya", "Error! Ocurrió un problema al guardar tu reseña, por favor, inténtalo más tarde");
                return Conflict("Error " + ex.Message);
            }

            var itemsDetalle = resenyaForCreate.ResenyaBocadillo
                .Join(bocadillos,
                      i => i.BocadilloId,
                      b => b.Id,
                      (i, b) => new ResenyaItemDTO(
                          b.Id,
                          b.Nombre,
                          b.Pvp,
                          b.Tamano,
                          b.TipoPanNombre,
                          i.Puntuacion
                      ))
                .ToList();

            var resenyaDetail = new ResenyaDetailDTO(
                id: resenya.Id,
                fechaPublicacion: resenya.FechaPublicacion,
                nombreUsuario: resenya.NombreUsuario,
                titulo: resenya.Titulo,
                descripcion: resenya.Descripcion,
                valoracion: resenyaForCreate.Valoracion,
                resenyaBocadillo: itemsDetalle
            );

            return CreatedAtAction(nameof(GetResenya), new { id = resenya.Id }, resenyaDetail);
        }
    }
}

