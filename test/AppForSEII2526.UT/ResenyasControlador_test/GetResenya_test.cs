using AppForSEII2526.API.Controller;
using AppForSEII2526.API.DTOs.ResenyaDTOs;
using AppForSEII2526.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace AppForSEII2526.UT.ResenyasControlador_test
{
    public class GetResenya_test : AppForMovies.UT.AppForMovies4SqliteUT
    {
        private readonly Bocadillo _boc;
        private readonly Resenya _seedResenya;

        public GetResenya_test()
        {
            // Seed mínimo: tipo de pan y bocadillo
            var tipoPan = new TipoPan { Nombre = "Barra" };
            _boc = new Bocadillo { Nombre = "Atún", Pvp = 3.5F, Tamano = Tamaño.normal, tipopan = tipoPan };

            _context.AddRange(tipoPan, _boc);
            _context.SaveChanges();

            // Añadimos una reseña existente con un item
            _seedResenya = new Resenya(
                id: 0,
                descripcion: "Buena",
                fechaPublicacion: DateTime.UtcNow,
                nombreUsuario: "user1",
                titulo: "Muy buena"
            )
            {
                Valoracion = Resenya.ValoracionGeneral.Cuatro,
                ResenyaBocadillo = new List<ResenyaBocadillo>
                {
                    new ResenyaBocadillo { Bocadillo = _boc, BocadilloId = _boc.Id, Puntuacion = 8 }
                }
            };

            _context.Add(_seedResenya);
            _context.SaveChanges();
        }

        [Fact]
        public async System.Threading.Tasks.Task GetResenya_Returns_Ok_With_Detail()
        {
            var logger = new Mock<ILogger<ResenyasControlador>>().Object;
            var controller = new ResenyasControlador(_context, logger);

            var existingId = _context.Resenyas.First().Id;

            var result = await controller.GetResenya(existingId);

            var ok = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<ResenyaDetailDTO>(ok.Value);

            Assert.Equal(existingId, dto.Id);
            Assert.Equal("user1", dto.NombreUsuario);
            Assert.Equal("Muy buena", dto.Titulo);
            Assert.Single(dto.ResenyaBocadillo);
            Assert.Equal(_boc.Id, dto.ResenyaBocadillo.First().BocadilloId);
        }

        [Fact]
        public async System.Threading.Tasks.Task GetResenya_Returns_NotFound_When_Missing()
        {
            var logger = new Mock<ILogger<ResenyasControlador>>().Object;
            var controller = new ResenyasControlador(_context, logger);

            var result = await controller.GetResenya(999999);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}