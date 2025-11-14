using AppForMovies.UT;
using AppForSEII2526.API.Controllers;
using AppForSEII2526.API.DTOs.PedidoBocaDTOs;
using AppForSEII2526.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AppForSEII2526.UT.PedidoController_test
{
    // Clase de pruebas unitarias para el endpoint CrearPedido del controller PedidoController.
    // Hereda de AppForMovies4SqliteUT que prepara un contexto SQLite en memoria para cada test.
    public class Post_CrearPedido_test : AppForMovies4SqliteUT
    {
        // Campos de prueba (instanciados en el constructor). Se marcan readonly porque
        // se inicializan en el constructor y no deben cambiar durante la ejecución del test.
        private readonly ApplicationUser _existingUser;
        private readonly MetodoPago _metodo;
        private readonly Bocadillo _bocadillo;
        private readonly TipoPan _tipoPan;

        // Constructor de la clase de pruebas: prepara datos en la BBDD en memoria
        // que los tests utilizarán (tipo de pan, método de pago, usuario y bocadillo).
        public Post_CrearPedido_test()
        {
            // Creamos un tipo de pan "Integral".
            _tipoPan = new TipoPan { Nombre = "Integral" };

            // Crear un método de pago concreto (Tarjeta). En la BBDD se espera su nombre.
            _metodo = new Tarjeta(); // metodoName == "Tarjeta"

            // Creamos un usuario existente para los tests de éxito.
            _existingUser = new ApplicationUser("Luis", "Gonzalez", "Perez") { Id = Guid.NewGuid().ToString() };

            // Importante: inicializar Resenyabocadillo (NOT NULL en la BBDD SQLite usada).
            // Configuramos un bocadillo con stock 5 para poder testear casos de stock insuficiente.
            _bocadillo = new Bocadillo
            {
                Id = 10,
                Nombre = "Pollo",
                Pvp = 4.5F,
                Stock = 5,
                Tamano = Tamaño.normal,
                tipopan = _tipoPan,
                Resenyabocadillo = "" // evita SQLite NOT NULL constraint failed
            };

            // Añadimos todas las entidades al contexto de pruebas y guardamos.
            _context.AddRange(_tipoPan, _metodo, _existingUser, _bocadillo);
            _context.SaveChanges();
        }

        // Método que proporciona casos de prueba parametrizados para errores en CreatePedido.
        // Cada elemento devuelto es un array con: (CreatePedidoDTO dto, string mensajeEsperado).
        public static IEnumerable<object[]> TestCasesFor_CreatePedido()
        {
            // Caso: lista de items vacía -> debe provocar validación de "debes seleccionar algun bocadillo".
            var emptyItemsDto = new CreatePedidoDTO(
                nombreCliente: "Luis",
                apellidoCliente1: "Gonzalez",
                apellidosCliente2: "Perez",
                metodo: "Tarjeta",
                bocadilloItem: new List<ItemPedidoDTO>());

            // Caso: cantidad cero en un item -> validación de cantidad obligatoria > 0.
            var zeroQuantityDto = new CreatePedidoDTO(
                nombreCliente: "Luis",
                apellidoCliente1: "Gonzalez",
                apellidosCliente2: "Perez",
                metodo: "Tarjeta",
                bocadilloItem: new List<ItemPedidoDTO> { new ItemPedidoDTO(10, "Pollo", "Integral", 0, 4.5F) });

            // Caso: usuario no encontrado -> el controlador debe devolver error de usuario no registrado.
            var userNotFoundDto = new CreatePedidoDTO(
                nombreCliente: "NoExiste",
                apellidoCliente1: "X",
                apellidosCliente2: null,
                metodo: "Tarjeta",
                bocadilloItem: new List<ItemPedidoDTO> { new ItemPedidoDTO(10, "Pollo", "Integral", 1, 4.5F) });

            // Caso: método de pago no registrado -> validación de método no encontrado.
            var metodoNoRegistradoDto = new CreatePedidoDTO(
                nombreCliente: "Luis",
                apellidoCliente1: "Gonzalez",
                apellidosCliente2: "Perez",
                metodo: "MetodoInexistente",
                bocadilloItem: new List<ItemPedidoDTO> { new ItemPedidoDTO(10, "Pollo", "Integral", 1, 4.5F) });

            // Caso: se pide más cantidad que la existente en stock -> validación de stock insuficiente.
            var stockInsuficienteDto = new CreatePedidoDTO(
                nombreCliente: "Luis",
                apellidoCliente1: "Gonzalez",
                apellidosCliente2: "Perez",
                metodo: "Tarjeta",
                bocadilloItem: new List<ItemPedidoDTO> { new ItemPedidoDTO(10, "Pollo", "Integral", 10, 4.5F) }); // stock 5 en setup

            // Caso: bocadillo inexistente -> el controlador debe reportar que el bocadillo no existe.
            var bocadilloNoExisteDto = new CreatePedidoDTO(
                nombreCliente: "Luis",
                apellidoCliente1: "Gonzalez",
                apellidosCliente2: "Perez",
                metodo: "Tarjeta",
                bocadilloItem: new List<ItemPedidoDTO> { new ItemPedidoDTO(999, "NoExiste", "Integral", 1, 1.0F) });

            // Lista de pruebas con el mensaje esperado que se comparará con StartsWith en el test.
            var allTests = new List<object[]>
            {
                new object[] { emptyItemsDto, "Error! Debes seleccionar algun bocadillo" },
                new object[] { zeroQuantityDto, "La cantidad es obligatoria y debe ser mayor que 0." },
                new object[] { userNotFoundDto, "Error! Nombre y/o apellidos no registrados." },
                new object[] { metodoNoRegistradoDto, "El método de pago 'MetodoInexistente' no está registrado." },
                new object[] { stockInsuficienteDto, "Error! se han pedido 10 bocadillos, pero no hay suficientes" },
                new object[] { bocadilloNoExisteDto, "El bocadillo 999 no existe." },
            };
            return allTests;
        }

        // Test parametrizado que verifica que para cada DTO inválido el controlador devuelve BadRequest
        // con ValidationProblemDetails y que el mensaje de error empieza por la cadena esperada.
        [Theory]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        [MemberData(nameof(TestCasesFor_CreatePedido))]
        public async Task CrearPedido_Error_test(CreatePedidoDTO dto, string errorExpected)
        {
            // Arrange: mock del logger y creación del controller con el contexto de pruebas.
            var mock = new Mock<ILogger<PedidoController>>();
            ILogger<PedidoController> logger = mock.Object;
            var controller = new PedidoController(_context, logger);

            // Act: llamamos al método CrearPedido del controller con el DTO inválido.
            var result = await controller.CrearPedido(dto);

            // Assert: esperamos un BadRequestObjectResult con ValidationProblemDetails.
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var problemDetails = Assert.IsType<ValidationProblemDetails>(badRequestResult.Value);

            // Extraemos el primer mensaje de error y comprobamos que empieza por el texto esperado.
            var errorActual = problemDetails.Errors.First().Value[0];
            Assert.StartsWith(errorExpected, errorActual);
        }

        // Test que verifica el flujo exitoso: DTO válido debe crear una compra y devolver CreatedAtAction.
        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        public async Task CrearPedido_Success_test()
        {
            // Arrange: preparar logger y controller
            var mock = new Mock<ILogger<PedidoController>>();
            ILogger<PedidoController> logger = mock.Object;
            var controller = new PedidoController(_context, logger);

            // Construimos un DTO válido usando el usuario y bocadillo creados en el constructor.
            var dto = new CreatePedidoDTO(
                nombreCliente: _existingUser.NombreCliente,
                apellidoCliente1: _existingUser.ApellidoCliente1,
                apellidosCliente2: _existingUser.ApellidoCliente2,
                metodo: _metodo.metodoName,
                bocadilloItem: new List<ItemPedidoDTO> { new ItemPedidoDTO(_bocadillo.Id, _bocadillo.Nombre, _tipoPan.Nombre, 2, _bocadillo.Pvp) }
            );

            // Act: llamamos al endpoint CrearPedido.
            var result = await controller.CrearPedido(dto);

            // Assert: comprobamos CreatedAtActionResult y el DTO devuelto.
            var created = Assert.IsType<CreatedAtActionResult>(result);
            var details = Assert.IsType<DetailsPedidoDTO>(created.Value);

            // Comparaciones robustas: comprobamos datos relevantes (evitamos comparar Fecha/Id generados runtime).
            Assert.Equal(_existingUser.NombreCliente, details.NombreCliente);
            Assert.Equal(_existingUser.ApellidoCliente1, details.ApellidoCliente1);
            Assert.Equal(_metodo.metodoName, details.Metodo);
            Assert.Equal(1, _context.Compras.Count()); // se ha creado una compra en la BBDD de pruebas
            Assert.Equal(2, details.BocadilloItem.First().Cantidad);
        }
    }
}