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

        // Nueva prueba: id inválido (negativo o 0) debe devolver NotFound según la nueva regla
        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task GetPedido_Returns_NotFound_For_InvalidId(int invalidId)
        {
            // Arrange
            var logger = new Mock<ILogger<PedidoController>>().Object;
            var controller = new PedidoController(_context, logger);

            // Act
            var result = await controller.GetPedido(invalidId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        // Opcional: si el controlador ahora incluye Stock en ItemPedidoDTO, comprobamos que se devuelve correctamente.
        // Si no has añadido Stock al DTO o la proyección, borra esta prueba.
        [Fact]
        public async Task GetPedido_Includes_Stock_In_Item()
        {
            // Arrange
            var logger = new Mock<ILogger<PedidoController>>().Object;
            var controller = new PedidoController(_context, logger);

            // Act
            var result = await controller.GetPedido(_compraSeed.CompraId);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var dto = Assert.IsType<DetailsPedidoDTO>(ok.Value);

            // Aseguramos que hay exactamente una línea y comparamos el stock con el seed
            Assert.Single(dto.BocadilloItem);
            var itemDto = dto.BocadilloItem.Single();
            var seedItem = _compraSeed.BocadillosComprados.Single();

            // Si ItemPedidoDTO tiene la propiedad Stock, esta aserción sirve.
            // Si no existe Stock en ItemPedidoDTO, esta línea causará error y debes eliminarla.
            Assert.Equal(seedItem.Bocadillo.Stock, itemDto.Stock);
        }
    }
}