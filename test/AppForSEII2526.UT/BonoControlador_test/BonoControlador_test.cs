using AppForMovies.UT;
using AppForSEII2526.API.Controller;
using AppForSEII2526.API.DTOs.CompraBonoDTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppForSEII2526.UT.BonoControlador_test
{
    public class BonoControlador_test : AppForMovies4SqliteUT
    {
        public BonoControlador_test()
        {
            // Tipos de bocadillo
            var tipos = new List<TipoBocadillo>()
            {
                new TipoBocadillo { IdTipo = 1, NombreTipo = "Vegano" },
                new TipoBocadillo { IdTipo = 2, NombreTipo = "Normal" },
                new TipoBocadillo { IdTipo = 3, NombreTipo = "Sin gluten" }
            };

            // Bonos con IDs específicos para coincidir con las expectativas del test
            var bonos = new List<BonoBocadillo>()
            {
                new BonoBocadillo { BonoId = 1, Nombre = "Bono Alpha", PVP = 9.5, NBocadillos = 2, CantidadDisponible = 10, TipoBocadillos = tipos[0] },
                new BonoBocadillo { BonoId = 2, Nombre = "Bono Beta", PVP = 12.0, NBocadillos = 3, CantidadDisponible = 5, TipoBocadillos = tipos[1] },
                new BonoBocadillo { BonoId = 3, Nombre = "Super Bono", PVP = 15.0, NBocadillos = 5, CantidadDisponible = 2, TipoBocadillos = tipos[2] }
            };

            _context.AddRange(tipos);
            _context.AddRange(bonos);
            _context.SaveChanges();
        }

        public static IEnumerable<object[]> TestCasesFor_GetBonos_OK()
        {
            // Los nombres se ordenan alfabéticamente en el controlador
            // IMPORTANTE: usar los IDs reales que tendrán en la BD (1, 2, 3)
            var expectedAll = new List<SelectBonoDTO>()
            {
                new SelectBonoDTO(1, "Bono Alpha", 9.5, 2, "Vegano"),
                new SelectBonoDTO(2, "Bono Beta", 12.0, 3, "Normal"),
                new SelectBonoDTO(3, "Super Bono", 15.0, 5, "Sin gluten")
            }.OrderBy(x => x.Nombre).ToList();

            // Filtro por nombre "Bono" -> incluye "Bono Alpha" y "Bono Beta"
            var expectedFilterName = expectedAll.Where(b => b.Nombre.Contains("Bono")).OrderBy(x => x.Nombre).ToList();

            // Filtro por tipo "Normal" -> solo "Bono Beta"
            var expectedTipoNormal = expectedAll.Where(b => b.Tipo == "Normal").ToList();

            return new List<object[]>
            {
                new object[] { null, null, expectedAll },
                new object[] { "Bono", null, expectedFilterName },
                new object[] { null, "Normal", expectedTipoNormal }
            };
        }

        [Theory]
        [MemberData(nameof(TestCasesFor_GetBonos_OK))]
        public async Task GetBonoParaCompra_OK_test(string? filtroNombre, string? tipoBocadillo, IList<SelectBonoDTO> expected)
        {
            // Arrange
            var mock = new Mock<ILogger<BonoControlador>>();
            var controller = new BonoControlador(_context, mock.Object);

            // Act
            var result = await controller.GetBonoParaCompra(filtroNombre, tipoBocadillo);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var actual = Assert.IsType<List<SelectBonoDTO>>(okResult.Value);

            // Comparación por valores (SelectBonoDTO no implementa Equals)
            // NOTA: El constructor SelectBonoDTO ignora el primer parámetro (BonoID), 
            // así que comparamos sin él
            var expectedTuples = expected.Select(e => (e.Nombre, e.Precio, e.NumeroDeBocadillos, e.Tipo)).ToList();
            var actualTuples = actual.Select(a => (a.Nombre, a.Precio, a.NumeroDeBocadillos, a.Tipo)).ToList();

            Assert.Equal(expectedTuples, actualTuples);
        }

        [Fact]
        public async Task GetBonoParaCompra_NotFound_test()
        {
            // Arrange
            var mock = new Mock<ILogger<BonoControlador>>();
            var controller = new BonoControlador(_context, mock.Object);

            // Primero forzamos que no haya bonos en la BD para cubrir la rama donde la lista queda vacía
            _context.BonosBocadillos.RemoveRange(_context.BonosBocadillos);
            _context.SaveChanges();

            // Act
            var result = await controller.GetBonoParaCompra(null, null);

            // Assert
            var notFound = Assert.IsType<NotFoundObjectResult>(result);

            // Comprobar que el mensaje exacto devuelto por el controlador está presente
            Assert.Equal("No hay bonos que cumplan los requisitos", notFound.Value);
        }
    }
}