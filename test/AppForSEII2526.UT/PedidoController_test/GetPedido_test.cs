using AppForMovies.UT;
using AppForSEII2526.API.Controllers;
using AppForSEII2526.API.DTOs.PedidoBocaDTOs;
using AppForSEII2526.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AppForSEII2526.UT.PedidoController_test
{
    public class GetPedido_test : AppForMovies4SqliteUT
    {
        private readonly Compra _compraSeed;

        public GetPedido_test()
        {
            // Datos mínimos para una compra
            var tipoPan = new TipoPan { Nombre = "Barra" };
            var metodo = new Tarjeta(); // metodoName == "Tarjeta"
            var user = new ApplicationUser("Ana", "Lopez", "Gomez") { Id = Guid.NewGuid().ToString() };

            // IMPORTANTE: inicializar Resenyabocadillo para evitar NOT NULL constraint
            var boc = new Bocadillo { Id = 1, Nombre = "Atún", Pvp = 3.0F, Stock = 10, Tamano = Tamaño.normal, tipopan = tipoPan, Resenyabocadillo = "" };

            var compra = new Compra
            {
                FechaCompra = DateTime.Now,
                ApplicationUser = user,
                metodoPago = metodo,
                PrecioTotal = 6.0F,
                nBoadillos = 2
            };

            var linea = new CompraBocadillo
            {
                Bocadillo = boc,
                BocadilloId = boc.Id,
                Cantidad = 2,
                NombreBocadillo = boc.Nombre,
                Precio = boc.Pvp,
                Compra = compra
            };

            compra.BocadillosComprados.Add(linea);
            _context.AddRange(tipoPan, metodo, user, boc, compra);
            _context.SaveChanges();
            // Recuperamos la compra con su id asignado por EF
            _compraSeed = compra;
        }

        [Fact]
        public async Task GetPedido_Returns_Ok_With_Details()
        {
            // Arrange
            var logger = new Mock<ILogger<PedidoController>>().Object;
            var controller = new PedidoController(_context, logger);

            // Act
            var result = await controller.GetPedido(_compraSeed.CompraId);

            // Assert: el controlador devuelve ActionResult<DetailsPedidoDTO>,
            // el IActionResult real está en result.Result
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var dto = Assert.IsType<DetailsPedidoDTO>(ok.Value);

            Assert.Equal(_compraSeed.CompraId, dto.Id);
            Assert.Equal(_compraSeed.ApplicationUser.NombreCliente, dto.NombreCliente);
            Assert.Equal(_compraSeed.ApplicationUser.ApellidoCliente1, dto.ApellidoCliente1);
            Assert.Equal(_compraSeed.nBoadillos, dto.BocadilloItem.Sum(i => i.Cantidad));
        }

        [Fact]
        public async Task GetPedido_Returns_NotFound_When_NotExist()
        {
            // Arrange
            var logger = new Mock<ILogger<PedidoController>>().Object;
            var controller = new PedidoController(_context, logger);

            // Act
            var result = await controller.GetPedido(99999);

            // Assert: comprobar el IActionResult contenido en result.Result
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetPedido_Returns_NotFound_IdInexistente()
        {
            var logger = new Mock<ILogger<PedidoController>>().Object;
            var controller = new PedidoController(_context, logger);
            var idInvalido = -2;
            var result = await controller.GetPedido(idInvalido);
            Assert.IsType<NotFoundResult>(result.Result);
        }


    }
}