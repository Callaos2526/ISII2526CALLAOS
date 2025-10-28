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
    public class ResenyasControlador : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ResenyasControlador> _logger;

        public ResenyasControlador(ApplicationDbContext context, ILogger<ResenyasControlador> logger)
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
    }
}

