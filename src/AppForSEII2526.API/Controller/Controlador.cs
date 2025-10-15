using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AppForSEII2526.API.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class Controlador : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<Controlador> _logger;

        public Controlador(ApplicationDbContext context, ILogger<Controlador> logger)
        {
            _context = context;
            _logger = logger;
        }


    }
}
