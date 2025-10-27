using AppForSEII2526.API.DTOs.CompraBonoDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace AppForSEII2526.API.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class BonoControlador : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<BonoControlador> _logger;

        public BonoControlador(ApplicationDbContext context, ILogger<BonoControlador> logger)
        {
            _context = context;
            _logger = logger;
        }
        //hacer get para mostrar bonos disponibles
        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(IList<SelectBonoDTO>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> GetBonoParaCompra(string? filtroNombre, string? tipoBocadillo)
        {
            IList<SelectBonoDTO> bonos = await _context.BonosBocadillos
                .Include(b => b.TipoBocadillos)
                .Include(b => b.bonosComprados).ThenInclude(lc => lc.Compra)
                .Where(bono => (filtroNombre == null || bono.Nombre.Contains(filtroNombre)) &&
                                 (tipoBocadillo == null || bono.TipoBocadillos.ToString() == tipoBocadillo))
                .OrderBy(bono => bono.Nombre)
                .Select(b => new SelectBonoDTO(b.BonoId, b.Nombre, b.PVP, b.NBocadillos, b.TipoBocadillos.ToString()))
                .ToListAsync();
            if (bonos == null || !bonos.Any())
            {
                return NotFound("No hay bonos que cumplan los requisitos");
            }
            return Ok(bonos);
        }

    }
}
