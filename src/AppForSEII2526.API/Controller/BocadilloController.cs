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
        [ProducesResponseType(typeof(IList<SelectBocadilloDTO>),(int)HttpStatusCode.OK)]
        //metodo que devuelve un ActionResult 
        public async Task<ActionResult> GetBocadilloParaPedir(string? filtroTamano, string? filtroTipoPan) //tipo Pan bien que sea String 
        {
            
            //Empiezas una consulta LINQ sobre tabla Movies en ApplicationDbContext
            IList<SelectBocadilloDTO> bocadillos= await _context.Bocadillos
                .Include(b => b.tipopan) //cuando cargue cada bocadillo, que incluya navegacion TipoPan
                .Include(b=> b.ComprasBocadillo) //igual con comprasBocadillo
                    .ThenInclude(cb=>cb.Compra) //para cada compraBocadillo incluye su compra
                     //FILTROS: tamáño y tipo de pan (los 2 como string)
                    .Where(bocadillo=>(filtroTamano==null || bocadillo.Tamano.ToString().Equals(filtroTamano)) && 
                (filtroTipoPan==null  || bocadillo.tipopan.Nombre.Equals(filtroTipoPan)))
                .OrderBy(bocadillo=> bocadillo.Nombre)

                //CREO EL DTO que voy a exponer al cliente: SelectBocadilloDTO
                .Select(b=>new SelectBocadilloDTO(b.Id, b.Nombre, b.Tamano, b.tipopan.Nombre, b.Pvp))
                .ToListAsync();

            //Comprobamos si hay resultados
            if ( !bocadillos.Any()) //no hace falta lo del null porque bocadillos nunca va a ser null despues de ToListAsync() (si puede ser [])
            {
                return NotFound("No hay bocadillos que cumplan los requisitos");
            }
            return Ok(bocadillos);

                


        }


    }
}
