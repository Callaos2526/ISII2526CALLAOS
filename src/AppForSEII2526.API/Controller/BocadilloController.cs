using AppForSEII2526.API.DTOs.PedidoBocaDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AppForSEII2526.API
{       
      
    [Route("api/[controller]")]
    [ApiController]
    public class BocadilloController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<BocadilloController> _logger;

        public BocadilloController(ApplicationDbContext context, 
            ILogger<BocadilloController> logger)
        {
            _context = context;
            _logger = logger;
        }

        //Listar bocadillos (con filtros por tamaño y tipo pan
        [HttpGet] //Accion que responde a peticiones HTTP GET
        [Route("[action]")]
        //si todo va bien devolvemos una lista SelectBocadilloDTO
        [ProducesResponseType(typeof(IList<SelectBocadilloDTO>), (int)HttpStatusCode.OK)]
        //metodo que devuelve un ActionResult 
        public async Task<ActionResult> GetBocadilloParaPedir(string? filtroTamano, string? filtroTipoPan, float? filtromin, float? filtromax) //tipo Pan bien que sea String 
        {
            Tamaño? tamanoFiltrado = null;

            if (!string.IsNullOrWhiteSpace(filtroTamano) &&
                Enum.TryParse<Tamaño>(filtroTamano, ignoreCase: true, out var parsed))
            {
                tamanoFiltrado = parsed;
            }

            var query = _context.Bocadillos
            .AsNoTracking()
            .Include(b => b.tipopan)
            .Include(b => b.ComprasBocadillo).ThenInclude(cb => cb.Compra)
            .AsQueryable();

            if (filtromin.HasValue)
                query = query.Where(b => b.Pvp >= filtromin.Value);  //filtro por float
            if (filtromax.HasValue)
                query = query.Where(b => b.Pvp <= filtromax.Value);  //filtro por float

            if (tamanoFiltrado.HasValue)
                query = query.Where(b => b.Tamano == tamanoFiltrado.Value);  //filtro por string

            if (!string.IsNullOrWhiteSpace(filtroTipoPan))
                query = query.Where(b => b.tipopan.Nombre == filtroTipoPan);

            var bocadillos = await query
                .OrderBy(b => b.Nombre)
                    .Select(b => new SelectBocadilloDTO(b.Id, b.Nombre, b.Tamano, b.tipopan.Nombre, b.Pvp))
                    .ToListAsync();

            if (!bocadillos.Any())
                return NotFound("No hay bocadillos que cumplan los requisitos");

            return Ok(bocadillos);

        }


    }
}
