using AppForSEII2526.API.Controller;
using AppForSEII2526.API.DTOs.ResenyaDTOs;
using AppForSEII2526.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace AppForSEII2526.UT.ResenyasControlador_test
{
    public class PostResenya_test : AppForMovies.UT.AppForMovies4SqliteUT
    {
        private readonly Bocadillo _b1;
        private readonly Bocadillo _b2;

        public PostResenya_test()
        {
            var tipoPan = new TipoPan { Nombre = "Barra" };
            _b1 = new Bocadillo { Nombre = "Atún", Pvp = 3.5F, Tamano = Tamaño.normal, tipopan = tipoPan };
            _b2 = new Bocadillo { Nombre = "Jamón", Pvp = 4.0F, Tamano = Tamaño.normal, tipopan = tipoPan };

            _context.AddRange(tipoPan, _b1, _b2);
            _context.SaveChanges();
        }

        [Fact]
        public async System.Threading.Tasks.Task CreateResenya_Returns_BadRequest_When_Items_Missing()
        {
            var logger = new Mock<ILogger<ResenyasControlador>>().Object;
            var controller = new ResenyasControlador(_context, logger);

            var dto = new ResenyaForCreateDTO(null, "T", "D", Resenya.ValoracionGeneral.Cinco, new List<ResenyaItemDTO>());

            var result = await controller.CreateResenya(dto);

            var bad = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<ValidationProblemDetails>(bad.Value);
        }

        [Fact]
        public async System.Threading.Tasks.Task CreateResenya_Returns_BadRequest_When_Puntuacion_Invalid()
        {
            var logger = new Mock<ILogger<ResenyasControlador>>().Object;
            var controller = new ResenyasControlador(_context, logger);

            var items = new List<ResenyaItemDTO> { new ResenyaItemDTO(_b1.Id, 11) }; // puntuación inválida
            var dto = new ResenyaForCreateDTO("u", "T", "D", Resenya.ValoracionGeneral.Cinco, items);

            var result = await controller.CreateResenya(dto);

            var bad = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<ValidationProblemDetails>(bad.Value);
        }

        [Fact]
        public async System.Threading.Tasks.Task CreateResenya_Returns_BadRequest_When_Duplicate_Bocadillo()
        {
            var logger = new Mock<ILogger<ResenyasControlador>>().Object;
            var controller = new ResenyasControlador(_context, logger);

            // Duplicado: mismo BocadilloId dos veces
            var items = new List<ResenyaItemDTO>
            {
                new ResenyaItemDTO(_b1.Id, 5),
                new ResenyaItemDTO(_b1.Id, 7)
            };
            var dto = new ResenyaForCreateDTO("u", "T", "D", Resenya.ValoracionGeneral.Cuatro, items);

            var result = await controller.CreateResenya(dto);

            var bad = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<ValidationProblemDetails>(bad.Value);
        }

        [Fact]
        public async System.Threading.Tasks.Task CreateResenya_Returns_BadRequest_When_Bocadillo_NotExists()
        {
            var logger = new Mock<ILogger<ResenyasControlador>>().Object;
            var controller = new ResenyasControlador(_context, logger);

            var items = new List<ResenyaItemDTO> { new ResenyaItemDTO(9999, 5) }; // id inexistente
            var dto = new ResenyaForCreateDTO("u", "T", "D", Resenya.ValoracionGeneral.Tres, items);

            var result = await controller.CreateResenya(dto);

            var bad = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<ValidationProblemDetails>(bad.Value);
        }

        [Fact]
        public async System.Threading.Tasks.Task CreateResenya_Returns_Created_When_Valid()
        {
            var logger = new Mock<ILogger<ResenyasControlador>>().Object;
            var controller = new ResenyasControlador(_context, logger);

            var items = new List<ResenyaItemDTO>
            {
                new ResenyaItemDTO(_b1.Id, 7),
                new ResenyaItemDTO(_b2.Id, 9)
            };
            var dto = new ResenyaForCreateDTO("juan", "Gran bocata", "Muy rico", Resenya.ValoracionGeneral.Cinco, items);

            var result = await controller.CreateResenya(dto);

            var created = Assert.IsType<CreatedAtActionResult>(result);
            var detail = Assert.IsType<ResenyaDetailDTO>(created.Value);

            Assert.Equal("juan", detail.NombreUsuario);
            Assert.Equal("Gran bocata", detail.Titulo);
            Assert.Equal(2, detail.ResenyaBocadillo.Count);
            Assert.True(_context.Resenyas.Any(r => r.Id == detail.Id));
        }
    }
}