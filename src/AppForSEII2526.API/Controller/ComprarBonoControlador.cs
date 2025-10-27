using AppForSEII2526.API.DTOs.CompraBonoDTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Net;

namespace LosDelEspacio.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComprasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ComprasController> _logger;

        public ComprasController(ApplicationDbContext context, ILogger<ComprasController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(CompraBonoDetallesDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> GetCompra(int id)
        {
            if (_context.ComprasBono == null)
            {
                _logger.LogError("Error: No hay compras disponibles");
                return NotFound();
            }

            var comprasdto = await _context.ComprasBono
             .Where(comprabono => comprabono.CompraBonoId == id)
                 .Include(compra => compra.bonosComprados)
                    .ThenInclude(bonoItem => bonoItem.Bono)
                        .ThenInclude(bono => bono.TipoBocadillos)
             .Select(comprabono => new CompraBonoDetallesDTO(comprabono.CompraBonoId, comprabono.NombreCliente, comprabono.ApellidoCliente1, comprabono.ApellidoCliente2,
                                                             comprabono.MetodoPago, comprabono.FechaCompraBono, comprabono.PrecioTotalBono, comprabono.bonosComprados.Select(
                                                             cb => new BonoItemDTO(cb.Bono.BonoId, cb.Bono.Nombre, cb.Bono.PVP, cb.Bono.NBocadillos, cb.Bono.TipoBocadillos))
                                                             .ToList())).FirstOrDefaultAsync();


            if (comprasdto == null)
            {
                _logger.LogError($"Error: Compra con {id} no existe");
                return NotFound();
            }

            return Ok(comprasdto);
        }


        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType(typeof(CompraBonoDetallesDTO), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult> CrearCompra(CrearCompraDTO crearCompra)
        {


            if (crearCompra.BonoItem.Count == 0)
            {
                ModelState.AddModelError("bonos", "Error! Debes seleccionar algún libro");
                return BadRequest(new ValidationProblemDetails(ModelState));
            }



            // Corrección para CS7036: Se debe proporcionar todos los argumentos requeridos por el constructor de CompraBono.
            // Además, simplificación de la expresión "new" para IDE0090.

            CompraBono com = new(
                crearCompra.CompraId,
                crearCompra.NombreCliente,
                crearCompra.ApellidoCliente1,
                crearCompra.ApellidoCliente2,
                crearCompra.FechaCompra,
                crearCompra.Metodo,
                crearCompra.BonoItem.Count, // NBonos, asumiendo que corresponde a la cantidad de bonos comprados
                crearCompra.PrecioTotal,    // precioTotalBono
                new List<BonosComprados>()  // bonosComprados, inicializado vacío
            );

            BonoBocadillo bono;
            foreach (var item in crearCompra.BonoItem)
            {
                bono = await _context.BonosBocadillos.FindAsync(item.ID);
                if (bono == null)
                {
                    ModelState.AddModelError("Bonos", $"Error, el bono {item.Nombre}no existe en nuestra tienda");

                }
                else
                {
                    if (bono.NBocadillos < item.NumeroDeBocadillos)
                    {
                        ModelState.AddModelError("Bonos", $"Error, la cantidad de bonos {item.Nombre} no puede ser mayor a 20 y has seleccionado {item.NumeroDeBocadillos}");
                    }
                    else
                    {
                        bono.NBocadillos -= item.NumeroDeBocadillos;
                        com.bonosComprados.Add(new BonosComprados(item.ID, bono.BonoId, item.NumeroDeBocadillos, com.CompraBonoId, com.PrecioTotalBono, bono, com));
                    }
                }

            }

            if (ModelState.ErrorCount > 0)
            {
                return BadRequest(new ValidationProblemDetails(ModelState));
            }
            com.PrecioTotalBono = com.bonosComprados.Sum(pi => pi.Cantidad * pi.Bono.PVP);

            _context.ComprasBono.Add(com);
            await _context.SaveChangesAsync();

            var compraDetalles = new CompraBonoDetallesDTO(com.CompraBonoId, com.NombreCliente, com.ApellidoCliente1, com.ApellidoCliente2, com.MetodoPago
                                                           ,com.FechaCompraBono, com.PrecioTotalBono, crearCompra.BonoItem);

            return CreatedAtAction("GetCompra", new { id = com.CompraBonoId }, compraDetalles);
        }


    }
}