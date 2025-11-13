using AppForSEII2526.API.DTOs.ComprarMerch;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AppForSEII2526.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MerchandaisingController : ControllerBase
    {
        private readonly ApplicationDbContext _context; 
        private readonly ILogger<MerchandaisingController> _logger;
        public MerchandaisingController(ApplicationDbContext context, ILogger<MerchandaisingController> logger)
        {
            _context = context;
            _logger = logger;
        }
        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(IList<ComprarMerchandaisingDTO>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> GetMerchParaCompra(string? filtroTipo, int? filtroPrecio)
        {
            IList<ComprarMerchandaisingDTO> productos = await _context.Producto
            .Where(producto => (filtroTipo == null || producto.TipoProducto.NombreProducto.Contains(filtroTipo)) &&
                               (filtroPrecio == null || producto.PVP <= filtroPrecio) && 
                                producto.Stock > 0)
            .OrderBy(producto => producto.NombreProducto)
            .Select(p => new ComprarMerchandaisingDTO(
                p.Productoid,
                p.NombreProducto,
                p.PVP,
                p.Stock,
                p.TipoProducto.NombreProducto)) 
            .ToListAsync();
            if (productos == null || !productos.Any())
            {
                return NotFound("No hay productos que cumplan los requisitos");
            }
            return Ok(productos);
        }
    }
}
