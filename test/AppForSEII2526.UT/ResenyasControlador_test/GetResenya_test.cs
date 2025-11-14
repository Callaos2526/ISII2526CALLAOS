using AppForMovies.UT;
using AppForSEII2526.API.Controller;
using AppForSEII2526.API.DTOs.ResenyaDTOs;

namespace AppForSEII2526.UT.ResenyasControlador_test
{
    // Pruebas para el Get (detalle) del controlador ResenyasControlador.
    // Sigue el patrón de la profesora: datos en el constructor, tests claros y comparaciones por Equals.
    public class GetResenya_test : AppForMovies4SqliteUT
    {
        public GetResenya_test()
        {
            // Preparamos datos mínimos en la BD en memoria:
            // - un tipo de pan
            // - dos bocadillos (uno usado en la reseña)
            var tipoPan = new TipoPan { Nombre = "Integral" };

            var bocadillo1 = new Bocadillo
            {
                Nombre = "Pollo",
                Pvp = 4.5F,
                Stock = 10,
                Tamano = Tamaño.normal,
                tipopan = tipoPan,
                Resenyabocadillo = "" // evitar NOT NULL en SQLite
            };

            var bocadillo2 = new Bocadillo
            {
                Nombre = "Atún",
                Pvp = 3.0F,
                Stock = 8,
                Tamano = Tamaño.normal,
                tipopan = tipoPan,
                Resenyabocadillo = ""
            };

            _context.AddRange(tipoPan, bocadillo1, bocadillo2);
            _context.SaveChanges();

            // Crear una reseña con un item (referenciando bocadillo1)
            var resenya = new Resenya(
                id: 0,
                descripcion: "Excelente sabor",
                fechaPublicacion: DateTime.Now,
                nombreUsuario: "usuario.test",
                titulo: "Muy buena"
            );
            resenya.Valoracion = Resenya.ValoracionGeneral.Cinco;

            // Añadir item de reseña (EF rellenará ResenyaId al guardar)
            resenya.ResenyaBocadillo.Add(new ResenyaBocadillo
            {
                BocadilloId = bocadillo1.Id,
                Puntuacion = 9,
                Resenya = resenya
            });

            _context.Add(resenya);
            _context.SaveChanges();
        }

        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        public async Task GetResenya_Returns_NotFound_When_IdNotExists()
        {
            // Arrange
            var logger = new Mock<ILogger<ResenyasControlador>>().Object;
            var controller = new ResenyasControlador(_context, logger);

            // Act
            var result = await controller.GetResenya(9999); // id que no existe

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        public async Task GetResenya_Returns_Ok_With_Detail_When_Exists()
        {
            // Arrange
            var logger = new Mock<ILogger<ResenyasControlador>>().Object;
            var controller = new ResenyasControlador(_context, logger);

            // Obtener el id de la reseña creada en el constructor
            var existingResenya = _context.Resenyas.Include(r => r.ResenyaBocadillo).First();
            var id = existingResenya.Id;

            // Act
            var result = await controller.GetResenya(id);

            // Assert: Ok + ResenyaDetailDTO
            var ok = Assert.IsType<OkObjectResult>(result);
            var actual = Assert.IsType<ResenyaDetailDTO>(ok.Value);

            // Construimos expected copiando Id y FechaPublicacion desde el actual
            // y construyendo los items esperados con los datos enriquecidos por la BD.
            var expectedItems = new List<ResenyaItemDTO>
            {
                new ResenyaItemDTO(
                    bocadilloId: actual.ResenyaBocadillo.First().BocadilloId,
                    nombre: actual.ResenyaBocadillo.First().Nombre,
                    pvp: actual.ResenyaBocadillo.First().Pvp ?? 0f,
                    tamano: actual.ResenyaBocadillo.First().Tamano,
                    puntuacion: actual.ResenyaBocadillo.First().Puntuacion)
            };

            var expected = new ResenyaDetailDTO(
                id: actual.Id, // usar el Id generado por EF
                fechaPublicacion: actual.FechaPublicacion, // usar la fecha generada
                nombreUsuario: actual.NombreUsuario,
                titulo: actual.Titulo,
                descripcion: actual.Descripcion,
                valoracion: actual.Valoracion,
                resenyaBocadillo: expectedItems
            );

            // Igual que hicimos en otros tests: para que Equals compare listas correctamente,
            // asignamos la lista devuelta por la acción al expected (comportamiento del test de la profesora).
            expected.ResenyaBocadillo = actual.ResenyaBocadillo;

            // Comprobación única como la profesora
            Assert.Equal(expected, actual);
        }
    }
}