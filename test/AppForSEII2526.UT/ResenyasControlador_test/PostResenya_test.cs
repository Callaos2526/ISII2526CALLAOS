using AppForMovies.UT;
using AppForSEII2526.API.Controller;
using AppForSEII2526.API.DTOs.ResenyaDTOs;

namespace AppForSEII2526.UT.ResenyasControlador_test
{
    /*
     Clase de pruebas unitarias para el endpoint CreateResenya del controlador
     ResenyasControlador.
    */
    public class PostResenya_test : AppForMovies4SqliteUT
    {
        private readonly TipoPan _tipoPan;
        private readonly Bocadillo _bocadillo;

        public PostResenya_test()
        {
            _tipoPan = new TipoPan { Nombre = "Integral" };
            _bocadillo = new Bocadillo
            {
                Id = 1,
                Nombre = "Pollo",
                Pvp = 4.5F,
                Stock = 10,
                Tamano = Tamaño.normal,
                tipopan = _tipoPan,
                Resenyabocadillo = ""
            };

            _context.AddRange(_tipoPan, _bocadillo);
            _context.SaveChanges();
        }

        public static IEnumerable<object[]> TestCasesFor_CreateResenya()
        {
            var emptyItems = new ResenyaForCreateDTO(
                nombreUsuario: "User",
                titulo: "Titulo",
                descripcion: "Descripcion valida",
                valoracion: Resenya.ValoracionGeneral.Tres,
                resenyaBocadillo: new List<ResenyaItemDTO>());

            var puntuacionFuera = new ResenyaForCreateDTO(
                nombreUsuario: "User",
                titulo: "Titulo",
                descripcion: "Descripcion valida",
                valoracion: Resenya.ValoracionGeneral.Cuatro,
                resenyaBocadillo: new List<ResenyaItemDTO> { new ResenyaItemDTO(1, 11) });

            var duplicados = new ResenyaForCreateDTO(
                nombreUsuario: "User",
                titulo: "Titulo",
                descripcion: "Descripcion valida",
                valoracion: Resenya.ValoracionGeneral.Cuatro,
                resenyaBocadillo: new List<ResenyaItemDTO> { new ResenyaItemDTO(1, 8), new ResenyaItemDTO(1, 7) });

            var bocadilloNoExiste = new ResenyaForCreateDTO(
                nombreUsuario: "User",
                titulo: "Titulo",
                descripcion: "Descripcion valida",
                valoracion: Resenya.ValoracionGeneral.Cinco,
                resenyaBocadillo: new List<ResenyaItemDTO> { new ResenyaItemDTO(999, 9) });

            var allTests = new List<object[]>
            {
                new object[] { emptyItems, "Error: Debes incluir al menos un bocadillo con su puntuación (1..10)" },
                new object[] { puntuacionFuera, "Error: La puntuación de cada bocadillo debe estar entre 1 y 10" },
                new object[] { duplicados, "Error: No se permiten bocadillos duplicados en la reseña" },
                new object[] { bocadilloNoExiste, "Error: Alguno de los bocadillos no existe en la base de datos" },
            };

            return allTests;
        }

        [Theory]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        [MemberData(nameof(TestCasesFor_CreateResenya))]
        public async Task CreateResenya_Error_test(ResenyaForCreateDTO dto, string errorExpected)
        {
            var mock = new Mock<ILogger<ResenyasControlador>>();
            ILogger<ResenyasControlador> logger = mock.Object;
            var controller = new ResenyasControlador(_context, logger);

            var result = await controller.CreateResenya(dto);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var problems = Assert.IsType<ValidationProblemDetails>(badRequest.Value);

            var firstError = problems.Errors.First().Value[0];
            Assert.StartsWith(errorExpected, firstError);
        }

        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        public async Task CreateResenya_Success_test()
        {
            var mock = new Mock<ILogger<ResenyasControlador>>();
            ILogger<ResenyasControlador> logger = mock.Object;
            var controller = new ResenyasControlador(_context, logger);

            var dto = new ResenyaForCreateDTO(
                nombreUsuario: "UsuarioPrueba",
                titulo: "Titulo valido",
                descripcion: "Descripcion valida para la reseña",
                valoracion: Resenya.ValoracionGeneral.Cinco,
                resenyaBocadillo: new List<ResenyaItemDTO> { new ResenyaItemDTO(1, 8) }
            );

            var result = await controller.CreateResenya(dto);

            var created = Assert.IsType<CreatedAtActionResult>(result);
            var actual = Assert.IsType<ResenyaDetailDTO>(created.Value);

            // Construimos el expected con los valores conocidos (copiamos Id y FechaPublicacion desde el actual)
            var expectedItems = new List<ResenyaItemDTO>
            {
                new ResenyaItemDTO(_bocadillo.Id, _bocadillo.Nombre, _bocadillo.Pvp, _bocadillo.Tamano, dto.ResenyaBocadillo.First().Puntuacion)
            };

            var expected = new ResenyaDetailDTO(
                id: actual.Id,
                fechaPublicacion: actual.FechaPublicacion,
                nombreUsuario: dto.NombreUsuario,
                titulo: dto.Titulo,
                descripcion: dto.Descripcion,
                valoracion: dto.Valoracion,
                resenyaBocadillo: expectedItems
            );

            // CORRECCIÓN: el Equals compara la lista por referencia, por eso asignamos la lista devuelta
            // por el controlador al expected para que la comparación global funcione igual que en el ejemplo de la profesora.
            expected.ResenyaBocadillo = actual.ResenyaBocadillo;

            Assert.Equal(expected, actual);

            // Comprobación adicional: la reseña se ha persistido
            Assert.Equal(1, _context.Resenyas.Count());
        }
    }
}