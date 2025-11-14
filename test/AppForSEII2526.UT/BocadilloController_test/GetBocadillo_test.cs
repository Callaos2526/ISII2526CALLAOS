using AppForMovies.UT;
using AppForSEII2526.API;
using AppForSEII2526.API.DTOs.PedidoBocaDTOs;
using AppForSEII2526.API.Models;
using AppForSEII2526.UT;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace AppForSEII2526.UT.BocadilloController_test
{
    public class GetBocadillo_test : AppForMovies4SqliteUT
    {
        public GetBocadillo_test()
        {
            var panes = new List<TipoPan>
            {
                new TipoPan { Nombre = "Barra" },
                new TipoPan { Nombre = "Integral" },
                new TipoPan { Nombre = "Chapata" },
            };

            var bocadillos = new List<Bocadillo>()
            {// el orden importa porque EF asigna IDs secuenciales al insertar
                new Bocadillo { Nombre = "Atún con tomate", Tamano = Tamaño.normal, tipopan = panes[0], Pvp = 3.0F, Resenyabocadillo = "" },
                new Bocadillo { Nombre = "Jamón y queso",   Tamano = Tamaño.normal, tipopan = panes[1], Pvp = 4.0F, Resenyabocadillo = "" },
                new Bocadillo { Nombre = "Vegetal",         Tamano = Tamaño.pequeño, tipopan = panes[2], Pvp = 3.5F, Resenyabocadillo = "" },
                new Bocadillo { Nombre = "Pollo asado",     Tamano = Tamaño.normal, tipopan = panes[0], Pvp = 3.0F, Resenyabocadillo = "" },
                new Bocadillo { Nombre = "Lomo con queso",  Tamano = Tamaño.normal, tipopan = panes[2], Pvp = 3.0F, Resenyabocadillo = "" },
            };

            _context.AddRange(panes);
            _context.AddRange(bocadillos);
            _context.SaveChanges();
        }

        public static IEnumerable<object[]> TestCasesFor_GetBocadilloParaPedir_OK()
        {
            // Creamos la lista base en el mismo orden de inserción en el constructor
            // (IDs asignados por EF en ese orden)
            var BocadillosDTOs = new List<SelectBocadilloDTO>
    {
        new SelectBocadilloDTO(1, "Atún con tomate", Tamaño.normal, "Barra", 3.0F),
        new SelectBocadilloDTO(2, "Jamón y queso",   Tamaño.normal, "Integral", 4.0F),
        new SelectBocadilloDTO(3, "Vegetal",         Tamaño.pequeño,"Chapata", 3.5F),
        new SelectBocadilloDTO(4, "Pollo asado",     Tamaño.normal, "Barra", 3.0F),
        new SelectBocadilloDTO(5, "Lomo con queso",  Tamaño.normal, "Chapata", 3.0F),
    };

            // Construimos explícitamente los casos esperados (como hace la profesora)
            var tc1 = BocadillosDTOs.OrderBy(b => b.NombreBocadillo).ToList(); // todos ordenados por nombre

            // pequeño -> "Vegetal" (baseList[2])
            var tc2 = new List<SelectBocadilloDTO> { BocadillosDTOs[2] }
                      .OrderBy(b => b.NombreBocadillo).ToList();

            // Chapata -> "Vegetal" (baseList[2]) y "Lomo con queso" (baseList[4])
            var tc3 = new List<SelectBocadilloDTO> { BocadillosDTOs[2], BocadillosDTOs[4] }
                      .OrderBy(b => b.NombreBocadillo).ToList();

            // normal + Integral -> "Jamón y queso" (baseList[1])
            var tc4 = new List<SelectBocadilloDTO> { BocadillosDTOs[1] }
                      .OrderBy(b => b.NombreBocadillo).ToList();

            // IMPORTANTE: pasar las cadenas que coincidan con los nombres del enum Tamaño
            var allTests = new List<object[]>
    {
        new object[] { null,                         null,      tc1 },
        new object[] { Tamaño.pequeño.ToString(),    null,      tc2 }, // filtro por tamaño pequeño
        new object[] { null,                         "Chapata", tc3 },
        new object[] { Tamaño.normal.ToString(),     "Integral",tc4 }, // filtro por tamaño normal + Integral
    };
            return allTests;
        }

        [Theory]
        [MemberData(nameof(TestCasesFor_GetBocadilloParaPedir_OK))]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetBocadilloParaPedir_OK_test(string? filtroTamano, string? filtroTipoPan,
            IList<SelectBocadilloDTO> expectedBocadillos)
        {
            // Arrange
            var controller = new BocadilloController(_context, null);

            // Act
            var result = await controller.GetBocadilloParaPedir(filtroTamano, filtroTipoPan);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var bocadilloDTOsActual = Assert.IsType<List<SelectBocadilloDTO>>(okResult.Value);
            Assert.Equal(expectedBocadillos, bocadilloDTOsActual);

            
        }

        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        public async Task GetBocadilloParaPedir_FiltrosSinResultados_DevuelveNotFound()
        {   //tenemos que devolver NotFound porque el controlador lo hace así (ignora el filtroTamano inválido y devuelve notfound)
            //(Lo hemos hecho asi para que el test sea coherente con el comportamiento del controlador)
            var mock = new Mock<ILogger<BocadilloController>>();
            ILogger<BocadilloController> logger = mock.Object;
            var controller = new BocadilloController(_context, logger);

            // filtroTamano no se parsea (el controlador lo ignorará),
            // filtroTipoPan sí se aplica y con este valor garantizamos 0 resultados
            var filtroTamanoInvalido = "Grande";
            var filtroTipoPanInexistente = "PanInexistente";

            // Act: llamamos al método bajo prueba
            var result = await controller.GetBocadilloParaPedir(filtroTamanoInvalido, filtroTipoPanInexistente);

            // Assert: comprobamos que el controlador responde NotFound con el mensaje esperado
            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            var message = Assert.IsType<string>(notFound.Value);
            Assert.Equal("No hay bocadillos que cumplan los requisitos", message);
            Assert.Equal((int)HttpStatusCode.NotFound, notFound.StatusCode);


        }

    }
}