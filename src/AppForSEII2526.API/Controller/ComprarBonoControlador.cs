using AppForSEII2526.API.DTOs.CompraBonoDTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Net;

namespace LosDelEspacio.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComprarBonoControlador : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ComprarBonoControlador> _logger;

        public ComprarBonoControlador(ApplicationDbContext context, ILogger<ComprarBonoControlador> logger)
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
             .Select(comprabono => new CompraBonoDetallesDTO(comprabono.CompraBonoId, comprabono.Cliente,comprabono.MetodoPago, comprabono.FechaCompraBono, comprabono.PrecioTotalBono,
                                                             comprabono.bonosComprados.Select(cb => new BonoItemForCreateDTO(cb.Bono.BonoId, cb.Cantidad, cb.Bono.Nombre, cb.Bono.PVP, cb.Bono.NBocadillos,
                                                             cb.Bono.TipoBocadillos.NombreTipo))
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
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult> CrearCompra(CrearCompraDTO crearCompra)
        {
            // Validaciones iniciales
            if (crearCompra == null)
            {
                ModelState.AddModelError("CrearCompra", "Error! El cuerpo de la petición no puede ser vacío");
                return BadRequest(new ValidationProblemDetails(ModelState));
            }

            if (crearCompra.BonoItem == null || crearCompra.BonoItem.Count == 0)
            {
                ModelState.AddModelError("Bonos", "Error! Debes seleccionar algún bono");
            }

            if (string.IsNullOrWhiteSpace(crearCompra.NombreCliente))
                ModelState.AddModelError("NombreCliente", "Error! El nombre es obligatorio");

            if (string.IsNullOrWhiteSpace(crearCompra.ApellidoCliente1) || string.IsNullOrWhiteSpace(crearCompra.ApellidoCliente2))
                ModelState.AddModelError("Apellidos", "Error! Los apellidos son obligatorios");

            // Buscar cliente existente por nombre y apellidos; si no existe, se creará uno nuevo
            ApplicationUser cliente = null;
            if (!ModelState.ContainsKey("NombreCliente") && !ModelState.ContainsKey("Apellidos"))
            {
                cliente = _context.ApplicationUsers
                    .FirstOrDefault(u => u.NombreCliente == crearCompra.NombreCliente
                                      && u.ApellidoCliente1 == crearCompra.ApellidoCliente1
                                      && u.ApellidoCliente2 == crearCompra.ApellidoCliente2);
            }

            // Buscar método de pago por nombre (priorizamos el nombre en el DTO)
            AppForSEII2526.API.Models.MetodoPago metodoPago = null;
            if (!string.IsNullOrWhiteSpace(crearCompra.MetodoPagoName))
            {
                metodoPago = _context.Set<AppForSEII2526.API.Models.MetodoPago>()
                                     .FirstOrDefault(m => m.metodoName == crearCompra.MetodoPagoName);
            }

            if (metodoPago == null)
                ModelState.AddModelError("MetodoPago", "Error! Método de pago no existe o no fue proporcionado correctamente");

            if (ModelState.ErrorCount > 0)
                return BadRequest(new ValidationProblemDetails(ModelState));

            // Si no hay cliente en BD, crear uno nuevo (se guardará junto con la compra)
            if (cliente == null)
            {
                cliente = new ApplicationUser()
                {
                    Id = Guid.NewGuid().ToString(),
                    NombreCliente = crearCompra.NombreCliente,
                    ApellidoCliente1 = crearCompra.ApellidoCliente1,
                    ApellidoCliente2 = crearCompra.ApellidoCliente2,
                    UserName = crearCompra.NombreCliente // valor provisional; ajusta según lógica de usuario real
                };
                _context.ApplicationUsers.Add(cliente);
            }

            // Crear la entidad CompraBono
            var com = new CompraBono(0, cliente, crearCompra.FechaCompra, metodoPago, 0, 0.0, new List<BonosComprados>());

            BonoBocadillo bonoEntity;
            foreach (var item in crearCompra.BonoItem)
            {
                // Validar campos del item
                if (item == null)
                {
                    ModelState.AddModelError("Bonos", "Error! Item de bono inválido");
                    continue;
                }

                // Cargar bono con su tipo
                bonoEntity = await _context.BonosBocadillos
                    .Include(b => b.TipoBocadillos)
                    .FirstOrDefaultAsync(b => b.BonoId == item.BonoId);

                if (bonoEntity == null)
                {
                    ModelState.AddModelError("Bonos", $"Error, el bono '{item?.BonoId}' no existe en nuestra tienda");
                    continue;
                }

                if (item.Cantidad <= 0)
                {
                    ModelState.AddModelError("Bonos", $"Error, la cantidad solicitada para '{bonoEntity.Nombre}' debe ser mayor que 0");
                    continue;
                }

                if (bonoEntity.CantidadDisponible < item.Cantidad)
                {
                    ModelState.AddModelError("Bonos", $"Error, no hay suficiente stock del bono '{bonoEntity.Nombre}'. Disponibles: {bonoEntity.CantidadDisponible}, solicitados: {item.Cantidad}");
                    continue;
                }

                // Actualizamos stock y añadimos el item a la compra
                bonoEntity.CantidadDisponible -= item.Cantidad;

                var precioUnidad = bonoEntity.PVP;

                var bonosComprados = new BonosComprados(0, bonoEntity.BonoId, item.Cantidad, 0, precioUnidad, bonoEntity, com);
                com.bonosComprados.Add(bonosComprados);
            }

            // Si hubo errores al procesar items, devolvemos BadRequest
            if (ModelState.ErrorCount > 0)
                return BadRequest(new ValidationProblemDetails(ModelState));

            // Actualizamos totales
            com.NBonos = com.bonosComprados.Sum(bc => bc.Cantidad);
            com.PrecioTotalBono = com.bonosComprados.Sum(bc => bc.Cantidad * bc.PrecioBono);

            _context.ComprasBono.Add(com);

            try
            {
                // Guardamos compra, cliente nuevo (si se creó) y actualización de stock
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error guardando la compra");
                ModelState.AddModelError("Compra", "Error! Ocurrió un error al guardar la compra, inténtalo más tarde");
                return Conflict("Error: " + ex.Message);
            }

            // Construimos DTO de respuesta con los datos finales
            var compraDetalles = new CompraBonoDetallesDTO(
                com.CompraBonoId,
                com.Cliente,
                com.MetodoPago,
                com.FechaCompraBono,
                com.PrecioTotalBono,
                com.bonosComprados.Select(bc => new BonoItemForCreateDTO(
                    bc.Bono.BonoId,
                    bc.Cantidad,
                    bc.Bono.Nombre,
                    bc.PrecioBono,
                    bc.Bono.NBocadillos,
                    bc.Bono.TipoBocadillos.NombreTipo
                )).ToList()
            );

            return CreatedAtAction("GetCompra", new { id = com.CompraBonoId }, compraDetalles);
        }
    }
}