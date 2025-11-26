
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
            
            if (id <= 0)
            {
                _logger.LogError("Error: Id de compra no valido");
                return NotFound();
            }

            //buscar compra con ese id, incluyendo las lineas y proyectarla
            var comprasdto = await _context.Compras 
             .Where(compra => compra.CompraId == id)                
             .Include(compra => compra.BocadillosComprados) 
                .ThenInclude(bocadilloItem => bocadilloItem.Bocadillo) 
                     .ThenInclude(Bocadillo => Bocadillo.tipopan)    
             .Select(compra => new DetailsPedidoDTO(
                 compra.CompraId,
                 compra.FechaCompra,
                 compra.ApplicationUser.NombreCliente, 
                 compra.ApplicationUser.ApellidoCliente1,
                 compra.ApplicationUser.ApellidoCliente2,
                 compra.metodoPago.metodoName,
                 compra.BocadillosComprados.Select(
                     cb => new ItemPedidoDTO(  
                         cb.BocadilloId,
                         cb.Bocadillo.Nombre,
                         cb.Bocadillo.tipopan.Nombre, 
                         cb.Cantidad,
                         cb.Bocadillo.Pvp
                     )).ToList())).FirstOrDefaultAsync();

            if (comprasdto == null )
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

            if (string.IsNullOrWhiteSpace(crearPedido.NombreCliente))
                ModelState.AddModelError("Nombre", "El nombre es obligatorio");
            if (string.IsNullOrWhiteSpace(crearPedido.ApellidoCliente1))
                ModelState.AddModelError("Apellido_1", "El primer apellido es obligatorio");

            // Buscar usuario existente (case-insensitive)
            var nombre = crearPedido.NombreCliente?.Trim();
            var apellido1 = crearPedido.ApellidoCliente1?.Trim();
            var apellido2 = crearPedido.ApellidoCliente2?.Trim();

            if (ModelState.ErrorCount > 0)
                return BadRequest(new ValidationProblemDetails(ModelState));

            var usersQuery = _context.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(nombre) && !string.IsNullOrWhiteSpace(apellido1))
            {
                var nombreLower = nombre.ToLower();
                var apellido1Lower = apellido1.ToLower();
                usersQuery = usersQuery.Where(u =>
                    u.NombreCliente.ToLower() == nombreLower &&
                    u.ApellidoCliente1.ToLower() == apellido1Lower);

                if (!string.IsNullOrWhiteSpace(apellido2))
                {
                    var apellido2Lower = apellido2.ToLower();
                    usersQuery = usersQuery.Where(u => u.ApellidoCliente2.ToLower() == apellido2Lower);
                }
            }

            var user = await usersQuery.FirstOrDefaultAsync();
            if (user == null)
            {
                ModelState.AddModelError("ApplicationUser", "Error! Nombre y/o apellidos no registrados.");
                return BadRequest(new ValidationProblemDetails(ModelState));
            }


            //comprobacion del metodo de pago
            // 3. Obtener método de pago 
            var metodo_Pago = await _context.Set<MetodoPago>()
                .FirstOrDefaultAsync(m => m.metodoName == crearPedido.Metodo);

            if (metodo_Pago == null)
            {
                ModelState.AddModelError("Metodo",
                    $"El método de pago '{crearPedido.Metodo}' no está registrado.");
            }                           

            //PASO 2. vamos recuperando objetos y rellenando el pedido 

            //sacar nombre de bocadillos que quiere el cliente
            var bocadilloNombres = crearPedido.BocadilloItem.Select(bi => bi.ID).ToList();
            //cargamos bocadillos desde la BD con la info que necesitamos 
            var bocadillosBD = _context.Bocadillos
                .Where(b => bocadilloNombres.Contains(b.Id))
                .Select(b=> new 
                {
                     b.Id,
                     b.Nombre,
                     b.Pvp,
                     b.Stock
                }).ToList();

           
           //crear compra usando usuario recuperado
            var compra = new Compra             //algunos paramtetros los saco del DTO que recibo
            { 
                metodoPago = metodo_Pago,
                FechaCompra = DateTime.Now, //aqui pongo la fecha en el momento 
                ApplicationUser = user,
                BocadillosComprados = new List<CompraBocadillo>(),
                PrecioTotal = 0,
                nBoadillos = 0
            };
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
                ModelState.AddModelError("Compra", "Error interno guardando la compra.");
                return Conflict("Error" + ex.Message);

            }
            //devuelvo PedidoDetails
            var pedidoDetalles = new DetailsPedidoDTO(compra.CompraId,
                compra.FechaCompra, compra.ApplicationUser.NombreCliente, compra.ApplicationUser.ApellidoCliente1,
                compra.ApplicationUser.ApellidoCliente2, metodo_Pago.metodoName,
                crearPedido.BocadilloItem);

            //devuelvo el recurso DTO de detalles del pedido
            return CreatedAtAction("GetPedido",new { id = compra.CompraId },pedidoDetalles);
        


        }
        
        

    }
}