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
        {/*
            
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
            return Ok(bocadillos);*/
            
                Tamaño? tamanoFiltrado = null; // Variable nullable para almacenar el enum filtrado si se puede parsear

            if (!string.IsNullOrWhiteSpace(filtroTamano) && // Comprueba que el filtro de tamaño venga con algún valor.
                    Enum.TryParse<Tamaño>(filtroTamano, ignoreCase: true, out var parsed)) // Intenta convertir el string al enum (ignorando mayúsculas/minúsculas)
            {
                    tamanoFiltrado = parsed; // Si pudo parsear, guarda el valor convertido.
                }

                var query = _context.Bocadillos
                    .AsNoTracking() // No se hace seguimiento de cambios (más eficiente para solo lectura).
                    .Include(b => b.tipopan)  // Carga la navegación TipoPan para cada bocadillo.
                    .Include(b => b.ComprasBocadillo).ThenInclude(cb => cb.Compra) // Carga ComprasBocadillo y su Compra asociada.
                    .AsQueryable();  // Asegura que seguimos trabajando con una consulta componible.


            //APLICAR FILTROS
            if (tamanoFiltrado.HasValue) // Si el filtro de tamaño es válido...
                query = query.Where(b => b.Tamano == tamanoFiltrado.Value); // ...aplica el filtro tipado por enum (sin ToString()).

            if (!string.IsNullOrWhiteSpace(filtroTipoPan)) // Si llegó un filtro para el tipo de pan...
                query = query.Where(b => b.tipopan.Nombre == filtroTipoPan); // ...filtra por coincidencia exacta del nombre de pan.

            var bocadillos = await query  // Ejecuta la consulta construida...
                    .OrderBy(b => b.Nombre)  // ...ordenando alfabéticamente por nombre.
                    .Select(b => new SelectBocadilloDTO(b.Id, b.Nombre, b.Tamano, b.tipopan.Nombre, b.Pvp))
                    .ToListAsync();

                if (!bocadillos.Any())  // Si la lista quedó vacía...
                return NotFound("No hay bocadillos que cumplan los requisitos"); // ...devuelve 404 con un mensaje.

            return Ok(bocadillos);  // Si hay resultados, devuelve 200 con la lista de DTOs.






        }


    }
}
