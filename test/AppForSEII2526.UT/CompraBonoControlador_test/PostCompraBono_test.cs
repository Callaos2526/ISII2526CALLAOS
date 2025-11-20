using AppForMovies.UT;
using AppForSEII2526.API.DTOs.CompraBonoDTOs;
using LosDelEspacio.API.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppForSEII2526.UT.CompraBonoControlador_test
{
    public class PostCompraBono_test : AppForMovies4SqliteUT
    {
        private const string _clienteNombre = "Ana";
        private const string _clienteApellido1 = "Garcia";
        private const string _clienteApellido2 = "Lopez";
        private const string _metodoPago = "Tarjeta";

        private const string _bono1Nombre = "Bono Completo";
        private const string _tipo1Nombre = "Normal";
        private const string _bono2Nombre = "Bono Pequeño";
        private const string _tipo2Nombre = "Sin gluten";

        public PostCompraBono_test()
        {
            var tipos = new List<TipoBocadillo>() {
                new TipoBocadillo { NombreTipo = _tipo1Nombre },
                new TipoBocadillo { NombreTipo = _tipo2Nombre },
            };

            var bonos = new List<BonoBocadillo>(){
                new BonoBocadillo { Nombre = _bono1Nombre, PVP = 15.0, NBocadillos = 5, CantidadDisponible = 10, TipoBocadillos = tipos[0] },
                new BonoBocadillo { Nombre = _bono2Nombre, PVP = 10.0, NBocadillos = 2, CantidadDisponible = 2, TipoBocadillos = tipos[1] },
            };

            var tarjeta = new Tarjeta() { metodoName = _metodoPago };

            _context.AddRange(tipos);
            _context.AddRange(bonos);
            // Añadir como MetodoPago (aprovechando herencia)
            _context.MetodoPago.Add(tarjeta);
            _context.SaveChanges();
        }

        public static IEnumerable<object[]> TestCasesFor_CreateCompra()
        {
            var compraSinBonos = new CrearCompraDTO(0, _clienteNombre, _clienteApellido1, _clienteApellido2,
                DateTime.Today, _metodoPago, new List<BonoItemForCreateDTO>());

            var bonoItems = new List<BonoItemForCreateDTO>() { new BonoItemForCreateDTO(2, 5, _bono2Nombre, 10.0, 2, _tipo2Nombre) };

            var compraNombreVacio = new CrearCompraDTO(0, "", _clienteApellido1, _clienteApellido2,
                DateTime.Today, _metodoPago, bonoItems);

            var compraApellidosVacios = new CrearCompraDTO(0, _clienteNombre, "", "",
                DateTime.Today, _metodoPago, bonoItems);

            var compraMetodoPagoInvalido = new CrearCompraDTO(0, _clienteNombre, _clienteApellido1, _clienteApellido2,
                DateTime.Today, "Bitcoin", bonoItems);

            var compraBonoNoDisponible = new CrearCompraDTO(0, _clienteNombre, _clienteApellido1, _clienteApellido2,
                DateTime.Today, _metodoPago,
                new List<BonoItemForCreateDTO>() { new BonoItemForCreateDTO(2, 5, _bono2Nombre, 10.0, 1, _tipo2Nombre) });
            //Examen Sprint2
            var compraMenorPVP = new CrearCompraDTO(0, _clienteNombre, _clienteApellido1, _clienteApellido2,
                DateTime.Today, _metodoPago,
                new List<BonoItemForCreateDTO>() { new BonoItemForCreateDTO(2, 5, _bono2Nombre, 0.0, 2, _tipo2Nombre) });
            //Fin

            var allTests = new List<object[]>
            {
                
                new object[] { compraSinBonos, "Error! Debes seleccionar algún bono" },
                new object[] { compraNombreVacio, "Error! El nombre es obligatorio" },
                new object[] { compraApellidosVacios, "Error! Los apellidos son obligatorios" },
                new object[] { compraMetodoPagoInvalido, "Error! Método de pago no existe" },
                new object[] { compraBonoNoDisponible, "Error, no hay suficiente stock del bono" },
                //Examen Sprint2
                new object[] { compraMenorPVP, "Error, El precio unitario tiene que ser mayor que 3" },
                //FIN

            };

            return allTests;
        }

        [Theory]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        [MemberData(nameof(TestCasesFor_CreateCompra))]
        public async Task CrearCompra_Error_test(CrearCompraDTO compraDTO, string errorExpected)
        {
            // Arrange
            var mock = new Mock<ILogger<ComprarBonoControlador>>();
            ILogger<ComprarBonoControlador> logger = mock.Object;

            var controller = new ComprarBonoControlador(_context, logger);

            // Act
            var result = await controller.CrearCompra(compraDTO);

            //Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var problemDetails = Assert.IsType<ValidationProblemDetails>(badRequestResult.Value);

            var errorActual = problemDetails.Errors.First().Value[0];

            Assert.StartsWith(errorExpected, errorActual);
        }

        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        public async Task CrearCompra_Success_test()
        {
            // Arrange
            var mock = new Mock<ILogger<ComprarBonoControlador>>();
            ILogger<ComprarBonoControlador> logger = mock.Object;

            var controller = new ComprarBonoControlador(_context, logger);

            var compraDTO = new CrearCompraDTO(1, _clienteNombre, _clienteApellido1, _clienteApellido2,
                DateTime.Today, _metodoPago, new List<BonoItemForCreateDTO>()
                { new BonoItemForCreateDTO(2, 2, _bono2Nombre, 10.0, 2, _tipo2Nombre) });

            var expectedCompraDetailDTO = new CompraBonoDetallesDTO(1, // ID esperado (se actualizará con el real)
                new ApplicationUser(_clienteNombre, _clienteApellido1, _clienteApellido2),
                new Tarjeta() { metodoName = _metodoPago },
                DateTime.Today, 20.0,
                new List<BonoItemForCreateDTO>()
                { new BonoItemForCreateDTO(2, 2, _bono2Nombre, 10.0, 2, _tipo2Nombre) });

            // Act
            var result = await controller.CrearCompra(compraDTO);

            //Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var actualCompraDetailDTO = Assert.IsType<CompraBonoDetallesDTO>(createdResult.Value);

            Assert.Equal(expectedCompraDetailDTO, actualCompraDetailDTO);
        }
    }
}
