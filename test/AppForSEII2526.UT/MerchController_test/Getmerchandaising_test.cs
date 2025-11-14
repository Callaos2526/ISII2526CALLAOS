using AppForMovies.UT;
using AppForSEII2526.API.Controllers;
using AppForSEII2526.API.Models;
using AppForSEII2526.API.DTOs.ComprarMerch;


namespace AppForSEII2526.UT.MerchController_test
{
    public class Getmerchandaising_test : AppForMovies4SqliteUT
    {
        public Getmerchandaising_test()
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
                new Producto(3, "Chaqueta Cuero", 100, 0) { TipoProducto = tipos[2] }, // stock 0 -> excluded
                new Producto(4, "Camiseta Marvel", 25, 15) { TipoProducto = tipos[0] },
                new Producto(5, "Pantalon Chandal", 30, 8) { TipoProducto = tipos[1] }
            };
            _context.TipoProducto.AddRange(tipos);
            _context.Producto.AddRange(productos);
            _context.SaveChanges();
        }

        public static IEnumerable<object[]> TestCasesFor_GetMerchParaCompra_OK()
        {
            var allProducts = new List<ComprarMerchandaisingDTO>()
            {
                new ComprarMerchandaisingDTO(4, "Camiseta Marvel", 25, 15, "Camiseta"),
                new ComprarMerchandaisingDTO(1, "Camiseta Star Wars", 20, 10, "Camiseta"),
                new ComprarMerchandaisingDTO(5, "Pantalon Chandal", 30, 8, "Pantalon"),
                new ComprarMerchandaisingDTO(2, "Pantalon Jeans", 40, 5, "Pantalon")
            };

            var pantalones = new List<ComprarMerchandaisingDTO>()
            {
                new ComprarMerchandaisingDTO(5, "Pantalon Chandal", 30, 8, "Pantalon"),
                new ComprarMerchandaisingDTO(2, "Pantalon Jeans", 40, 5, "Pantalon")
            }
            .OrderBy(m => m.NombreProducto).ToList();

            var precioHasta25 = new List<ComprarMerchandaisingDTO>()
            {
                new ComprarMerchandaisingDTO(4, "Camiseta Marvel", 25, 15, "Camiseta"),
                new ComprarMerchandaisingDTO(1, "Camiseta Star Wars", 20, 10, "Camiseta")
            }
            .OrderBy(m => m.NombreProducto).ToList();
            ;

            var camisetaHasta22 = new List<ComprarMerchandaisingDTO>()
            {
                new ComprarMerchandaisingDTO(1, "Camiseta Star Wars", 20, 10, "Camiseta")
            };

            return new List<object[]>
            {
                new object[] { null, null, allProducts },
                new object[] { "Pantalon", null, pantalones },
                new object[] { null, 25, precioHasta25 },
                new object[] { "Camiseta", 22, camisetaHasta22 }
            };
        }

        [Theory]
        [MemberData(nameof(TestCasesFor_GetMerchParaCompra_OK))]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetMerchParaCompra_OK_test(string? filtroTipo, int? filtroPrecio, IList<ComprarMerchandaisingDTO> expectedProducts)
        {
            // Arrange
            var controller = new MerchandaisingController(_context, null);
            
            // Act
            var result = await controller.GetMerchParaCompra(filtroTipo, filtroPrecio);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var actual = Assert.IsType<List<ComprarMerchandaisingDTO>>(okResult.Value);
            Assert.Equal(expectedProducts, actual);
        }

        [Fact]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetMerchParaCompra_NotFound_test()
        {
            // Arrange
            var controller = new MerchandaisingController(_context, null);

            // Act: solo hay 'Chaqueta' con stock 0 -> debe devolver NotFound
            var result = await controller.GetMerchParaCompra("Chaqueta", null);

            // Assert
            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            var message = Assert.IsType<string>(notFound.Value);
            Assert.Equal("No hay productos que cumplan los requisitos", message);
        }
    }
}