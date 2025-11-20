using AppForMovies.UT;
using AppForSEII2526.API.Controllers;
using AppForSEII2526.API.DTOs.ComprarMerch;

namespace AppForSEII2526.UT.MerchController_test
{
    public class PostMerch_test : AppForMovies4SqliteUT
    {
        private const string _nombre = "Miguel";
        private const string _apellido1 = "Requena";
        private const string _apellido2 = "López";
        private const string _direccion = "Calle prueba1";
        private const string _metodoValido = "Paypal";

        public PostMerch_test()
        {
            var tipos = new List<TipoProducto>()
            {
                new TipoProducto("Camiseta", 1),
                new TipoProducto("Pantalon", 2)
            };

            var productos = new List<Producto>()
            {
                // productoid, nombre, pvp, stock
                new Producto(1, "Camiseta", 10, 5) { TipoProducto = tipos[0] },
                new Producto(2, "Pantalon", 20, 1) { TipoProducto = tipos[1] }
            };

            _context.TipoProducto.AddRange(tipos);
            _context.Producto.AddRange(productos);

            // Añadimos métodos de pago que usa el controlador
            _context.Paypals.Add(new Paypal());
            _context.Tarjetas.Add(new Tarjeta());
            _context.GooglePays.Add(new GooglePay());

            _context.SaveChanges();
        }

        public static IEnumerable<object[]> TestCasesFor_CreateCompraMerch()
        {
            var dtoNoItems = new ComprarMerchCreateDTO()
            {
                Nombre = _nombre,
                Apellido_1 = _apellido1,
                Apellido_2 = _apellido2,
                Direccion_Envio = _direccion,
                Metodo_Pago = _metodoValido,
                MerchItems = new List<ComprarMerchItemDTO>()
            };

            var dtoCantidad0 = new ComprarMerchCreateDTO()
            {
                Nombre = _nombre,
                Apellido_1 = _apellido1,
                Direccion_Envio = _direccion,
                Metodo_Pago = _metodoValido,
                MerchItems = new List<ComprarMerchItemDTO>()
                {
                    new ComprarMerchItemDTO(1, "Camiseta", 10, "Camiseta", 0)
                }
            };

            var dtoProductoNoExiste = new ComprarMerchCreateDTO()
            {
                Nombre = _nombre,
                Apellido_1 = _apellido1,
                Direccion_Envio = _direccion,
                Metodo_Pago = _metodoValido,
                MerchItems = new List<ComprarMerchItemDTO>()
                {
                    new ComprarMerchItemDTO(999, "NoExiste", 0, "NA", 1)
                }
            };

            var dtoInsuficienteStock = new ComprarMerchCreateDTO()
            {
                Nombre = _nombre,
                Apellido_1 = _apellido1,
                Direccion_Envio = _direccion,
                Metodo_Pago = _metodoValido,
                MerchItems = new List<ComprarMerchItemDTO>()
                {
                    new ComprarMerchItemDTO(2, "Pantalon", 20, "Pantalon", 5)
                }
            };

            var dtoMetodoInvalido = new ComprarMerchCreateDTO()
            {
                Nombre = _nombre,
                Apellido_1 = _apellido1,
                Direccion_Envio = _direccion,
                Metodo_Pago = "Bitcoin",
                MerchItems = new List<ComprarMerchItemDTO>()
                {
                    new ComprarMerchItemDTO(1, "Camiseta", 10, "Camiseta", 1)
                }
            };
            var dtoDireccionInvalida = new ComprarMerchCreateDTO()
            {
                Nombre = _nombre,
                Apellido_1 = _apellido1,
                Direccion_Envio = "C/ Rosario",
                Metodo_Pago = _metodoValido,
                MerchItems = new List<ComprarMerchItemDTO>()
                {
                    new ComprarMerchItemDTO(1, "Camiseta", 10, "Camiseta", 1)
                }
            };

            return new List<object[]>
            {
                new object[] { dtoNoItems, "¡Error! Debes seleccionar al menos un producto de merchandising" },
                new object[] { dtoCantidad0, "La cantidad debe ser mayor que 0." },
                new object[] { dtoProductoNoExiste, "El producto con id 999 no existe." },
                new object[] { dtoInsuficienteStock, "No hay suficiente stock para Pantalon." },
                new object[] { dtoMetodoInvalido, "El método de pago 'Bitcoin' no existe." },
                new object[] { dtoDireccionInvalida, "Error! por favor introduce una dirección de envío válido" }
            };
        }

        [Theory]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        [MemberData(nameof(TestCasesFor_CreateCompraMerch))]
        public async Task CreateCompraMerch_Error_test(ComprarMerchCreateDTO dto, string errorExpected)
        {
            // Arrange
            var mock = new Mock<ILogger<CompraMerchController>>();
            ILogger<CompraMerchController> logger = mock.Object;
            var controller = new CompraMerchController(_context, logger);

            // Act
            var result = await controller.CrearCompraMerch(dto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var problemDetails = Assert.IsType<ValidationProblemDetails>(badRequestResult.Value);
            var errorActual = problemDetails.Errors.First().Value[0];

            Assert.StartsWith(errorExpected, errorActual);
        }

        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        public async Task CreateCompraMerch_Success_test()
        {
            // Arrange
            var mock = new Mock<ILogger<CompraMerchController>>();
            ILogger<CompraMerchController> logger = mock.Object;
            var controller = new CompraMerchController(_context, logger);

            var dto = new ComprarMerchCreateDTO()
            {
                Nombre = _nombre,
                Apellido_1 = _apellido1,
                Apellido_2 = _apellido2,
                Direccion_Envio = _direccion,
                Metodo_Pago = _metodoValido,
                MerchItems = new List<ComprarMerchItemDTO>()
                {
                    new ComprarMerchItemDTO(1, "Camiseta", 10, "Camiseta", 2)
                }
            };


            // Act
            var result = await controller.CrearCompraMerch(dto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var compraDetail = Assert.IsType<ComprarMerchDetailDTO>(createdResult.Value);

            Assert.Equal(_nombre, compraDetail.Nombre);
            Assert.Equal(_apellido1, compraDetail.Apellido_1);
            Assert.Equal(_direccion, compraDetail.Direccion_Envio);
            Assert.Equal(_metodoValido, compraDetail.Metodo_Pago);
            Assert.Equal(2, compraDetail.Cantidad);

            Assert.Single(compraDetail.MerchItems);
            var item = compraDetail.MerchItems.First();
            Assert.Equal(1, item.Id);
            Assert.Equal("Camiseta", item.NombreProducto);
            Assert.Equal(10, item.PVP);
            Assert.Equal(2, item.Cantidad);
        }
    }
}