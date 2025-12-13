using AppForSEII2526.API.DTOs.ComprarMerchDTOs;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace AppForSEII2526.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MerchandaisingController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<MerchandaisingController> _logger;

        public MerchandaisingController(
            ApplicationDbContext context,
            ILogger<MerchandaisingController> logger)
        {
            _context = context;
            _logger = logger;

            _logger.LogInformation("MerchandaisingController initialized");
        }

        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(IList<ComprarMerchandaisingDTO>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> GetMerchParaCompra(string? filtroTipo, int? filtroPrecio)
        {
            try
            {
                _logger.LogInformation(
                    "GetMerchParaCompra called with filtroTipo={FiltroTipo}, filtroPrecio={FiltroPrecio}",
                    filtroTipo, filtroPrecio);

                IList<ComprarMerchandaisingDTO> productos = await _context.Producto
                    .Where(producto =>
                        (filtroTipo == null || producto.TipoProducto.NombreProducto.Contains(filtroTipo)) &&
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

                //Lo uso para simular un error y que aparezca al susbcribirme a log.error
                if (filtroTipo == "boom")
                {
                    throw new Exception("Error de prueba");
                }
                if (productos == null || !productos.Any())
                {
                    _logger.LogWarning("GetMerchParaCompra found no products");
                    return NotFound("No hay productos que cumplan los requisitos");
                }
                


                _logger.LogInformation("GetMerchParaCompra returned {Count} products", productos.Count);
                return Ok(productos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetMerchParaCompra");
                return StatusCode((int)HttpStatusCode.InternalServerError, "Error interno del servidor");
            }
        }
    }
}
