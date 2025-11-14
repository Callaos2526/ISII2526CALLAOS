using AppForSEII2526.API;
using AppForSEII2526.UT;
using AppForSEII2526.API.Models;
using AppForSEII2526.API.DTOs.PedidoBocaDTOs;
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
using AppForMovies.UT;

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
            {
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
            // Creamos la lista esperada (sin depender de IDs fijos)
            var all = new List<SelectBocadilloDTO>
            {
                new SelectBocadilloDTO(1, "Atún con tomate", Tamaño.normal, "Barra", 3.0F),
                new SelectBocadilloDTO(2, "Jamón y queso",   Tamaño.normal, "Integral", 4.0F),
                new SelectBocadilloDTO(3, "Lomo con queso",  Tamaño.normal, "Chapata", 3.0F),
                new SelectBocadilloDTO(4, "Pollo asado",     Tamaño.normal, "Barra", 3.0F),
                new SelectBocadilloDTO(5, "Vegetal",         Tamaño.pequeño,"Chapata", 3.5F),
            }
            .OrderBy(b => b.NombreBocadillo).ToList();

            // Subconjuntos esperados
            var tc1 = all;
            var tc2 = all.Where(b => b.Tamano == Tamaño.pequeño).OrderBy(b => b.NombreBocadillo).ToList();
            var tc3 = all.Where(b => b.TipoPanNombre == "Chapata").OrderBy(b => b.NombreBocadillo).ToList();
            var tc4 = all.Where(b => b.Tamano == Tamaño.normal && b.TipoPanNombre == "Integral")
                         .OrderBy(b => b.NombreBocadillo).ToList();

            // IMPORTANTE: pasar las cadenas que coincidan con los nombres del enum Tamaño
            return new List<object[]>
            {
                new object[] { null,                         null,      tc1 },
                new object[] { Tamaño.pequeño.ToString(),    null,      tc2 }, // filtro por tamaño pequeño
                new object[] { null,                         "Chapata", tc3 },
                new object[] { Tamaño.normal.ToString(),     "Integral",tc4 }, // filtro por tamaño normal + Integral
            };
        }

        [Theory]
        [MemberData(nameof(TestCasesFor_GetBocadilloParaPedir_OK))]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetBocadilloParaPedir_OK_test(string? filtroTamano, string? filtroTipoPan,
            IList<SelectBocadilloDTO> expectedBocadillos)
        {
            // Arrange
            var controller = new BocadilloController(_context, /* logger */ null);

            // Act
            var result = await controller.GetBocadilloParaPedir(filtroTamano, filtroTipoPan);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var dtoList = Assert.IsType<List<SelectBocadilloDTO>>(okResult.Value);

            // Comparación por propiedades relevantes evitando fragilidad por IDs y por float precision
            var expectedProjection = expectedBocadillos
                .Select(b => new
                {
                    b.NombreBocadillo,
                    b.Tamano,
                    b.TipoPanNombre,
                    Pvp = Math.Round(b.Pvp, 2)
                })
                .ToList();

            var actualProjection = dtoList
                .Select(b => new
                {
                    b.NombreBocadillo,
                    b.Tamano,
                    b.TipoPanNombre,
                    Pvp = Math.Round(b.Pvp, 2)
                })
                .ToList();

            Assert.Equal(expectedProjection, actualProjection);
        }

        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        public async Task GetBocadilloParaPedir_NotFound_test()
        {
            var mock = new Mock<ILogger<BocadilloController>>();
            ILogger<BocadilloController> logger = mock.Object;
            var controller = new BocadilloController(_context, logger);

            var filtroTamano = "Grande";
            var filtroTipoPan = "PanInexistente";

            var result = await controller.GetBocadilloParaPedir(filtroTamano, filtroTipoPan);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            var message = Assert.IsType<string>(notFound.Value);
            Assert.Equal("No hay bocadillos que cumplan los requisitos", message);
            Assert.Equal((int)HttpStatusCode.NotFound, notFound.StatusCode);
        }
    }
}