using AppForMovies.UT;
using AppForSEII2526.API.Controllers;
using AppForSEII2526.API.DTOs.ComprarMerchDTOs;


namespace AppForSEII2526.UT.MerchController_test
{
    public class GetMerch_test : AppForMovies4SqliteUT
    {
        public GetMerch_test()
        {
            var tipos = new List<TipoProducto>()
            {
                new TipoProducto("Camiseta", 1),
                new TipoProducto("Pantalon", 2),
                new TipoProducto("Chaqueta", 3)
            };

            var productos = new List<Producto>()
            {
                new Producto(1, "Camiseta Star Wars", 20, 10) { TipoProducto = tipos[0] },
                new Producto(2, "Pantalon Jeans", 40, 5) { TipoProducto = tipos[1] },
                new Producto(3, "Chaqueta Cuero", 100, 0) { TipoProducto = tipos[2] }, // stock 0
                new Producto(4, "Camiseta Marvel", 25, 15) { TipoProducto = tipos[0] },
                new Producto(5, "Pantalon Chandal", 30, 8) { TipoProducto = tipos[1] }
            };

            _context.TipoProducto.AddRange(tipos);
            _context.Producto.AddRange(productos);
            _context.SaveChanges();
        }

        [Fact]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetMerch_NotFound_test()
        {
            // Arrange
            var mock = new Mock<ILogger<MerchandaisingController>>();
            ILogger<MerchandaisingController> logger = mock.Object;
            var controller = new MerchandaisingController(_context, logger);

            // Act: sólo 'Chaqueta' tiene stock 0, por tanto no debe encontrarse
            var result = await controller.GetMerchParaCompra("Chaqueta", null);

            // Assert
            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            var message = Assert.IsType<string>(notFound.Value);
            Assert.Equal("No hay productos que cumplan los requisitos", message);
        }

        [Fact]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetMerchParaCompra_Found_test()
        {
            // Arrange
            var mock = new Mock<ILogger<MerchandaisingController>>();
            ILogger<MerchandaisingController> logger = mock.Object;
            var controller = new MerchandaisingController(_context, logger);

            // Expected: productos con stock>0 ordenados por NombreProducto
            var expected = new List<ComprarMerchandaisingDTO>()
            {
                new ComprarMerchandaisingDTO(4, "Camiseta Marvel", 25, 15, "Camiseta"),
                new ComprarMerchandaisingDTO(1, "Camiseta Star Wars", 20, 10, "Camiseta"),
                new ComprarMerchandaisingDTO(5, "Pantalon Chandal", 30, 8, "Pantalon"),
                new ComprarMerchandaisingDTO(2, "Pantalon Jeans", 40, 5, "Pantalon")
            };

            // Act
            var result = await controller.GetMerchParaCompra(null, null);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var actual = Assert.IsType<List<ComprarMerchandaisingDTO>>(okResult.Value);
            Assert.Equal(expected, actual);
        }
    }
}