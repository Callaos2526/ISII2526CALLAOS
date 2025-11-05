
using AppForSEII2526.API.DTOs.PedidoBocaDTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.CodeModifier.CodeChange;
using System.Collections.Generic;
using System.Net;


namespace AppForSEII2526.API.Controllers
{   //ESTE ES EL DE LA COMPRA

    [Route("api/[controller]")]
    [ApiController]
    public class PedidoController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PedidoController> _logger;

        public PedidoController(ApplicationDbContext context, ILogger<PedidoController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet] //ESTE ES EL GET DETAILS: muestra los datos detallados de un elemento concreto
        [Route("[action]")]
        [ProducesResponseType(typeof(DetailsPedidoDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<DetailsPedidoDTO>> GetPedido(int id)
        {
            //comprobar que la tabla existe en el contexto
            if (_context.Compras == null)
            {
                _logger.LogError("Error: No hay compras disponibles");
                return NotFound();
            }
            //buscar compra con ese id, incluyendo las lineas y proyectarla 
            //directamente a DetailsPedidoDTO
            var comprasdto = await _context.Compras //nombre del DBCONTEXT
             .Where(compra => compra.CompraId == id)
                
                 .Include(compra => compra.BocadillosComprados) //relacion intermedia

                    .ThenInclude(bocadilloItem => bocadilloItem.Bocadillo) //relacion al bocadillo
                        .ThenInclude(Bocadillo => Bocadillo.tipopan)    //relacion al tipo de pan

             .Select(compra => new DetailsPedidoDTO(
                 compra.CompraId,
                 compra.FechaCompra,
                 compra.ApplicationUser.NombreCliente, //esto antes tenia lo de los string
                 compra.ApplicationUser.ApellidoCliente1,
                 compra.ApplicationUser.ApellidoCliente2,
                 compra.metodoPago.metodoName,
                 compra.BocadillosComprados.Select(
                     cb => new ItemPedidoDTO( //parametros del constructor 
                         cb.BocadilloId,
                         cb.Bocadillo.Nombre,
                         cb.Bocadillo.tipopan.Nombre, //porque es string
                         cb.Cantidad,
                         cb.Bocadillo.Pvp
                     )).ToList())).FirstOrDefaultAsync();





            if (comprasdto == null)
            {
                _logger.LogError($"Error: Compra con {id} no existe");
                return NotFound();
            }

            return Ok(comprasdto);
        }
        

        [HttpPost] //create y itemdto : envia datos al servidor para crear un nuevo elemento
        [Route("[action]")]
        [ProducesResponseType(typeof(DetailsPedidoDTO), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult> CrearPedido(CreatePedidoDTO crearPedido)
        //objeto que recibe (DTO)
        {

            //PASO 1. comprobaciones: paso 5 del caso de uso *******************************

            if (crearPedido.BocadilloItem == null || crearPedido.BocadilloItem.Count == 0) //si no ha pedido al menos 1 bocadillo no puedes crear el pedido
            {
                ModelState.AddModelError("Bocadillo", "Error! Debes seleccionar algun bocadillo");
                return BadRequest(new ValidationProblemDetails(ModelState));
            }
            else
            { //Paso 5. Se solicita la cantidad a comprar de cada bocadillo, siendo este campo obligatorio.
                for (int i = 0; i < crearPedido.BocadilloItem.Count; i++)
                {
                    var item = crearPedido.BocadilloItem[i];
                    if (item.Cantidad <= 0)
                    {
                        ModelState.AddModelError($"BocadilloItem[{i}].Cantidad", "La cantidad es obligatoria y debe ser mayor que 0.");
                    }
                }
            }
            //Obligatorio nombre Usuario, Apellido1, metodo pago
            var user = _context.ApplicationUsers.FirstOrDefault(n => n.UserName == crearPedido.NombreCliente);
            if (user == null)
                ModelState.AddModelError("ApplicationUser", "Error! Nombre no registrado");

            var apellido = _context.ApplicationUsers.FirstOrDefault(ap => ap.ApellidoCliente1 == crearPedido.ApellidoCliente1);
            if (apellido == null)
                ModelState.AddModelError("ApplicationUser", "Error! Apellido no registrado");

            //comprobacion del metodo de pago=> preguntar a noelia si esta bien
            var metodoName = crearPedido.Metodo; //saco el nombre del metodo de pago que hayan introducido
            
            var existe_metodo = await _context.Paypals.AnyAsync(p => p.metodoName == metodoName)
                || await _context.GooglePays.AnyAsync(g => g.metodoName == metodoName)
                || await _context.Tarjetas.AnyAsync(t => t.metodoName == metodoName);

            if (!existe_metodo) {
                ModelState.AddModelError("Metodo", "Error! Método de pago no registrado.");
                return BadRequest(new ValidationProblemDetails(ModelState));
            }
            if (ModelState.ErrorCount > 0)
            {
                return BadRequest(new ValidationProblemDetails(ModelState));
            }
            

            //PASO 2. vamos recuperando objetos y rellenando el pedido 

            //sacar nombre de bocadillos que quiere el cliente
            var bocadilloNombres = crearPedido.BocadilloItem.Select(bi => bi.ID).ToList();
            //cargamos bocadillos desde la BD con la info que necesitamos 
            var bocadillosBD = _context.Bocadillos
                .Where(bi => bocadilloNombres.Contains(bi.Id))
                .Select(b => new
                {
                    b.Id,
                    b.Nombre,
                    b.Pvp,
                    b.Stock
                }).ToList();

            //ahora creo mi pedido (compra) <= ya tengo los datos del bocadillo 
            var metodoPago = await _context.MetodoPago
                .FirstOrDefaultAsync(m => m.metodoName.ToLower() == crearPedido.Metodo.ToLower());
            if(metodoPago == null)
            {
                ModelState.AddModelError("Metodo_Pago", $"El método de pago '{crearPedido.Metodo}' no existe.");
                return BadRequest(new ValidationProblemDetails(ModelState));
            }
            //PASO3 . CREAR LA COMPRA en memoria y rellenarla 

            Compra compra = new Compra
            { //algunos paramtetros los saco del DTO que recibo
                metodoPago = metodoPago,
                FechaCompra = DateTime.Now, //aqui pongo la fecha en el momento 
                ApplicationUser = user,
                BocadillosComprados = new List<CompraBocadillo>()

            };
            //Inicializamos totales. Para inicializarlo a 0 y luego en el bucle donde vas añadiendo cada bocadillo vas sumando (se eliminan valores basura)
            //no los conozco hasta que recorro los items del pedido por eso los inicializo a 0 
            compra.PrecioTotal = 0;
            compra.nBoadillos = 0;

            //por cada bocadillo pedido, creamos la linea y actualizamos totales y stock
            foreach (var unidad in crearPedido.BocadilloItem)
            {
                //Buscar bocadillo en la BBDD por su ID
                var bocInfo = bocadillosBD.FirstOrDefault(b => b.Id == unidad.ID); //bocadillosBD variable creada PASO 2
               
                if (bocInfo == null)
                {
                    ModelState.AddModelError("Bocadillo", $"El bocadillo {unidad.ID} no existe.");
                    continue;
                }
                //VALIDACION de stock (comprobar que hay bocadillos)
                if (bocInfo.Stock < unidad.Cantidad)
                {
                    ModelState.AddModelError("CrearPedido", $"Error! se han pedido {unidad.Cantidad} bocadillos, pero no hay suficientes");
                }

                var cliente = new ApplicationUser
                {
                    NombreCliente = crearPedido.NombreCliente,
                    ApellidoCliente1=crearPedido.ApellidoCliente1,
                    ApellidoCliente2=crearPedido.ApellidoCliente2 ?? string.Empty,

                };
                //ahora que estoy recorriendo las lineas calculo el precio total
                var subtotal = bocInfo.Pvp * unidad.Cantidad; //calculo precio total de esa linea


                var linea = new CompraBocadillo  //creo la linea de esa unidad
                {
                    BocadilloId = bocInfo.Id,
                    Cantidad = unidad.Cantidad,
                    NombreBocadillo = bocInfo.Nombre,
                    Precio = bocInfo.Pvp,
                    TipoPan = new List<TipoPan>()
                };
                compra.BocadillosComprados.Add(linea);

                compra.PrecioTotal += subtotal;
                compra.nBoadillos += unidad.Cantidad;

            }

            //Si hay errores de validación, no sigo con la lógica normal
            if (ModelState.ErrorCount > 0)
                return BadRequest(new ValidationProblemDetails(ModelState));


            _context.Add(compra);

            try
            {
                
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                ModelState.AddModelError("Compra", $"Error! There was an error while saving your rental, plese, try again later");
                return Conflict("Error" + ex.Message);

            }
            //devuelvo PedidoDetails
            var pedidoDetalles = new DetailsPedidoDTO(compra.CompraId,
                compra.FechaCompra, compra.ApplicationUser.NombreCliente, compra.ApplicationUser.ApellidoCliente1,
                compra.ApplicationUser.ApellidoCliente2, crearPedido.Metodo,
                crearPedido.BocadilloItem);

            //devuelvo el recurso DTO de detalles del pedido
            return CreatedAtAction("GetPedido",new { id = compra.CompraId },pedidoDetalles);
        


        }
        
        

    }
}