using AppForSEII2526.API.DTOs.ComprarMerch;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AppForSEII2526.API.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompraProductoController : ControllerBase
    {
        private readonly ApplicationDbContext _context; 
        private readonly ILogger<CompraProductoController> _logger;
        public CompraProductoController(ApplicationDbContext context, ILogger<CompraProductoController> logger)
        {
            _context = context;
            _logger = logger;
        }
        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(IList<MerchDTO>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> GetMerchParaCompra(string? filtroTipo, int? filtroPrecio)
        {
            IList<MerchDTO> productos = await _context.Producto
                .Include(tp => tp.TipoProducto)
                .Include(tp => tp.producto_Compras).ThenInclude(c => c.compra)
                .Where(producto => (filtroTipo == null || producto.TipoProducto.Nombre.Equals(filtroTipo)) &&
                                (filtroPrecio == null || producto.PVP >= filtroPrecio))
                .OrderBy(producto => producto.Nombre)
                .Select(p => new MerchDTO(p.Productoid, p.Nombre, p.PVP, p.Stock, p.TipoProducto.ToString()))
                .ToListAsync();
            if (productos == null || !productos.Any())
            {
                return NotFound("No hay productos que cumplan los requisitos");
            }
            return Ok(productos);
        }
    }
}
