using AppForSEII2526.API.DTOs.ComprarMerch;
using AppForSEII2526.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Net;

namespace AppForSEII2526.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompraMerchController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CompraMerchController> _logger;

        public CompraMerchController(ApplicationDbContext context, ILogger<CompraMerchController> logger)
        {
            _context = context;
            _logger = logger;
        }

        //GET Details: muestra los datos detallados de una compra
        [HttpGet]  
        [Route("[action]")]
        [ProducesResponseType(typeof(ComprarMerchDetailDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<ComprarMerchDetailDTO>> GetCompraDetail(int id)
        {
            if (_context.Compra_Producto == null)
            {
                _logger.LogError("Error: No hay compras disponibles");
                return NotFound();
            }
            //Buscamos la commpra con ese id            
            var compradto = await _context.Compra_Producto
                .Where(c => c.Id == id)
                .Include(c => c.ListaCompra)
                    .ThenInclude(pc => pc.Producto)
                        .ThenInclude(p => p.TipoProducto)
                .Select(compra => new ComprarMerchDetailDTO(
                compra.CompraId,
                compra.Id,
                compra.Cliente,
                compra.DireccionEnvio,
                compra.Metodo_Pago.metodoName,
                compra.ListaCompra.Sum(pc => pc.Cantidad),
                compra.ListaCompra.Select(pc => new ComprarMerchItemDTO(
                    pc.Productoid,
                    pc.Producto.NombreProducto,
                    pc.PVP,
                    pc.Producto.TipoProducto.NombreProducto,
                    pc.Cantidad
            )).ToList<ComprarMerchItemDTO>())).FirstOrDefaultAsync();
            if (compradto == null)
            {
                _logger.LogError($"Error: Compra con id {id} no existe");
                return NotFound();
            }
            return Ok(compradto);
        }

        // POST: create y itemdto : envia datos al servidor para crear un nuevo elemento
        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType(typeof(ComprarMerchDetailDTO), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult> CrearCompraMerch(ComprarMerchCreateDTO compraMerch)
        {
            // Paso 1: comprobaciones: paso 5 del caso de uso 
            if (compraMerch.MerchItems == null || compraMerch.MerchItems.Count == 0)
            {
                ModelState.AddModelError("MerchItems", "¡Error! Debes seleccionar al menos un producto de merchandising");
                return BadRequest(new ValidationProblemDetails(ModelState));
            }
            if (string.IsNullOrWhiteSpace(compraMerch.Nombre))
                ModelState.AddModelError("Nombre", "El nombre es obligatorio");
            if (string.IsNullOrWhiteSpace(compraMerch.Apellido_1))
                ModelState.AddModelError("Apellido_1", "El primer apellido es obligatorio");
            if (string.IsNullOrWhiteSpace(compraMerch.Direccion_Envio))
                ModelState.AddModelError("Direccion_Envio", "La dirección de envío es obligatoria");
            

            // Validar cantidad en cada producto seleccionado
            for (int i = 0; i < compraMerch.MerchItems.Count; i++)
            {
                var item = compraMerch.MerchItems[i];
                if (item.Cantidad <= 0)
                    ModelState.AddModelError($"MerchItems[{i}].Cantidad", "La cantidad debe ser mayor que 0.");
            }

            // Si hay errores, devolvemos la respuesta antes de continuar
            if (ModelState.ErrorCount > 0)
                return BadRequest(new ValidationProblemDetails(ModelState));
            // Validar existencia y stock de productos
            var productosIds = compraMerch.MerchItems.Select(m => m.Id).ToList();
            var productosBD = await _context.Producto
                .Include(p => p.TipoProducto)
                .Where(p => productosIds.Contains(p.Productoid))
                .ToListAsync();
            
            double precioFinal = 0;
            List<Producto_Compra> lineasCompra = new();
            foreach (var item in compraMerch.MerchItems)
            {
                var producto = productosBD.FirstOrDefault(p => p.Productoid == item.Id);
                if (producto == null)
                {
                    ModelState.AddModelError("MerchItems", $"El producto con id {item.Id} no existe.");
                    continue;
                }
                if (producto.Stock < item.Cantidad)
                {
                    ModelState.AddModelError("MerchItems", $"No hay suficiente stock para {producto.NombreProducto}.");
                }
                precioFinal += producto.PVP * item.Cantidad;

                lineasCompra.Add(new Producto_Compra
                {
                    Productoid = producto.Productoid,
                    Cantidad = item.Cantidad,
                    PVP = (int)producto.PVP,
                    Producto = producto
                });
            }
            if (ModelState.ErrorCount > 0)
                return BadRequest(new ValidationProblemDetails(ModelState));
            // 🔹 Buscar método de pago
            var metodoPago = await _context.MetodoPago
                .FirstOrDefaultAsync(m => m.metodoName.ToLower() == compraMerch.Metodo_Pago.ToLower());

            if (metodoPago == null)
            {
                ModelState.AddModelError("Metodo_Pago", $"El método de pago '{compraMerch.Metodo_Pago}' no existe.");
                return BadRequest(new ValidationProblemDetails(ModelState));
            }

            var cliente = new ApplicationUser
            {
                NombreCliente = compraMerch.Nombre,
                ApellidoCliente1 = compraMerch.Apellido_1,
                ApellidoCliente2 = compraMerch.Apellido_2 ?? string.Empty
            };

            var compra_entity = new Compra_Producto
            {
                Cliente = cliente,
                DireccionEnvio = compraMerch.Direccion_Envio,
                Metodo_Pago = metodoPago,
                FechaCompra = DateTime.Now,
                PrecioFinal = (int)precioFinal,
                ListaCompra = lineasCompra
            };

            _context.Add(compra_entity);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return Conflict("Error: " + ex.Message);
            }

            var itemsDTO = productosBD.Select(p => new ComprarMerchItemDTO(
                p.Productoid,
                p.NombreProducto,
                p.PVP,
                p.TipoProducto.NombreProducto,
                lineasCompra.First(lc => lc.Productoid == p.Productoid).Cantidad
            )).ToList();

            var compraDetailDTO = new ComprarMerchDetailDTO(
                compra_entity.Id,
                compra_entity.CompraId,
                cliente,
                compra_entity.DireccionEnvio,
                metodoPago.metodoName,
                lineasCompra.Sum(x => x.Cantidad),
                itemsDTO
            );

            return CreatedAtAction("GetCompraDetail", new { id = compra_entity.Id }, compraDetailDTO);

        }
    }
}
