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

namespace AppForSEII2526.UT.BocadilloController_test
{
    
    public class GetBocadillo_test : AppForMovies.UT.AppForMovies4SqliteUT
    {
        public GetBocadillo_test()
        {
            
            var panes = new List<TipoPan>
            {
                new TipoPan { Nombre = "Barra" },
                new TipoPan { Nombre = "Integral" },
                new TipoPan { Nombre = "Chapata" },
            };

            
            var bocadillos = new List<Bocadillo>
            {
                new Bocadillo { Nombre = "Atún con tomate",    Tamano = Tamaño.normal, tipopan = panes[0], Pvp = 3.0F },
                new Bocadillo { Nombre = "Jamón y queso",      Tamano = Tamaño.normal,  tipopan = panes[1], Pvp = 4 },
                new Bocadillo { Nombre = "Vegetal",            Tamano = Tamaño.pequeño, tipopan = panes[2], Pvp = 3 },
                new Bocadillo { Nombre = "Pollo asado",        Tamano = Tamaño.normal, tipopan = panes[0], Pvp = 3 },
                new Bocadillo { Nombre = "Lomo con queso",     Tamano = Tamaño.normal,  tipopan = panes[2], Pvp = 3 },
            };

            _context.AddRange(panes);
            _context.AddRange(bocadillos);
            _context.SaveChanges();
        }

        

        public static IEnumerable<object[]> TestCasesFor_GetBocadilloParaPedir_OK()
        {
           
            var all = new List<SelectBocadilloDTO>
            {
                new SelectBocadilloDTO(1, "Atún con tomate", Tamaño.pequeño, "Barra",   3.5F),
                new SelectBocadilloDTO(2, "Jamón y queso",   Tamaño.normal,  "Integral",4.2F),
                new SelectBocadilloDTO(3, "Vegetal",         Tamaño.pequeño, "Chapata", 3.9F),
                new SelectBocadilloDTO(4, "Pollo asado",     Tamaño.normal, "Barra",   3.1F),
                new SelectBocadilloDTO(5, "Lomo con queso",  Tamaño.normal,  "Chapata", 4),
            }
            .OrderBy(b => b.NombreBocadillo).ToList();

            // TC1: Sin filtros -> todos, ordenados por Nombre
            var tc1 = all;

            // TC2: Filtro solo por tamaño (Mediano)
            var tc2 = all.Where(b => b.Tamano == Tamaño.pequeño)
                         .OrderBy(b => b.NombreBocadillo).ToList();

            // TC3: Filtro solo por tipo de pan (Chapata)
            var tc3 = all.Where(b => b.TipoPanNombre == "Chapata")
                         .OrderBy(b => b.NombreBocadillo).ToList();

            // TC4: Ambos filtros: Tamaño=Grande y TipoPan=Integral
            var tc4 = all.Where(b => b.Tamano == Tamaño.normal && b.TipoPanNombre == "Integral")
                         .OrderBy(b => b.NombreBocadillo).ToList();

            // TC5: Ambos filtros: Tamaño=Grande y TipoPan=Chapata
            var tc5 = all.Where(b => b.Tamano == Tamaño.pequeño && b.TipoPanNombre == "Chapata")
                         .OrderBy(b => b.NombreBocadillo).ToList();

            // Devolvemos: filtroTamano, filtroTipoPan, expectedList
            return new List<object[]>
            {
                new object[] { null,               null,        tc1 },
                new object[] { "Mediano",          null,        tc2 },
                new object[] { null,               "Chapata",   tc3 },
                new object[] { "Grande",           "Integral",  tc4 },
                new object[] { "Grande",           "Chapata",   tc5 },
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

            // Comparamos por secuencia (Id, Nombre, Tamano, TipoPanNombre, Pvp)
            Assert.Equal(expectedBocadillos, dtoList);
        }

        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        public async Task GetBocadilloParaPedir_NotFound_test()
        {
            // Arrange
            var mock = new Mock<ILogger<BocadilloController>>();
            ILogger<BocadilloController> logger = mock.Object;
            var controller = new BocadilloController(_context, logger);

            // Usamos un filtro que no tiene coincidencias
            // (tamaño inexistente o tipo pan inexistente). El enum se parsea ignorando may/min,
            // pero si el tipo de pan no existe, la query devolverá vacía.
            var filtroTamano = "Grande";
            var filtroTipoPan = "PanInexistente";

            // Act
            var result = await controller.GetBocadilloParaPedir(filtroTamano, filtroTipoPan);

            // Assert
            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            var message = Assert.IsType<string>(notFound.Value);
            Assert.Equal("No hay bocadillos que cumplan los requisitos", message);
            Assert.Equal((int)HttpStatusCode.NotFound, notFound.StatusCode);
        }
    }
}
